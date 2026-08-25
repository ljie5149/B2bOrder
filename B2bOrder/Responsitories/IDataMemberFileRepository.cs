using Microsoft.EntityFrameworkCore;
using B2bOrder.Data;
using B2bOrder.Models.Db;

namespace B2bOrder.Responsitories
{
    public interface IDataMemberFileRepository : IRepository<DataMemberFile>
    {
        Task<IEnumerable<DataMemberFile>> GetByMemberSidAsync(string memberSid);
    }

    public class DataMemberFileRepository : IDataMemberFileRepository
    {
        private readonly ApplicationDbContext _db;
        public DataMemberFileRepository(ApplicationDbContext db) => _db = db;

        public async Task AddAsync(DataMemberFile entity) => await _db.Set<DataMemberFile>().AddAsync(entity);

        public async Task DeleteAsync(DataMemberFile entity)
        {
            _db.Set<DataMemberFile>().Remove(entity);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<DataMemberFile>> GetAllAsync()
            => await _db.Set<DataMemberFile>().AsNoTracking().ToListAsync();

        public async Task<DataMemberFile?> GetByIdAsync(object id)
        {
            if (id is int nid) return await _db.Set<DataMemberFile>().FindAsync(nid);
            return null;
        }

        public async Task<IEnumerable<DataMemberFile>> GetByMemberSidAsync(string memberSid)
            => string.IsNullOrEmpty(memberSid) ? Array.Empty<DataMemberFile>() : await _db.Set<DataMemberFile>().AsNoTracking().Where(f => f.MemberSid == memberSid).ToListAsync();

        public async Task UpdateAsync(DataMemberFile entity)
        {
            _db.Set<DataMemberFile>().Update(entity);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}