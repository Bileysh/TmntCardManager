using TmntCardManager.Models;
using TmntCardManager.Models.Data;

namespace TmntCardManager.Services
{
    public class CardClassDataPortServiceFactory : IDataPortServiceFactory<Cardclass>
    {
        private readonly TmntCardsDbContext _context;

        public CardClassDataPortServiceFactory(TmntCardsDbContext context)
        {
            _context = context;
        }

        public IImportService<Cardclass> GetImportService(string contentType)
        {
            if (contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                return new CardClassImportService(_context);
            throw new NotImplementedException($"No import service implemented for {contentType}");
        }

        public IExportService<Cardclass> GetExportService(string contentType)
        {
            if (contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                return new CardClassExportService(_context);
            throw new NotImplementedException($"No export service implemented for {contentType}");
        }
    }
}