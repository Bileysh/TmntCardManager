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
    public class CardsController : Controller
    {
        private readonly TmntCardsDbContext _context;

        public CardsController(TmntCardsDbContext context)
        {
            _context = context;
        }

        // GET: Cards
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                ViewBag.CardclassName = "Всі (Загальний список)";
                var allCards = _context.Cards.Include(c => c.Class);
                return View(await allCards.ToListAsync());
            }

            ViewBag.CardclassId = id;
            var currentClass = await _context.Cardclasses.FindAsync(id);
            if (currentClass != null)
            {
                ViewBag.CardclassName = currentClass.Name;
            }
            var cardsByClass = _context.Cards
                .Where(c => c.Classid == id) 
                .Include(c => c.Class);      
            
            return View(await cardsByClass.ToListAsync());
        }

        // GET: Cards/Details/5
        public async Task<IActionResult> Details(int? id, int? factionId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var card = await _context.Cards
                .Include(c => c.Class)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (card == null)
            {
                return NotFound();
            }
            ViewBag.ReturnFactionId = factionId;
            return View(card);
        }

        // GET: Cards/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create(int id, int? factionId)
        {
            ViewBag.ReturnFactionId = factionId;
            ViewData["Classid"] = new SelectList(_context.Cardclasses, "Id", "Name");
            return View();
        }

        // POST: Cards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Imageurl,Name,Strength,Agility,Skill,Wit,Classid")] Card card)
        {
            var cardClass = _context.Cardclasses.FirstOrDefault(c => c.Id == card.Classid);

            if (cardClass != null)
            {
                card.Class = cardClass;
                ModelState.Clear();
                TryValidateModel(card);
            }
            if (ModelState.IsValid)
            {
                _context.Add(card);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "CardClasses", new { id = card.Classid });
            }

            ViewData["Classid"] = new SelectList(_context.Cardclasses, "Id", "Name", card.Classid);
            return View(card);
        }

        // GET: Cards/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id, int? factionId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var card = await _context.Cards.FindAsync(id);
            if (card == null)
            {
                return NotFound();
            }
            ViewBag.ReturnFactionId = factionId;
            ViewData["Classid"] = new SelectList(_context.Cardclasses, "Id", "Name");
            return View(card);
        }

        // POST: Cards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Imageurl,Name,Strength,Agility,Skill,Wit,Classid")] Card card)
        {
            if (id != card.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(card);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CardExists(card.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "CardClasses", new { id = card.Classid });
                
            }
            ViewData["Classid"] = new SelectList(_context.Cardclasses, "Id", "Name", card.Classid);
            return View(card);
        }

        // GET: Cards/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id, int? factionId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var card = await _context.Cards
                .Include(c => c.Class)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (card == null)
            {
                return NotFound();
            }
            ViewBag.ReturnFactionId = factionId;
            return View(card);
        }

        // POST: Cards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var card = await _context.Cards.FindAsync(id);
            if (card != null)
            {
                _context.Cards.Remove(card);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "CardClasses", new { id = card.Classid });
            
        }

        private bool CardExists(int id)
        {
            return _context.Cards.Any(e => e.Id == id);
        }
    }
}
