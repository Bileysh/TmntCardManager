using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmntCardManager.Api.DTOs;
using TmntCardManager.Models;
using TmntCardManager.Models.Data;
using TmntCardManager.Services;

namespace TmntCardManager.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DecksApiController : ControllerBase
{
    private readonly TmntCardsDbContext _context;
    private readonly DeckExportService _exportService;
    private readonly UserManager<User> _userManager;
    
    public DecksApiController(
        TmntCardsDbContext context, 
        DeckExportService exportService, 
        UserManager<User> userManager)
    {
        _context = context;
        _exportService = exportService;
        _userManager = userManager;
    }

    [HttpGet]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeckOutputDto>>> GetDecks() 
    {
        var decks = await _context.Decks
            .Include(d => d.User)
            .Include(d => d.Deckcards)
            .Select(d => new DeckOutputDto
            {
                Id = d.Id,
                Name = d.Name,
                CreatedAt = d.Createdat,
                OwnerId = d.Userid,
                OwnerName = d.User != null ? d.User.UserName : "Невідомо",
                TotalCards = d.Deckcards!.Count 
            })
            .ToListAsync();

        return Ok(decks);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<DeckOutputDto>> GetDeck(int id)
    {
        var deck = await _context.Decks
            .Include(d => d.User)
            .Include(d => d.Deckcards)
            .Where(d => d.Id == id)
            .Select(d => new DeckOutputDto
            {
                Id = d.Id,
                Name = d.Name,
                CreatedAt = d.Createdat,
                OwnerId = d.Userid,
                OwnerName = d.User != null ? d.User.UserName : "Невідомо",
                TotalCards = d.Deckcards!.Count
            })
            .FirstOrDefaultAsync();

        if (deck == null) return NotFound(new { message = "Колоду не знайдено." });
        return Ok(deck);
    }
    
    [HttpPost]
    public async Task<ActionResult<DeckOutputDto>> CreateDeck([FromBody] DeckInputDto dto)
    {
        var userIdString = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
        var userId = int.Parse(userIdString);

        var newDeck = new Deck
        {
            Name = dto.Name,
            Userid = userId,
            Createdat = DateTime.UtcNow 
        };

        _context.Decks.Add(newDeck);
        await _context.SaveChangesAsync();

        var owner = await _userManager.FindByIdAsync(userIdString);
        var resultDto = new DeckOutputDto
        {
            Id = newDeck.Id,
            Name = newDeck.Name,
            CreatedAt = newDeck.Createdat,
            OwnerId = newDeck.Userid,
            OwnerName = owner?.UserName ?? "Невідомо",
            TotalCards = 0
        };

        return CreatedAtAction(nameof(GetDeck), new { id = newDeck.Id }, resultDto);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDeck(int id)
    {
        var deck = await _context.Decks.FindAsync(id);
        if (deck == null) return NotFound();

        var currentUserId = int.Parse(_userManager.GetUserId(User)!);
        if (deck.Userid != currentUserId && !User.IsInRole("Admin"))
        {
            return Forbid(); 
        }

        _context.Decks.Remove(deck);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Колоду успішно видалено" });
    }
    
    [HttpGet("{id}/export")]
    public async Task<IActionResult> Export(int id)
    {
        var stream = new MemoryStream();
        await _exportService.ExportDeckAsync(stream, id, HttpContext.RequestAborted); 
        
        stream.Position = 0; 
        return File(stream, "application/json", $"deck_{id}.json");
    }
}