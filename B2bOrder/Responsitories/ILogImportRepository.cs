using Microsoft.EntityFrameworkCore;
using B2bOrder.Data;
using B2bOrder.Models.Db;

namespace B2bOrder.Responsitories
{
    public interface ILogImportRepository : IRepository<LogImport> { }

    public class LogImportRepository : ILogImportRepository
    {
        private readonly ApplicationDbContext _db;
        public LogImportRepository(ApplicationDbContext db) => _db = db;

        public async Task AddAsync(LogImport entity) => await _db.Set<LogImport>().AddAsync(entity);

        public async Task DeleteAsync(LogImport entity)
        {
            _db.Set<LogImport>().Remove(entity);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<LogImport>> GetAllAsync()
            => await _db.Set<LogImport>().AsNoTracking().ToListAsync();

        public async Task<LogImport?> GetByIdAsync(object id)
        {
            if (id is int nid) return await _db.Set<LogImport>().FindAsync(nid);
            return null;
        }

        public async Task UpdateAsync(LogImport entity)
        {
            _db.Set<LogImport>().Update(entity);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}