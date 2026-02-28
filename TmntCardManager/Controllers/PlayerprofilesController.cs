using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TmntCardManager.Models;
using TmntCardManager.Models.Data;

namespace TmntCardManager.Controllers
{
    public class PlayerprofilesController : Controller
    {
        private readonly TmntCardsDbContext _context;

        public PlayerprofilesController(TmntCardsDbContext context)
        {
            _context = context;
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
    }
}
