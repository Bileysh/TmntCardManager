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
public class PlayerProfilesApiController : ControllerBase
{
    private readonly TmntCardsDbContext _context;
    private readonly UserManager<User> _userManager;
    
    public PlayerProfilesApiController(TmntCardsDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerProfileOutputDto>>> GetProfiles() 
    {
        var profiles = await _context.Playerprofiles
            .Select(p => new PlayerProfileOutputDto
            {
                Id = p.Id,
                Nickname = p.Nickname,
                Winrate = p.Winrate,
                Avatarurl = p.Avatarurl,
                Experience = p.Experience,
                Level = p.Level,
                BalanceCoins = p.BalanceCoins
            })
            .ToListAsync();
            
        return Ok(profiles);
    }
    [HttpGet("my")]
    public async Task<ActionResult<PlayerProfileOutputDto>> GetMyProfile()
    {
        var userId = int.Parse(_userManager.GetUserId(User)!); 
        var profile = await _context.Playerprofiles
            .Where(p => p.Id == userId)
            .Select(p => new PlayerProfileOutputDto
            {
                Id = p.Id,
                Nickname = p.Nickname,
                Winrate = p.Winrate,
                Avatarurl = p.Avatarurl,
                Experience = p.Experience,
                Level = p.Level,
                BalanceCoins = p.BalanceCoins
            })
            .FirstOrDefaultAsync();

        return profile != null ? Ok(profile) : NotFound(new { message = "Профіль не знайдено" });
    }
    
    [HttpPut("my")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] PlayerProfileInputDto dto)
    {
        var userId = int.Parse(_userManager.GetUserId(User)!); 
        var profile = await _context.Playerprofiles.FirstOrDefaultAsync(p => p.Id == userId);
        
        if (profile == null) return NotFound(new { message = "Профіль не знайдено" });

        profile.Nickname = dto.Nickname;
        profile.Avatarurl = dto.Avatarurl;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Профіль успішно оновлено!" });
    }
}