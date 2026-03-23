using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TmntCardManager.Models;
using TmntCardManager.Models.Data;

namespace TmntCardManager.Controllers
{
    [Authorize]
    public class DeckcardsController : Controller
    {
        private readonly TmntCardsDbContext _context;

        public DeckcardsController(TmntCardsDbContext context)
        {
            _context = context;
        }

        // GET: Deckcards
        public async Task<IActionResult> Index()
        {
            var tmntCardsDbContext = _context.Deckcards.Include(d => d.Card).Include(d => d.Deck);
            return View(await tmntCardsDbContext.ToListAsync());
        }

        // GET: Deckcards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deckcard = await _context.Deckcards
                .Include(d => d.Card)
                .Include(d => d.Deck)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deckcard == null)
            {
                return NotFound();
            }

            return View(deckcard);
        }

        // GET: Deckcards/Create
        public IActionResult Create(int? deckId)
        {
            ViewData["Cardid"] = new SelectList(_context.Cards, "Id", "Name");
            ViewData["Deckid"] = new SelectList(_context.Decks, "Id", "Name", deckId);
            return View();
        }

        // POST: Deckcards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Deckid,Quantity,Cardid")] Deckcard deckcard)
        {
            if (ModelState.IsValid)
            {
                var existingCard = await _context.Deckcards
                    .FirstOrDefaultAsync(dc => dc.Deckid == deckcard.Deckid && dc.Cardid == deckcard.Cardid);

                if (existingCard != null)
                {
                    existingCard.Quantity += 1;
                    _context.Update(existingCard);
                }
                else
                {
                    deckcard.Quantity = 1;
                    _context.Add(deckcard);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Decks", new { id = deckcard.Deckid });
            }
            ViewData["Cardid"] = new SelectList(_context.Cards, "Id", "Name", deckcard.Cardid);
            ViewData["Deckid"] = new SelectList(_context.Decks, "Id", "Name", deckcard.Deckid);
            return View(deckcard);
        }

        // GET: Deckcards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deckcard = await _context.Deckcards.FindAsync(id);
            if (deckcard == null)
            {
                return NotFound();
            }
            ViewData["Cardid"] = new SelectList(_context.Cards, "Id", "Name", deckcard.Cardid);
            ViewData["Deckid"] = new SelectList(_context.Decks, "Id", "Id", deckcard.Deckid);
            return View(deckcard);
        }

        // POST: Deckcards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Deckid,Quantity,Cardid")] Deckcard deckcard)
        {
            if (id != deckcard.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deckcard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeckcardExists(deckcard.Id))
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
            ViewData["Cardid"] = new SelectList(_context.Cards, "Id", "Name", deckcard.Cardid);
            ViewData["Deckid"] = new SelectList(_context.Decks, "Id", "Id", deckcard.Deckid);
            return View(deckcard);
        }

        // GET: Deckcards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deckcard = await _context.Deckcards
                .Include(d => d.Card)
                .Include(d => d.Deck)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deckcard == null)
            {
                return NotFound();
            }

            return View(deckcard);
        }

        // POST: Deckcards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deckcard = await _context.Deckcards.FindAsync(id);
            if (deckcard != null)
            {
                var deckId = deckcard.Deckid; 

                if (deckcard.Quantity > 1)
                {
                    deckcard.Quantity -= 1;
                    _context.Update(deckcard);
                }
                else
                {
                    _context.Deckcards.Remove(deckcard);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Decks", new { id = deckId });
            }
    
            return RedirectToAction("Index", "Decks");
        }

        private bool DeckcardExists(int id)
        {
            return _context.Deckcards.Any(e => e.Id == id);
        }
    }
}
