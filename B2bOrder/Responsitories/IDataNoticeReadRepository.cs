using B2bOrder.Data;
using B2bOrder.Models.Db;
using Microsoft.EntityFrameworkCore;
using System;

namespace B2bOrder.Repositories
{
    public interface IDataNoticeReadRepository
    {
        /// <summary>
        /// 新增一筆已讀紀錄
        /// </summary>
        Task<bool> AddAsync(DataNoticeRead entity);

        /// <summary>
        /// 檢查該會員是否已經讀過特定公告
        /// </summary>
        Task<bool> ExistsAsync(string noticeSid, string memberSid);

        /// <summary>
        /// 取得該會員在 data_notice_read 中所有已讀的 notice_sid 列表
        /// </summary>
        Task<IEnumerable<string>> GetReadNoticeSidsByMemberAsync(string memberSid);

        /// <summary>
        /// 清空特定會員的所有已讀紀錄（用於一鍵全部已讀時釋放空間）
        /// </summary>
        Task<bool> DeleteByMemberSidAsync(string memberSid);
    }
    public class DataNoticeReadRepository : IDataNoticeReadRepository
    {
        private readonly ApplicationDbContext _context;

        public DataNoticeReadRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(DataNoticeRead entity)
        {
            await _context.DataNoticeReads.AddAsync(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsAsync(string noticeSid, string memberSid)
        {
            return await _context.DataNoticeReads
                .AnyAsync(r => r.NoticeSid == noticeSid && r.MemberSid == memberSid);
        }

        public async Task<IEnumerable<string>> GetReadNoticeSidsByMemberAsync(string memberSid)
        {
            return await _context.DataNoticeReads
                .Where(r => r.MemberSid == memberSid)
                .Select(r => r.NoticeSid)
                .ToListAsync();
        }

        public async Task<bool> DeleteByMemberSidAsync(string memberSid)
        {
            var records = await _context.DataNoticeReads
                .Where(r => r.MemberSid == memberSid)
                .ToListAsync();

            if (!records.Any()) return true;

            _context.DataNoticeReads.RemoveRange(records);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}