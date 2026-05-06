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
public class TournamentsApiController : ControllerBase
{
    private readonly TmntCardsDbContext _context;

    public TournamentsApiController(TmntCardsDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateTournament([FromBody] CreateTournamentDto dto)
    {
        if (dto.EndDate <= dto.StartDate)
        {
            return BadRequest(new { message = "Помилка: Дата завершення має бути пізніше за дату початку!" });
        }
        
        var tournament = new Tournament
        {
            Name = dto.Name,
            Description = dto.Description,
            PrizeCoins = dto.PrizeCoins,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsActive = true
        };

        _context.Tournaments.Add(tournament);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Турнір успішно створено!", tournamentId = tournament.Id });
    }

    [AllowAnonymous]
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveTournaments()
    {
        var tournaments = await _context.Tournaments
            .Where(t => t.IsActive)
            .OrderBy(t => t.EndDate)
            .ToListAsync();

        return Ok(tournaments);
    }

    [AllowAnonymous]
    [HttpGet("{id}/leaderboard")]
    public async Task<IActionResult> GetTournamentLeaderboard(int id)
    {
        var matches = await _context.Matches
            .Include(m => m.Winner)
            .Where(m => m.TournamentId == id && m.WinnerId != null)
            .ToListAsync();

        var leaderboard = matches
            .GroupBy(m => m.Winner!.UserName) 
            .Select(g => new TournamentLeaderboardDto
            {
                Nickname = g.Key!,
                Wins = g.Count() 
            })
            .OrderByDescending(x => x.Wins)
            .ToList();

        return Ok(leaderboard);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/finish")]
    public async Task<IActionResult> FinishTournament(int id)
    {
        var tournament = await _context.Tournaments.FindAsync(id);
        if (tournament == null || !tournament.IsActive)
            return BadRequest(new { message = "Турнір не знайдено або він вже завершений." });

        var topPlayerId = await _context.Matches
            .Where(m => m.TournamentId == id && m.WinnerId != null)
            .GroupBy(m => m.WinnerId)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefaultAsync();

        if (topPlayerId != null)
        {
            var winnerProfile = await _context.Playerprofiles.FindAsync(topPlayerId);
            if (winnerProfile != null)
            {
                winnerProfile.BalanceCoins += tournament.PrizeCoins; 
            }
        }

        tournament.IsActive = false; 
        await _context.SaveChangesAsync();

        if (topPlayerId == null)
            return Ok(new { message = "Турнір завершено. Жодного матчу не було зіграно, приз ніхто не отримав." });

        var winner = await _context.Users.FindAsync(topPlayerId);
        return Ok(new { message = $"Турнір завершено! Переможець: {winner?.UserName}. Приз {tournament.PrizeCoins} монет видано!" });
    }
    
    [HttpPost("{id}/join")]
    public async Task<IActionResult> JoinTournament(int id)
    {
        var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

        var tournament = await _context.Tournaments.FindAsync(id);
        if (tournament == null || !tournament.IsActive)
            return BadRequest(new { message = "Турнір не знайдено або він вже завершений." });

        var profile = await _context.Playerprofiles.FindAsync(currentUserId);
        if (profile == null) return NotFound("Профіль не знайдено");

        profile.ActiveTournamentId = id; 
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Ти успішно приєднався до турніру '{tournament.Name}'! Тепер можеш шукати суперників." });
    }
}