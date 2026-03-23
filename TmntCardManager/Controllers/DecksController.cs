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
    public class DecksController : Controller
    {
        private readonly TmntCardsDbContext _context;
        private readonly UserManager<User> _userManager;
        
        public DecksController(TmntCardsDbContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Decks
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var myDecks = _context.Decks
                .Include(d => d.User)
                .Where(d => d.Userid == currentUser.Id);
            
            return View(await myDecks.ToListAsync());        }

        // GET: Decks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deck = await _context.Decks
                .Include(d => d.User)
                .Include(d => d.Deckcards)!
                    .ThenInclude(dc => dc.Card)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deck == null)
            {
                return NotFound();
            }

            return View(deck);
        }

        // GET: Decks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Decks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Createdat")] Deck deck)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                deck.Userid = currentUser.Id; 

                _context.Add(deck);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(deck);
        }

        // GET: Decks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deck = await _context.Decks.FindAsync(id);
            if (deck == null)
            {
                return NotFound();
            }
            ViewData["Userid"] = new SelectList(_context.Users, "Id", "Id", deck.Userid);
            return View(deck);
        }

        // POST: Decks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Createdat,Userid")] Deck deck)
        {
            if (id != deck.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deck);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeckExists(deck.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Userid"] = new SelectList(_context.Users, "Id", "Id", deck.Userid);
            return View(deck);
        }

        // GET: Decks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deck = await _context.Decks
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deck == null)
            {
                return NotFound();
            }

            return View(deck);
        }

        // POST: Decks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deck = await _context.Decks.FindAsync(id);
            if (deck != null)
            {
                _context.Decks.Remove(deck);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeckExists(int id)
        {
            return _context.Decks.Any(e => e.Id == id);
        }
        [HttpGet]
        public async Task<IActionResult> Export(int id, [FromServices] TmntCardManager.Services.DeckExportService exportService, CancellationToken cancellationToken)
        {
            var deck = await _context.Decks.FindAsync(id);
            if (deck == null) return NotFound();

            var memoryStream = new MemoryStream();
    
            await exportService.ExportDeckAsync(memoryStream, id, cancellationToken);
    
            await memoryStream.FlushAsync(cancellationToken);
            memoryStream.Position = 0;

            return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = $"Deck_{deck.Name}_{DateTime.UtcNow:yyyyMMdd}.xlsx"
            };
        }
    }
}
