using Microsoft.EntityFrameworkCore;
using B2bOrder.Data;
using B2bOrder.Models.Db;

namespace B2bOrder.Responsitories
{
    public interface ILogActionRepository : IRepository<LogAction>
    {
        Task<IEnumerable<LogAction>> GetByUserSidAsync(string userSid);
        Task<IEnumerable<LogAction>> GetByModuleAsync(string moduleName);
        Task<IEnumerable<LogAction>> GetByActionTypeAsync(string actionType);
        Task<IEnumerable<LogAction>> GetByDateRangeAsync(
            DateTime startDate,
            DateTime endDate);
    }

    public class LogActionRepository : ILogActionRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly DbSet<LogAction> _table;

        public LogActionRepository(ApplicationDbContext db)
        {
            _db = db;
            _table = _db.Set<LogAction>();
        }

        public async Task AddAsync(LogAction entity)
        {
            await _table.AddAsync(entity);
        }

        public async Task DeleteAsync(LogAction entity)
        {
            _table.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<LogAction>> GetAllAsync()
        {
            return await _table
                .AsNoTracking()
                .OrderByDescending(x => x.CreateDate)
                .ToListAsync();
        }

        public async Task<LogAction?> GetByIdAsync(object id)
        {
            if (id is not int nid)
                return null;

            return await _table.FindAsync(nid);
        }

        public async Task<IEnumerable<LogAction>> GetByUserSidAsync(string userSid)
        {
            return await _table
                .AsNoTracking()
                .Where(x => x.UserSid == userSid)
                .OrderByDescending(x => x.CreateDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<LogAction>> GetByModuleAsync(string moduleName)
        {
            return await _table
                .AsNoTracking()
                .Where(x => x.ModuleName == moduleName)
                .OrderByDescending(x => x.CreateDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<LogAction>> GetByActionTypeAsync(string actionType)
        {
            return await _table
                .AsNoTracking()
                .Where(x => x.ActionType == actionType)
                .OrderByDescending(x => x.CreateDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<LogAction>> GetByDateRangeAsync(
            DateTime startDate,
            DateTime endDate)
        {
            return await _table
                .AsNoTracking()
                .Where(x =>
                    x.CreateDate >= startDate &&
                    x.CreateDate <= endDate)
                .OrderByDescending(x => x.CreateDate)
                .ToListAsync();
        }

        public async Task UpdateAsync(LogAction entity)
        {
            _table.Update(entity);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}