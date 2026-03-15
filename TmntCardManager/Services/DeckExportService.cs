using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using TmntCardManager.Models.Data; 

namespace TmntCardManager.Services
{
    public class DeckExportService
    {
        private readonly TmntCardsDbContext _context;

        public DeckExportService(TmntCardsDbContext context)
        {
            _context = context;
        }

        public async Task ExportDeckAsync(Stream stream, int deckId, CancellationToken cancellationToken)
        {
            var deck = await _context.Decks
                .Include(d => d.Deckcards)!
                    .ThenInclude(dc => dc.Card)
                    .ThenInclude(c => c.Class) 
                .FirstOrDefaultAsync(d => d.Id == deckId, cancellationToken);

            if (deck == null) throw new Exception("Колоду не знайдено");

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add($"Колода {deck.Name}");

            worksheet.Cell(1, 1).Value = "Назва карти";
            worksheet.Cell(1, 2).Value = "Фракція";
            worksheet.Cell(1, 3).Value = "Кількість";
            worksheet.Cell(1, 4).Value = "Сила";
            worksheet.Cell(1, 5).Value = "Спритність";
            worksheet.Cell(1, 6).Value = "Майстерність";
            worksheet.Cell(1, 7).Value = "Кмітливість";
            worksheet.Row(1).Style.Font.Bold = true;

            int rowIndex = 2;
            if (deck.Deckcards != null)
                foreach (var item in deck.Deckcards)
                {
                    worksheet.Cell(rowIndex, 1).Value = item.Card?.Name;
                    worksheet.Cell(rowIndex, 2).Value = item.Card?.Class?.Name;
                    worksheet.Cell(rowIndex, 3).Value = item.Quantity;
                    worksheet.Cell(rowIndex, 4).Value = item.Card?.Strength ?? 0;
                    worksheet.Cell(rowIndex, 5).Value = item.Card?.Agility ?? 0;
                    worksheet.Cell(rowIndex, 6).Value = item.Card?.Skill ?? 0;
                    worksheet.Cell(rowIndex, 7).Value = item.Card?.Wit ?? 0;
                    rowIndex++;
                }

            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(stream);
        }
    }
}