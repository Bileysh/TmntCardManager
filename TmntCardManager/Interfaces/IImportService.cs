
using TmntCardManager.Models;

namespace TmntCardManager.Services
{
    public interface IImportService<TEntity> where TEntity : class
    {
        Task<List<Cardclass>> ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken);
    }
}