using Microsoft.EntityFrameworkCore;
using B2bOrder.Data;
using B2bOrder.Models.Db;

namespace B2bOrder.Responsitories
{
    public interface ILogPushRepository : IRepository<LogPush>
    {
        Task<IEnumerable<LogPush>> GetByMemberSidAsync(string memberSid);
    }

    public class LogPushRepository : ILogPushRepository
    {
        private readonly ApplicationDbContext _db;
        public LogPushRepository(ApplicationDbContext db) => _db = db;

        public async Task AddAsync(LogPush entity) => await _db.Set<LogPush>().AddAsync(entity);

        public async Task DeleteAsync(LogPush entity)
        {
            _db.Set<LogPush>().Remove(entity);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<LogPush>> GetAllAsync()
            => await _db.Set<LogPush>().AsNoTracking().ToListAsync();

        public async Task<LogPush?> GetByIdAsync(object id)
        {
            if (id is int nid) return await _db.Set<LogPush>().FindAsync(nid);
            return null;
        }

        public async Task<IEnumerable<LogPush>> GetByMemberSidAsync(string memberSid)
            => string.IsNullOrEmpty(memberSid) ? Array.Empty<LogPush>() : await _db.Set<LogPush>().AsNoTracking().Where(l => l.MemberSid == memberSid).ToListAsync();

        public async Task UpdateAsync(LogPush entity)
        {
            _db.Set<LogPush>().Update(entity);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}