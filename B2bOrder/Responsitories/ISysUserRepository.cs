using Microsoft.EntityFrameworkCore;
using B2bOrder.Data;
using B2bOrder.Models.Db;

namespace B2bOrder.Responsitories
{
    public interface ISysUserRepository : IRepository<SysUser>
    {
        Task<SysUser?> GetBySidAsync(string sid);
        Task<SysUser?> GetByAccountAsync(string account);
        Task<SysUser?> GetByMemberSidAsync(string memberSid);

        // 新增：透過重設密碼 Token 查詢使用者
        Task<SysUser?> GetByResetTokenAsync(string token);
    }

    public class SysUserRepository : ISysUserRepository
    {
        private readonly ApplicationDbContext _db;
        public SysUserRepository(ApplicationDbContext db) => _db = db;

        public async Task AddAsync(SysUser entity) => await _db.Set<SysUser>().AddAsync(entity);

        public async Task DeleteAsync(SysUser entity)
        {
            _db.Set<SysUser>().Remove(entity);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<SysUser>> GetAllAsync()
            => await _db.Set<SysUser>().AsNoTracking().ToListAsync();

        public async Task<SysUser?> GetByIdAsync(object id)
        {
            if (id is int nid) return await _db.Set<SysUser>().FindAsync(nid);
            return null;
        }

        public async Task<SysUser?> GetBySidAsync(string sid)
            => string.IsNullOrEmpty(sid) ? null : await _db.Set<SysUser>().AsNoTracking().FirstOrDefaultAsync(x => x.Sid == sid);
        public async Task<SysUser?> GetByAccountAsync(string account)
            => string.IsNullOrEmpty(account) ? null : await _db.Set<SysUser>().AsNoTracking().FirstOrDefaultAsync(x => x.Account == account);
        public async Task<SysUser?> GetByMemberSidAsync(string memberSid)
            => string.IsNullOrEmpty(memberSid) ? null : await _db.Set<SysUser>().AsNoTracking().FirstOrDefaultAsync(x => x.MemberSid == memberSid);
        public async Task<SysUser?> GetByResetTokenAsync(string token)
            => string.IsNullOrEmpty(token) ? null : await _db.Set<SysUser>().FirstOrDefaultAsync(x => x.PasswordResetToken == token);
        public async Task UpdateAsync(SysUser entity)
        {
            _db.Set<SysUser>().Update(entity);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}