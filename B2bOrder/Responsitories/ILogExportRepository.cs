using Microsoft.EntityFrameworkCore;
using B2bOrder.Data;
using B2bOrder.Models.Db;

namespace B2bOrder.Responsitories
{
    public interface ILogExportRepository : IRepository<LogExport> { }

    public class LogExportRepository : ILogExportRepository
    {
        private readonly ApplicationDbContext _db;
        public LogExportRepository(ApplicationDbContext db) => _db = db;

        public async Task AddAsync(LogExport entity) => await _db.Set<LogExport>().AddAsync(entity);

        public async Task DeleteAsync(LogExport entity)
        {
            _db.Set<LogExport>().Remove(entity);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<LogExport>> GetAllAsync()
            => await _db.Set<LogExport>().AsNoTracking().ToListAsync();

        public async Task<LogExport?> GetByIdAsync(object id)
        {
            if (id is int nid) return await _db.Set<LogExport>().FindAsync(nid);
            return null;
        }

        public async Task UpdateAsync(LogExport entity)
        {
            _db.Set<LogExport>().Update(entity);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}