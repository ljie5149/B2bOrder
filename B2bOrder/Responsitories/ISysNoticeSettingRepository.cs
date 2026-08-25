using Microsoft.EntityFrameworkCore;
using B2bOrder.Data;
using B2bOrder.Models.Db;

namespace B2bOrder.Responsitories
{
    public interface ISysNoticeSettingRepository : IRepository<SysNoticeSetting> { }

    public class SysNoticeSettingRepository : ISysNoticeSettingRepository
    {
        private readonly ApplicationDbContext _db;
        public SysNoticeSettingRepository(ApplicationDbContext db) => _db = db;

        public async Task AddAsync(SysNoticeSetting entity) => await _db.Set<SysNoticeSetting>().AddAsync(entity);

        public async Task DeleteAsync(SysNoticeSetting entity)
        {
            _db.Set<SysNoticeSetting>().Remove(entity);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<SysNoticeSetting>> GetAllAsync()
            => await _db.Set<SysNoticeSetting>().AsNoTracking().ToListAsync();

        public async Task<SysNoticeSetting?> GetByIdAsync(object id)
        {
            if (id is int nid) return await _db.Set<SysNoticeSetting>().FindAsync(nid);
            return null;
        }

        public async Task UpdateAsync(SysNoticeSetting entity)
        {
            _db.Set<SysNoticeSetting>().Update(entity);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}