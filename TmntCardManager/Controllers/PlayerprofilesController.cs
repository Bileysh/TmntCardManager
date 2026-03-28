using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TmntCardManager.Models;
using TmntCardManager.Models.Data;

namespace TmntCardManager.Controllers
{
    [Authorize]
    public class PlayerprofilesController : Controller
    {
        private readonly TmntCardsDbContext _context;
        private readonly UserManager<User> _userManager;
        
        public PlayerprofilesController(TmntCardsDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Playerprofiles
        public async Task<IActionResult> Index()
        {
            var tmntCardsDbContext = _context.Playerprofiles.Include(p => p.IdNavigation);
            return View(await tmntCardsDbContext.ToListAsync());
        }

        // GET: Playerprofiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playerprofile = await _context.Playerprofiles
                .Include(p => p.IdNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (playerprofile == null)
            {
                return NotFound();
            }

            return View(playerprofile);
        }
        

        // GET: Playerprofiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playerprofile = await _context.Playerprofiles.FindAsync(id);
            if (playerprofile == null)
            {
                return NotFound();
            }
            ViewData["Id"] = new SelectList(_context.Users, "Id", "Id", playerprofile.Id);
            return View(playerprofile);
        }

        // POST: Playerprofiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nickname,Winrate,Avatarurl")] Playerprofile playerprofile)
        {
            if (id != playerprofile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(playerprofile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerprofileExists(playerprofile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Users", new { id = playerprofile.Id });            }
            
            ViewData["Id"] = new SelectList(_context.Users, "Id", "Id", playerprofile.Id);
            return View(playerprofile);
        }

        private bool PlayerprofileExists(int id)
        {
            return _context.Playerprofiles.Any(e => e.Id == id);
        }
        
        // GET: PlayerProfiles/MyProfile
        public async Task<IActionResult> MyProfile()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Redirect("/Identity/Account/Login");

            ViewBag.Email = currentUser.Email;

            var profile = await _context.Playerprofiles
                .FirstOrDefaultAsync(p => p.Id == currentUser.Id);

            var oldDefaultAvatar = "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_960_720.png";

            if (profile == null)
            {
                string newnickname = "";
                var random = new Random();
        
                while (true)
                {
                    newnickname = "Player" + random.Next(1000, 9999);
                    bool nicknameExists = await _context.Playerprofiles.AnyAsync(p => p.Nickname == newnickname);
            
                    if (!nicknameExists)
                    {
                        break; 
                    }
                }

                profile = new Playerprofile 
                { 
                    Id = currentUser.Id, 
                    Nickname = newnickname, 
                    Avatarurl = "", 
                    Winrate = 0
                };
                _context.Playerprofiles.Add(profile);
                await _context.SaveChangesAsync();
            }
            else if (profile.Avatarurl == oldDefaultAvatar)
            {
                profile.Avatarurl = "";
                _context.Playerprofiles.Update(profile);
                await _context.SaveChangesAsync();
            }

            return View(profile); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(int Id, string Nickname, string AvatarUrl)
        {
            var currentUser = await _userManager.GetUserAsync(User);
    
            if (Id != currentUser.Id) return Unauthorized();

            var profile = await _context.Playerprofiles.FindAsync(Id);
            if (profile != null)
            {
                profile.Nickname = Nickname;
        
                profile.Avatarurl = string.IsNullOrWhiteSpace(AvatarUrl) ? "" : AvatarUrl;
        
                _context.Update(profile);
                await _context.SaveChangesAsync();
            }
    
            return RedirectToAction(nameof(MyProfile)); 
        }
    }
}   
