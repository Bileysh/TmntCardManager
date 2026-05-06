using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmntCardManager.Models;
using TmntCardManager.Models.Data;

namespace TmntCardManager.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class InventoryApiController : ControllerBase
{
    private readonly TmntCardsDbContext _context;
    private readonly UserManager<User> _userManager;

    public InventoryApiController(TmntCardsDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetMyInventory()
    {
        var userIdString = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
        var userId = int.Parse(userIdString);

        var inventory = await _context.Usercards
            .Where(uc => uc.UserId == userId)
            .Include(uc => uc.Card) 
            .ThenInclude(c => c.Class) 
            .Select(uc => new 
            {
                uc.Id,
                uc.Quantity,
                Card = new 
                {
                    uc.Card.Id,
                    uc.Card.Name,
                    ClassName = uc.Card.Class.Name,
                    uc.Card.Strength,
                    uc.Card.Agility,
                    uc.Card.Skill,
                    uc.Card.Wit
                }
            })
            .ToListAsync();

        return Ok(inventory);
    }
    
    [HttpPost("add/{cardId}")]
    public async Task<IActionResult> AddCardToInventory(int cardId)
    {
        var userIdString = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
        var userId = int.Parse(userIdString);

        var cardExists = await _context.Cards.AnyAsync(c => c.Id == cardId);
        if (!cardExists) return NotFound(new { message = "Такої картки не існує в грі." });

        var existingUserCard = await _context.Usercards
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CardId == cardId);

        if (existingUserCard != null)
        {
            existingUserCard.Quantity += 1;
        }
        else
        {
            var newUserCard = new UserCard
            {
                UserId = userId,
                CardId = cardId,
                Quantity = 1
            };
            _context.Usercards.Add(newUserCard);
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Картку успішно додано до вашого інвентарю!" });
    }
}