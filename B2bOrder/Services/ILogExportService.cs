using B2bOrder.Data;
using B2bOrder.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace B2bOrder.Services
{
    public interface ILogExportService
    {
        Task<IEnumerable<LogExport>> GetAllAsync();
        Task<LogExport?> GetByIdAsync(int nid);
        Task CreateAsync(LogExport entity);
        Task UpdateAsync(LogExport entity);
        Task DeleteAsync(int nid);
    }
    public class LogExportService : BaseService, ILogExportService
    {
        private readonly ApplicationDbContext _db;
        public LogExportService(IServiceProvider serviceProvider, ApplicationDbContext db) : base(serviceProvider)
        {
            _db = db;
        }

        public async Task<IEnumerable<LogExport>> GetAllAsync() => await _db.LogExports.AsNoTracking().ToListAsync();

        public async Task<LogExport?> GetByIdAsync(int nid) => await _db.LogExports.FindAsync(nid);

        public async Task CreateAsync(LogExport entity)
        {
            await _db.LogExports.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(LogExport entity)
        {
            _db.LogExports.Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int nid)
        {
            var e = await _db.LogExports.FindAsync(nid);
            if (e is null) return;
            _db.LogExports.Remove(e);
            await _db.SaveChangesAsync();
        }
    }
}