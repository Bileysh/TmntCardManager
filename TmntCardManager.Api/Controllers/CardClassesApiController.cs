using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmntCardManager.Api.DTOs;
using TmntCardManager.Models;
using TmntCardManager.Models.Data;

namespace TmntCardManager.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CardClassesApiController : ControllerBase
{
    private readonly TmntCardsDbContext _context;

    public CardClassesApiController(TmntCardsDbContext context)
    {
        _context = context;
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<CardClassOutputDto>> GetClass(int id)
    {
        var cardClass = await _context.Cardclasses
            .Where(c => c.Id == id)
            .Select(c => new CardClassOutputDto 
            { 
                Id = c.Id, 
                Name = c.Name, 
                Description = c.Description 
            })
            .FirstOrDefaultAsync();

        return cardClass == null ? NotFound() : Ok(cardClass);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CardClassOutputDto>>> GetClasses()
    {
        var classes = await _context.Cardclasses
            .Select(c => new CardClassOutputDto 
            { 
                Id = c.Id, 
                Name = c.Name, 
                Description = c.Description 
            })
            .ToListAsync();
            
        return Ok(classes);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<CardClassOutputDto>> CreateClass([FromBody] CardClassInputDto dto)
    {
        var newCardClass = new Cardclass 
        { 
            Name = dto.Name, 
            Description = dto.Description 
        };

        _context.Cardclasses.Add(newCardClass);
        await _context.SaveChangesAsync();

        var resultDto = new CardClassOutputDto 
        { 
            Id = newCardClass.Id, 
            Name = newCardClass.Name, 
            Description = newCardClass.Description 
        };

        return CreatedAtAction(nameof(GetClass), new { id = resultDto.Id }, resultDto);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClass(int id, [FromBody] CardClassInputDto dto)
    {
        var cardClass = await _context.Cardclasses.FindAsync(id);
        if (cardClass == null) return NotFound();

        cardClass.Name = dto.Name;
        cardClass.Description = dto.Description;

        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClass(int id)
    {
        var cardClass = await _context.Cardclasses.FindAsync(id);
        if (cardClass == null) return NotFound();
        _context.Cardclasses.Remove(cardClass);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Фракцію видалено" });
    }
}