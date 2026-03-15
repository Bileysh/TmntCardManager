using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using TmntCardManager.Models;
using TmntCardManager.Models.Data;

namespace TmntCardManager.Services
{
    public class CardClassImportService : IImportService<Cardclass>
    {
        private readonly TmntCardsDbContext _context;

        public CardClassImportService(TmntCardsDbContext context)
        {
            _context = context;
        }

        public async Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanRead) throw new ArgumentException("Дані не можуть бути прочитані", nameof(stream));

            using var workBook = new XLWorkbook(stream);

            foreach (IXLWorksheet worksheet in workBook.Worksheets)
            {
                var fractionName = worksheet.Name;
                
                var fraction = await _context.Cardclasses
                    .FirstOrDefaultAsync(c => c.Name == fractionName, cancellationToken);
                if (fraction == null)
                {
                    fraction = new Cardclass { Name = fractionName, Description = "Імпортовано з Excel" };
                    _context.Cardclasses.Add(fraction);
                    await _context.SaveChangesAsync(cancellationToken); 
                }

                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    var cardName = row.Cell(1).Value.ToString();
                    if (string.IsNullOrWhiteSpace(cardName)) continue;

                    var card = await _context.Cards
                        .FirstOrDefaultAsync(c => c.Name == cardName, cancellationToken);
                    if (card == null)
                    {
                        card = new Card();
                        _context.Cards.Add(card);
                    }

                    card.Name = cardName;
                    card.Strength = int.TryParse(row.Cell(2).Value.ToString(), out int str) ? str : 0;
                    card.Agility = int.TryParse(row.Cell(3).Value.ToString(), out int agi) ? agi : 0;
                    card.Skill = int.TryParse(row.Cell(4).Value.ToString(), out int skl) ? skl : 0;
                    card.Wit = int.TryParse(row.Cell(5).Value.ToString(), out int wit) ? wit : 0;
                    card.Imageurl = row.Cell(6).Value.ToString();
                    card.Classid = fraction.Id;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}