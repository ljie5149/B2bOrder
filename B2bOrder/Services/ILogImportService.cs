using B2bOrder.Data;
using B2bOrder.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace B2bOrder.Services
{
    public interface ILogImportService
    {
        Task<IEnumerable<LogImport>> GetAllAsync();
        Task<LogImport?> GetByIdAsync(int nid);
        Task CreateAsync(LogImport entity);
        Task UpdateAsync(LogImport entity);
        Task DeleteAsync(int nid);
    }
    public class LogImportService : BaseService, ILogImportService
    {
        private readonly ApplicationDbContext _db;
        public LogImportService(IServiceProvider serviceProvider, ApplicationDbContext db) : base(serviceProvider)
        {
            _db = db;
        }

        public async Task<IEnumerable<LogImport>> GetAllAsync() => await _db.LogImports.AsNoTracking().ToListAsync();

        public async Task<LogImport?> GetByIdAsync(int nid) => await _db.LogImports.FindAsync(nid);

        public async Task CreateAsync(LogImport entity)
        {
            await _db.LogImports.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(LogImport entity)
        {
            _db.LogImports.Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int nid)
        {
            var e = await _db.LogImports.FindAsync(nid);
            if (e is null) return;
            _db.LogImports.Remove(e);
            await _db.SaveChangesAsync();
        }
    }
}