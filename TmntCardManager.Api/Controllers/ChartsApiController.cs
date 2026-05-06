using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmntCardManager.Models.Data;

namespace TmntCardManager.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ChartsApiController : ControllerBase
{
    private readonly TmntCardsDbContext _context;

    public ChartsApiController(TmntCardsDbContext context)
    {
        _context = context;
    }

    [HttpGet("cardsByFraction")]
    public async Task<IActionResult> GetCardsByFraction()
    {
        var data = await _context.Cards
            .Include(c => c.Class)
            .GroupBy(c => c.Class.Name)
            .Select(g => new { label = g.Key, value = g.Count() })
            .ToListAsync();
        return Ok(data);
    }

    [HttpGet("deckStats/{deckId}")]
    public async Task<IActionResult> GetDeckStats(int deckId)
    {
        var stats = await _context.Deckcards
            .Where(dc => dc.Deckid == deckId)
            .Include(dc => dc.Card)
            .Select(dc => new { dc.Card.Name, dc.Card.Strength, dc.Card.Agility })
            .ToListAsync();
        return Ok(stats);
    }
}