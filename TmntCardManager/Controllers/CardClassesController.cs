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
using TmntCardManager.Services;

namespace TmntCardManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CardClassesController : Controller
    {
        private readonly TmntCardsDbContext _context;
        private readonly IDataPortServiceFactory<Cardclass> _portFactory;
        private readonly CardImportManager _importManager;

        public CardClassesController(TmntCardsDbContext context, IDataPortServiceFactory<Cardclass> portFactory, CardImportManager importManager)
        {
            _portFactory = portFactory;
            _importManager = importManager;
            _context = context;
        }

        // GET: CardClasses
        public async Task<IActionResult> Index()
        {
            return View(await _context.Cardclasses.ToListAsync());
        }

        // GET: CardClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cardclass = await _context.Cardclasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cardclass == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Cards", new { id = cardclass.Id, name = cardclass.Name });
        }

        // GET: CardClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CardClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Cardclass cardclass)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cardclass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cardclass);
        }

        // GET: CardClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cardclass = await _context.Cardclasses.FindAsync(id);
            if (cardclass == null)
            {
                return NotFound();
            }
            return View(cardclass);
        }

        // POST: CardClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Cardclass cardclass)
        {
            if (id != cardclass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cardclass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CardclassExists(cardclass.Id))
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
            return View(cardclass);
        }

        // GET: CardClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cardclass = await _context.Cardclasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cardclass == null)
            {
                return NotFound();
            }

            return View(cardclass);
        }

        // POST: CardClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cardclass = await _context.Cardclasses.FindAsync(id);
            if (cardclass != null)
            {
                _context.Cardclasses.Remove(cardclass);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CardclassExists(int id)
        {
            return _context.Cardclasses.Any(e => e.Id == id);
        }
        
        [HttpGet]
        public async Task<IActionResult> Export([FromQuery] string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", CancellationToken cancellationToken = default)
        {
            var exportService = _portFactory.GetExportService(contentType);
            var memoryStream = new MemoryStream();
            await exportService.WriteToAsync(memoryStream, cancellationToken);
            await memoryStream.FlushAsync(cancellationToken);
            memoryStream.Position = 0;

            return new FileStreamResult(memoryStream, contentType)
            {
                FileDownloadName = $"CardsCatalog_{DateTime.UtcNow:yyyyMMdd}.xlsx"
            };
        }

        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile? fileExcel, CancellationToken cancellationToken = default)
        {
            if (fileExcel == null || fileExcel.Length == 0) return RedirectToAction(nameof(Index));

            try
            {
                var importService = _portFactory.GetImportService(fileExcel.ContentType);
                using var stream = fileExcel.OpenReadStream();
                var rawClasses = await importService.ImportFromStreamAsync(stream, cancellationToken);
                
                var result = await _importManager.ProcessAndSaveAsync(rawClasses, cancellationToken);
                
                if (result.HasError) TempData["ErrorMessage"] = result.ErrorMessage;
                if (!string.IsNullOrEmpty(result.WarningMessage)) TempData["WarningMessage"] = result.WarningMessage;
                if (!string.IsNullOrEmpty(result.SuccessMessage)) TempData["SuccessMessage"] = result.SuccessMessage;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Критична помилка імпорту: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
