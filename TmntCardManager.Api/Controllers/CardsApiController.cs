using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmntCardManager.Api.DTOs;
using TmntCardManager.Models.Data; 
using TmntCardManager.Models;

namespace TmntCardManager.Api.Controllers
{
    [Authorize] 
    [Route("api/[controller]")]
    [ApiController]
    public class CardsApiController : ControllerBase
    {
        private readonly TmntCardsDbContext _context;

        public CardsApiController(TmntCardsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CardOutputDto>>> GetCards()
        {
            var cards = await _context.Cards
                .Include(c => c.Class)
                .Select(c => new CardOutputDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ImageUrl = c.Imageurl,
                    Strength = c.Strength,
                    Agility = c.Agility,
                    Skill = c.Skill,
                    Wit = c.Wit,
                    ClassId = c.Classid,
                    ClassName = c.Class.Name
                })
                .ToListAsync();
            
            return Ok(cards);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CardOutputDto>> GetCard(int id)
        {
            var card = await _context.Cards
                .Include(c => c.Class)
                .Where(c => c.Id == id)
                .Select(c => new CardOutputDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ImageUrl = c.Imageurl,
                    Strength = c.Strength,
                    Agility = c.Agility,
                    Skill = c.Skill,
                    Wit = c.Wit,
                    ClassId = c.Classid,
                    ClassName = c.Class.Name
                })
                .FirstOrDefaultAsync();

            if (card == null) return NotFound(new { message = $"Картку з ID {id} не знайдено." });

            return Ok(card);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCard([FromBody] CardInputDto dto)
        {
            var classExists = await _context.Cardclasses.AnyAsync(c => c.Id == dto.ClassId);
            if (!classExists) return BadRequest(new { message = "Вказаної фракції не існує." });

            var newCard = new Card
            {
                Name = dto.Name,
                Imageurl = dto.ImageUrl,
                Strength = dto.Strength,
                Agility = dto.Agility,
                Skill = dto.Skill,
                Wit = dto.Wit,
                Classid = dto.ClassId
            };

            _context.Cards.Add(newCard);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Картку успішно створено", id = newCard.Id });
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCard(int id, [FromBody] CardInputDto dto)
        {
            var card = await _context.Cards.FindAsync(id);
            if (card == null) return NotFound(new { message = $"Картку з ID {id} не знайдено." });

            var classExists = await _context.Cardclasses.AnyAsync(c => c.Id == dto.ClassId);
            if (!classExists) return BadRequest(new { message = "Вказаної фракції не існує." });

            card.Name = dto.Name;
            card.Imageurl = dto.ImageUrl;
            card.Strength = dto.Strength;
            card.Agility = dto.Agility;
            card.Skill = dto.Skill;
            card.Wit = dto.Wit;
            card.Classid = dto.ClassId;

            await _context.SaveChangesAsync();

            return NoContent(); 
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCard(int id)
        {
            var card = await _context.Cards.FindAsync(id);
            if (card == null)
            {
                return NotFound(new { message = $"Картку з ID {id} не знайдено." });
            }

            _context.Cards.Remove(card);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Картку '{card.Name}' успішно видалено." });
        }
    }
}