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
public class ShopApiController : ControllerBase
{
    private readonly TmntCardsDbContext _context;
    private readonly UserManager<User> _userManager;

    public ShopApiController(TmntCardsDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShopItemOutputDto>>> GetShopItems()
    {
        var items = await _context.ShopItems
            .Select(i => new ShopItemOutputDto
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                PriceCoins = i.PriceCoins,
                ImageUrl = i.ImageUrl
            })
            .ToListAsync();

        return Ok(items);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateShopItem([FromBody] ShopItemInputDto dto)
    {
        var newItem = new ShopItem
        {
            Name = dto.Name,
            Description = dto.Description,
            PriceCoins = dto.PriceCoins,
            ImageUrl = dto.ImageUrl
        };

        _context.ShopItems.Add(newItem);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Товар успішно додано до магазину!", id = newItem.Id });
    }
    
    [HttpPost("buy")]
    public async Task<IActionResult> BuyItem([FromBody] BuyItemInputDto dto)
    {
        var userId = int.Parse(_userManager.GetUserId(User)!);

        var item = await _context.ShopItems.FindAsync(dto.ShopItemId);
        if (item == null) return NotFound(new { message = "Товар не знайдено." });

        var profile = await _context.Playerprofiles.FindAsync(userId);
        if (profile == null) return NotFound(new { message = "Профіль гравця не знайдено." });

        if (profile.BalanceCoins < item.PriceCoins)
            return BadRequest(new { message = "Недостатньо монет для покупки!" });

        profile.BalanceCoins -= item.PriceCoins;

        var transaction = new Transaction
        {
            UserId = userId,
            ShopItemId = item.Id,
            SpentCoins = item.PriceCoins,
            PurchasedAt = DateTime.UtcNow
        };
        _context.Transactions.Add(transaction);
        
        var randomCards = await _context.Cards
            .OrderBy(c => Guid.NewGuid())
            .Take(3)
            .ToListAsync();

        var receivedCardNames = new List<string>();

        foreach (var card in randomCards)
        {
            receivedCardNames.Add(card.Name);

            var existingUserCard = await _context.Usercards
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CardId == card.Id);

            if (existingUserCard != null)
            {
                existingUserCard.Quantity += 1; 
            }
            else
            {
                _context.Usercards.Add(new UserCard 
                {
                    UserId = userId,
                    CardId = card.Id,
                    Quantity = 1
                });
            }
        }

        await _context.SaveChangesAsync();

        var result = new BuyResultDto
        {
            Message = $"Ви успішно придбали '{item.Name}'!",
            NewBalance = profile.BalanceCoins,
            ReceivedCards = receivedCardNames
        };

        return Ok(result);
    }
}