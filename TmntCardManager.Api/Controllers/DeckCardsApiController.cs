using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmntCardManager.Api.DTOs;
using TmntCardManager.Models;
using TmntCardManager.Models.Data;

namespace TmntCardManager.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DeckCardsApiController : ControllerBase
{
    private readonly TmntCardsDbContext _context;
    private readonly UserManager<User> _userManager;

    public DeckCardsApiController(TmntCardsDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    [HttpGet("{deckId}")]
    public async Task<ActionResult<IEnumerable<CardOutputDto>>> GetCardsInDeck(int deckId)
    {
        var cards = await _context.Deckcards
            .Where(dc => dc.Deckid == deckId)
            .Include(dc => dc.Card)
            .ThenInclude(c => c.Class) 
            .Select(dc => new CardOutputDto
            {
                Id = dc.Card!.Id,
                Name = dc.Card.Name,
                ImageUrl = dc.Card.Imageurl,
                Strength = dc.Card.Strength,
                Agility = dc.Card.Agility,
                Skill = dc.Card.Skill,
                Wit = dc.Card.Wit,
                ClassId = dc.Card.Classid,
                ClassName = dc.Card.Class.Name
            })
            .ToListAsync();
            
        return Ok(cards);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddCardToDeck([FromBody] DeckCardInputDto dto)
    {
        var currentUserId = int.Parse(_userManager.GetUserId(User)!);
        
        var cardsInDeckCount = await _context.Deckcards.CountAsync(dc => dc.Deckid == dto.DeckId);
        if (cardsInDeckCount >= 5)
        {
            return BadRequest(new { message = "Ліміт вичерпано! У колоді вже є 5 карток." });
        }
        
        var deck = await _context.Decks.FindAsync(dto.DeckId);
        if (deck == null) return NotFound(new { message = "Колоду не знайдено." });
        if (deck.Userid != currentUserId && !User.IsInRole("Admin")) 
            return Forbid(); 

        var userCard = await _context.Usercards
            .FirstOrDefaultAsync(uc => uc.UserId == currentUserId && uc.CardId == dto.CardId);
        if (userCard == null)
            return BadRequest(new { message = "У тебе немає цієї карти в інвентарі!" });
                
        var sameCardsInDeckCount = await _context.Deckcards
            .CountAsync(dc => dc.Deckid == dto.DeckId && dc.Cardid == dto.CardId);
        if (sameCardsInDeckCount >= userCard.Quantity)
            return BadRequest(new { message = $"У тебе є лише {userCard.Quantity} копій цієї карти, і всі вони вже в колоді!" });
        
        var deckCard = new Deckcard
        {
            Deckid = dto.DeckId,
            Cardid = dto.CardId
        };

        _context.Deckcards.Add(deckCard);
        await _context.SaveChangesAsync();
        
        return Ok(new { message = "Карту успішно додано до колоди!" });
    }

    [HttpDelete("{deckId}/{cardId}")]
    public async Task<IActionResult> RemoveCardFromDeck(int deckId, int cardId)
    {
        var currentUserId = int.Parse(_userManager.GetUserId(User)!);
        
        var deck = await _context.Decks.FindAsync(deckId);
        if (deck == null) return NotFound();
        if (deck.Userid != currentUserId && !User.IsInRole("Admin")) return Forbid();

        var item = await _context.Deckcards
            .FirstOrDefaultAsync(dc => dc.Deckid == deckId && dc.Cardid == cardId);
        if (item == null) return NotFound(new { message = "Цієї карти немає в даній колоді." });
        
        _context.Deckcards.Remove(item);
        await _context.SaveChangesAsync();
        
        return Ok(new { message = "Карту вилучено з колоди." });
    }
}