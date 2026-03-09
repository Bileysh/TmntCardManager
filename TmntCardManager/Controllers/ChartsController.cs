using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmntCardManager.Models;
using TmntCardManager.Models.Data; 

namespace TmntCardManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private readonly TmntCardsDbContext _context; 

        public ChartsController(TmntCardsDbContext context)
        {
            _context = context;
        }

        private record ChartItem(string Name, int Count);

        [HttpGet("deckStats/{deckId}")]
        public async Task<JsonResult> GetDeckStats(int deckId, CancellationToken cancellationToken)
        {
            var deckCards = await _context.Deckcards
                .Include(dc => dc.Card)
                .Where(dc => dc.Deckid == deckId)
                .ToListAsync(cancellationToken);

            var totalStrength = deckCards.Sum(dc => (dc.Card.Strength ?? 0) * dc.Quantity);
            var totalAgility = deckCards.Sum(dc => (dc.Card.Agility ?? 0) * dc.Quantity);
            var totalSkill = deckCards.Sum(dc => (dc.Card.Skill ?? 0) * dc.Quantity);
            var totalWit = deckCards.Sum(dc => (dc.Card.Wit ?? 0) * dc.Quantity);

            var data = new List<ChartItem>
            {
                new ChartItem("Сила", totalStrength),
                new ChartItem("Спритність", totalAgility),
                new ChartItem("Майстерність", totalSkill),
                new ChartItem("Кмітливість", totalWit)
            };

            return new JsonResult(data);
        }

        [HttpGet("cardsByFraction")]
        public async Task<JsonResult> GetCardsByFraction(CancellationToken cancellationToken)
        {
            var data = await _context.Cards
                .Include(c => c.Class) 
                .GroupBy(c => c.Class.Name)
                .Select(g => new ChartItem(g.Key, g.Count()))
                .ToListAsync(cancellationToken);

            return new JsonResult(data);
        }
    }
}