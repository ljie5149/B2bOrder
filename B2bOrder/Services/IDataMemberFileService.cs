using B2bOrder.Models.Db;
using B2bOrder.Responsitories;

namespace B2bOrder.Services
{
    public interface IDataMemberFileService
    {
        Task<IEnumerable<DataMemberFile>> GetAllAsync();
        Task<DataMemberFile?> GetByIdAsync(int nid);
        Task<IEnumerable<DataMemberFile>> GetByMemberSidAsync(string memberSid);
        Task CreateAsync(DataMemberFile entity);
        Task UpdateAsync(DataMemberFile entity);
        Task DeleteAsync(int nid);
        Task SaveChangesAsync();
    }
    public class DataMemberFileService : BaseService, IDataMemberFileService
    {
        private readonly IDataMemberFileRepository _repo;
        public DataMemberFileService(IServiceProvider serviceProvider, IDataMemberFileRepository repo) : base(serviceProvider)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<DataMemberFile>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<DataMemberFile?> GetByIdAsync(int nid) => await _repo.GetByIdAsync(nid);

        public async Task<IEnumerable<DataMemberFile>> GetByMemberSidAsync(string memberSid) => await _repo.GetByMemberSidAsync(memberSid);

        public async Task CreateAsync(DataMemberFile entity)
        {
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
        }

        public async Task UpdateAsync(DataMemberFile entity)
        {
            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int nid)
        {
            var e = await _repo.GetByIdAsync(nid);
            if (e is null) return;
            await _repo.DeleteAsync(e);
            await _repo.SaveChangesAsync();
        }

        public async Task SaveChangesAsync() => await _repo.SaveChangesAsync();
    }
}