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
public class MatchesApiController : ControllerBase
{
    private readonly TmntCardsDbContext _context;
    private readonly UserManager<User> _userManager;

    public MatchesApiController(TmntCardsDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost("play")]
    public async Task<IActionResult> PlayMatch([FromBody] PlayMatchInputDto dto)
    {
        var currentUserId = int.Parse(_userManager.GetUserId(User)!);

        var myDeck = await _context.Decks
            .Include(d => d.Deckcards)!
            .ThenInclude(dc => dc.Card)
            .FirstOrDefaultAsync(d => d.Id == dto.DeckId && d.Userid == currentUserId);

        if (myDeck == null) return NotFound(new { message = "Колоду не знайдено." });
        if (myDeck.Deckcards != null && myDeck.Deckcards.Count != 5)
            return BadRequest(new { message = "Для бою потрібно рівно 5 карт у колоді!" });

        var playerprofile = await _context.Playerprofiles.FindAsync(currentUserId);

        var query = _context.Decks
            .Include(d => d.Deckcards)!.ThenInclude(dc => dc.Card)
            .Include(d => d.User)
            .Where(d => d.Userid != currentUserId && d.Deckcards!.Count == 5);

        if (dto.TournamentId.HasValue)
        {
            if (playerprofile?.ActiveTournamentId != dto.TournamentId)
                return BadRequest(new { message = "Ти не зареєстрований у цьому турнірі! Спочатку приєднайся до нього через /join." });

            query = query.Where(d => _context.Playerprofiles.Any(p => p.Id == d.Userid && p.ActiveTournamentId == dto.TournamentId));
        }

        var opponentDeck = await query.OrderBy(d => Guid.NewGuid()).FirstOrDefaultAsync();

        if (opponentDeck == null) 
        {
            string errorMsg = dto.TournamentId.HasValue 
                ? "У цьому турнірі поки немає інших гравців з готовою колодою. Почекай, поки хтось приєднається!" 
                : "Не знайдено жодного суперника з повною колодою.";
            return BadRequest(new { message = errorMsg });
        }
        var opponentId = opponentDeck.Userid;
        var opponentName = opponentDeck.User?.UserName ?? "Невідомий Ворог";

        var myCards = myDeck.Deckcards!
            .Select(dc => dc.Card)
            .ToList();
        var opponentCards = opponentDeck.Deckcards!
            .Select(dc => dc.Card)
            .OrderBy(c => Guid.NewGuid())
            .ToList();

        var battleLog = new List<string>();
        battleLog.Add(
            $"⚔️ БІЙ ПОЧАВСЯ! Твоя колода '{myDeck.Name}' проти колоди '{opponentDeck.Name}' (Гравець: {opponentName}) ⚔️");

        int myWins = 0;
        int opponentWins = 0;
        var random = new Random();

        for (int i = 0; i < 5; i++)
        {
            var myCard = myCards[i];
            var oppCard = opponentCards[i];

            battleLog.Add($"\n--- РАУНД {i + 1}: {myCard!.Name} VS {oppCard!.Name} ---");

            int? myHp = myCard.Wit * 2;
            int? oppHp = oppCard.Wit * 2;

            int myArmor = (int)(myCard.Agility * 0.5)!;
            int oppArmor = (int)(oppCard.Agility * 0.5)!;

            int? myCritChance = myCard.Skill / 300;
            int? oppCritChance = oppCard.Skill / 300;

            if (myCritChance > 35) myCritChance = 35;
            if (oppCritChance > 35) oppCritChance = 35;

            int turn = 1;

            while (myHp > 0 && oppHp > 0 && turn <= 20)
            {
                bool myCrit = random.Next(1, 101) <= myCritChance;
                int? myDamage = myCard.Strength * (myCrit ? 2 : 1) - oppArmor;
                if (myDamage < 100) myDamage = 100;

                oppHp -= myDamage;
                string critText = myCrit ? "💥 КРИТ!" : "";
                battleLog.Add($"Ти б'єш: {myCard.Name} наносить {myDamage} урону {critText}. HP ворога: {oppHp}");

                if (oppHp <= 0) break;

                bool oppCrit = random.Next(1, 101) <= oppCritChance;
                int? oppDamage = oppCard.Strength * (oppCrit ? 2 : 1) - myArmor;
                if (oppDamage < 100) oppDamage = 100;

                myHp -= oppDamage;
                string oppCritText = oppCrit ? "💥 КРИТ!" : "";
                battleLog.Add($"Ворог б'є: {oppCard.Name} наносить {oppDamage} урону {oppCritText}. Твоє HP: {myHp}");

                turn++;
            }

            if (myHp > 0)
            {
                battleLog.Add($"🏆 {myCard.Name} перемагає у раунді {i + 1}!");
                myWins++;
            }
            else
            {
                battleLog.Add($"💀 {myCard.Name} падає. {oppCard.Name} перемагає у раунді {i + 1}.");
                opponentWins++;
            }
        }

        bool isMyWin = myWins > opponentWins;
        string matchStatus = isMyWin ? "Перемога!" : "Поразка...";
        battleLog.Add($"\n🏁 ПІДСУМОК: Рахунок {myWins} : {opponentWins}. {matchStatus}");

        int earnedCoins = isMyWin ? 50 : 10;
        int earnedXp = isMyWin ? 100 : 25;

        var myProfile = await _context.Playerprofiles.FindAsync(currentUserId);
        var oppProfile = await _context.Playerprofiles.FindAsync(opponentId);

        if (myProfile != null)
        {
            myProfile.BalanceCoins += earnedCoins;
            myProfile.Experience += earnedXp;

            int requiredXp = myProfile.Level * 500;
            bool leveledUp = false;

            while (myProfile.Experience >= requiredXp)
            {
                myProfile.Experience -= requiredXp;
                myProfile.Level++;
                leveledUp = true;

                requiredXp = myProfile.Level * 500;
            }

            if (leveledUp)
            {
                battleLog.Add($"\n🎉 ВІТАЄМО! Ти досяг {myProfile.Level} рівня! 🎉");
                myProfile.BalanceCoins += 200;
                battleLog.Add("Бонус за новий рівень: +200 монет!");
            }
        }

        var matchRecord = new Match
            {
                Player1Id = currentUserId,
                Player2Id = opponentId,
                WinnerId = isMyWin ? currentUserId : opponentId,
                PlayedAt = DateTime.UtcNow,
                TournamentId = dto.TournamentId
            };
            _context.Matches.Add(matchRecord);

            await _context.SaveChangesAsync();
            await RecalculateWinrate(currentUserId);
            await RecalculateWinrate(opponentId);

            var result = new MatchResultDto
            {
                MatchStatus = matchStatus,
                OpponentName = opponentName,
                EarnedCoins = earnedCoins,
                EarnedXp = earnedXp,
                BattleLog = battleLog
            };

            return Ok(result);
        }

    private async Task RecalculateWinrate(int playerId)
    {
        var totalMatches = await _context.Matches.CountAsync(m => m.Player1Id == playerId || m.Player2Id == playerId);
        if (totalMatches == 0) return;

        var totalWins = await _context.Matches.CountAsync(m => m.WinnerId == playerId);
        
        var profile = await _context.Playerprofiles.FindAsync(playerId);
        if (profile != null)
        {
            profile.Winrate = Math.Round((double)totalWins / totalMatches * 100, 2);
            await _context.SaveChangesAsync();
        }
    }
    
    [AllowAnonymous] 
    [HttpGet("leaderboard")]
    public async Task<IActionResult> GetLeaderboard()
    {
        var topPlayers = await _context.Playerprofiles
            .OrderByDescending(p => p.Level)       
            .ThenByDescending(p => p.Winrate)      
            .Take(10)
            .Select(p => new 
            {
                p.Nickname,
                p.Level,
                p.Winrate,
                p.Experience
            })
            .ToListAsync();

        return Ok(topPlayers);
    }
    
    [HttpGet("history")]
    public async Task<IActionResult> GetMatchHistory()
    {
        var currentUserId = int.Parse(_userManager.GetUserId(User)!);

        var history = await _context.Matches
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .Where(m => m.Player1Id == currentUserId || m.Player2Id == currentUserId)
            .OrderByDescending(m => m.PlayedAt) 
            .Select(m => new 
            {
                MatchId = m.Id,
                OpponentName = m.Player1Id == currentUserId ? m.Player2.UserName : m.Player1.UserName,
                Result = m.WinnerId == currentUserId ? "Перемога" : (m.WinnerId == null ? "Нічия" : "Поразка"),
                m.PlayedAt
            })
            .ToListAsync();

        return Ok(history);
    }
}