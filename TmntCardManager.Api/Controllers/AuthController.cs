using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmntCardManager.Api.DTOs;
using TmntCardManager.Models;
using TmntCardManager.Models.Data;

namespace TmntCardManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly TmntCardsDbContext _context;

        public AuthController(SignInManager<User> signInManager, UserManager<User> userManager, TmntCardsDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }
        
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new User { UserName = request.Email, Email = request.Email };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                var random = new Random();
                string newnickname = "";
                while (true)
                {
                    newnickname = "Player" + random.Next(1000, 9999);
            
                    bool nicknameExists = await _context.Playerprofiles.AnyAsync(p => p.Nickname == newnickname);
            
                    if (!nicknameExists)
                    {
                        break; 
                    }
                }
                var profile = new Playerprofile
                {
                    Id = user.Id,
                    Nickname = newnickname,
                    Winrate = 0.0,
                    BalanceCoins = 0, 
                    Experience = 0,
                    Avatarurl = "default_avatar.png"
                };

                _context.Playerprofiles.Add(profile);
                
                var starterCards = await _context.Cards.Take(5).ToListAsync();
            
                foreach (var card in starterCards)
                {
                    var userCard = new UserCard
                    {
                        UserId = user.Id,
                        CardId = card.Id,
                        Quantity = 1
                    };
                    _context.Usercards.Add(userCard);
                }
                
                await _context.SaveChangesAsync();

                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok(new { message = "Реєстрація успішна! Профіль та 5 стартових карт додано." });
                
            }

            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { message = "Помилка реєстрації", errors });
        }
        
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);

            if (result.Succeeded)
            {
                return Ok(new { message = "Успішний вхід! Cookie встановлено." });
            }

            return Unauthorized(new { message = "Неправильний логін або пароль." });
        }
        
        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Ви успішно вийшли з системи. Cookie видалено." });
        }
    }
}