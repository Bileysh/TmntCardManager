using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using TmntCardManager.Models;
using TmntCardManager.Models.Data;

namespace TmntCardManager.Services;

public class CardClassExportService: IExportService<Cardclass>
{
    private readonly TmntCardsDbContext _context;
    
    public CardClassExportService(TmntCardsDbContext context)
    {
        _context = context;
    }

    public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
    {
        if (!stream.CanWrite) throw new ArgumentException("Input stream is not writable");

        var cardClasses = await _context.Cardclasses
            .Include(c => c.Cards)
            .ToListAsync(cancellationToken);
        using var workbook = new XLWorkbook();

        foreach (var cardClass in cardClasses)
        {
            var worksheet = workbook.Worksheets.Add(cardClass.Name);

            worksheet.Cell(1, 1).Value = "Назва карти";
            worksheet.Cell(1, 2).Value = "Сила";
            worksheet.Cell(1, 3).Value = "Спритність";
            worksheet.Cell(1, 4).Value = "Майстерність";
            worksheet.Cell(1, 5).Value = "Кмітливість";
            worksheet.Cell(1, 6).Value = "URL Зображення";
            worksheet.Row(1).Style.Font.Bold = true;
            
            int rowIndex = 2;
            foreach (var card in cardClass.Cards)
            {
                worksheet.Cell(rowIndex, 1).Value = card.Name;
                worksheet.Cell(rowIndex, 2).Value = card.Strength ?? 0;
                worksheet.Cell(rowIndex, 3).Value = card.Agility ?? 0;
                worksheet.Cell(rowIndex, 4).Value = card.Skill ?? 0;
                worksheet.Cell(rowIndex, 5).Value = card.Wit ?? 0;
                worksheet.Cell(rowIndex, 6).Value = card.Imageurl;
                rowIndex++;
            }
            worksheet.Columns().AdjustToContents();
        }
        workbook.SaveAs(stream);
    }
}