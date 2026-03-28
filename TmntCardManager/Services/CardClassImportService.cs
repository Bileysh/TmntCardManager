using ClosedXML.Excel;
using TmntCardManager.Models;

namespace TmntCardManager.Services
{
    public class CardClassImportService : IImportService<Cardclass>
    {
        public CardClassImportService() { }

        public async Task<List<Cardclass>> ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanRead) throw new ArgumentException("Дані не можуть бути прочитані", nameof(stream));
            
            var resultList = new List<Cardclass>();
            using var workBook = new XLWorkbook(stream);

            foreach (IXLWorksheet worksheet in workBook.Worksheets)
            {
                var fractionName = worksheet.Name;
                
                var fraction = new Cardclass 
                { 
                    Name = fractionName, 
                    Description = "Імпортовано з Excel",
                    Cards = new List<Card>() 
                };

                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    var cardName = row.Cell(1).Value.ToString();

                    var card = new Card
                    {
                        Name = cardName,
                        Strength = int.TryParse(row.Cell(2).Value.ToString(), out int str) ? str : 0,
                        Agility = int.TryParse(row.Cell(3).Value.ToString(), out int agi) ? agi : 0,
                        Skill = int.TryParse(row.Cell(4).Value.ToString(), out int skl) ? skl : 0,
                        Wit = int.TryParse(row.Cell(5).Value.ToString(), out int wit) ? wit : 0,
                        Imageurl = row.Cell(6).Value.ToString()
                    };

                    fraction.Cards.Add(card);
                }

                resultList.Add(fraction);
            }

            return await Task.FromResult(resultList);
        }
    }
}