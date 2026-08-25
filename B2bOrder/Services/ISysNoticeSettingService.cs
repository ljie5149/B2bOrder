using B2bOrder.Data;
using B2bOrder.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace B2bOrder.Services
{
    public interface ISysNoticeSettingService
    {
        Task<IEnumerable<SysNoticeSetting>> GetAllAsync();
        Task<SysNoticeSetting?> GetByIdAsync(int nid);
        Task CreateAsync(SysNoticeSetting entity);
        Task UpdateAsync(SysNoticeSetting entity);
        Task DeleteAsync(int nid);
    }
    public class SysNoticeSettingService : BaseService, ISysNoticeSettingService
    {
        private readonly ApplicationDbContext _db;
        public SysNoticeSettingService(IServiceProvider serviceProvider, ApplicationDbContext db) : base(serviceProvider)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<IEnumerable<SysNoticeSetting>> GetAllAsync() => await _db.SysNoticeSettings.AsNoTracking().ToListAsync();

        public async Task<SysNoticeSetting?> GetByIdAsync(int nid) => await _db.SysNoticeSettings.FindAsync(nid);

        public async Task CreateAsync(SysNoticeSetting entity)
        {
            await _db.SysNoticeSettings.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(SysNoticeSetting entity)
        {
            _db.SysNoticeSettings.Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int nid)
        {
            var e = await _db.SysNoticeSettings.FindAsync(nid);
            if (e is null) return;
            _db.SysNoticeSettings.Remove(e);
            await _db.SaveChangesAsync();
        }
    }
}