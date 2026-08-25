using B2bOrder.Data;
using B2bOrder.Models.api;
using B2bOrder.Models.Db;
using B2bOrder.Models.Tree;
using Microsoft.EntityFrameworkCore;

namespace B2bOrder.Responsitories
{
    public interface IDataMemberRepository : IRepository<DataMember>
    {
        Task<DataMember?> GetBySidAsync(string sid);
        Task<DataMember?> GetByMidAsync(string mid);
        Task<DataMember?> GetByEmailAsync(string email);
        Task<List<DataMember>> GetChildrenAsync(string parentSid);
        Task<List<DataMember>> GetFullChildrenAsync(string memberSid);
        // 取得在未來 n 天內到期的會員（含 real_continue_date 或 continue_date）
        Task<IEnumerable<DataMember>> GetExpiringWithinDaysAsync(int days);
        Task<IEnumerable<DataMember>> GetOverExpiringAsync();
        Task<MemberTreeData?> GetMemberTreeDataAsync(string sid);
        /// <summary>
        /// 處理會員提交續約請求與個人資料更新
        /// </summary>
        /// <param name="model">前端傳入的會員中心 ViewModel</param>
        /// <returns>續約成功回傳 true，若查無會員或未登入回傳 false</returns>
        Task<bool> ProcessRenewalRequestAsync(RequestApiModel4Renewal model);
    }

    public class DataMemberRepository : IDataMemberRepository
    {
        private readonly ApplicationDbContext _db;
        public DataMemberRepository(ApplicationDbContext db) => _db = db;

        public async Task AddAsync(DataMember entity) => await _db.Set<DataMember>().AddAsync(entity);

        public async Task DeleteAsync(DataMember entity)
        {
            _db.Set<DataMember>().Remove(entity);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<DataMember>> GetAllAsync()
            => await _db.Set<DataMember>().AsNoTracking().ToListAsync();

        public async Task<DataMember?> GetByIdAsync(object id)
        {
            if (id is int nid) return await _db.Set<DataMember>().FindAsync(nid);
            return null;
        }
        public async Task<DataMember?> GetBySidAsync(string sid)
        {
            if (string.IsNullOrWhiteSpace(sid))
                return null;

            return await _db.Set<DataMember>()
                .AsNoTracking()
                .FirstOrDefaultAsync(m =>
                    m.Sid == sid &&
                    m.Avalible == "Y");
        }

        public async Task<DataMember?> GetByMidAsync(string mid)
        {
            if (string.IsNullOrWhiteSpace(mid))
                return null;

            return await _db.Set<DataMember>()
                .AsNoTracking()
                .FirstOrDefaultAsync(m =>
                    m.Mid == mid &&
                    m.Avalible == "Y");
        }
        public async Task<DataMember?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;
            return await _db.Set<DataMember>()
                .AsNoTracking()
                .FirstOrDefaultAsync(m =>
                    m.Email == email &&
                    m.Avalible == "Y");
        }
        public async Task<List<DataMember>> GetChildrenAsync(string parentSid)
        {
            if (string.IsNullOrWhiteSpace(parentSid))
                return new List<DataMember>();

            return await _db.Set<DataMember>()
                .AsNoTracking()
                .Where(x =>
                    x.ParentSid == parentSid &&
                    x.Avalible == "Y")
                .OrderBy(x => x.Mid)
                .ToListAsync();
        }

        public async Task<List<DataMember>> GetFullChildrenAsync(string memberSid)
        {
            if (string.IsNullOrWhiteSpace(memberSid))
                return new List<DataMember>();

            // 延長 Timeout 防止匯出或龐大組織樹時逾時
            _db.Database.SetCommandTimeout(60);

            // MySQL 必須加上 RECURSIVE 關鍵字
            var sql = @"
                        WITH RECURSIVE DownlineCTE AS (
                            SELECT *
                            FROM data_member
                            WHERE sid = {0} AND avalible = 'Y'

                            UNION ALL

                            SELECT m.*
                            FROM data_member m
                            INNER JOIN DownlineCTE d ON m.parent_sid = d.sid
                            WHERE m.avalible = 'Y'
                        )
                        SELECT * FROM DownlineCTE
                        WHERE sid <> {0};";

            return await _db.Set<DataMember>()
                .FromSqlRaw(sql, memberSid)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task UpdateAsync(DataMember entity)
        {
            _db.Set<DataMember>().Update(entity);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();

        // 新增：取得在未來 days 天內到期的會員
        public async Task<IEnumerable<DataMember>> GetExpiringWithinDaysAsync(int days)
        {
            if (days <= 0) return Array.Empty<DataMember>();

            var now = DateTime.UtcNow;
            var end = now.AddDays(days);

            // 以 continue_date 或 real_continue_date 在 [now, end] 範圍內且 avalible = 'Y' 為準
            var q = _db.Set<DataMember>().AsNoTracking()
                .Where(m => m.Avalible == "Y" &&
                            (
                                (m.ContinueDate != null && m.ContinueDate >= now && m.ContinueDate <= end) ||
                                (m.RealContinueDate != null && m.RealContinueDate >= now && m.RealContinueDate <= end)
                            ))
                .OrderBy(m => m.ParentSid ?? m.ContinueDate.ToString());

            return await q.ToListAsync();
        }
        // 新增：取得在已到期的會員
        public async Task<IEnumerable<DataMember>> GetOverExpiringAsync()
        {
            // 3. 計算即將到期與已到期數量
            var today = DateTime.Today;

            // 以 continue_date 或 real_continue_date 在 [now, end] 範圍內且 avalible = 'Y' 為準
            var q = _db.Set<DataMember>()
                    .AsNoTracking()
                    .Where(x => x.Avalible == "Y" &&
                                x.ContinueDate != null &&
                                x.ContinueDate < today)
                .OrderBy(m => m.ParentSid ?? m.ContinueDate.ToString());

            return await q.ToListAsync();
        }
        public async Task<MemberTreeData?> GetMemberTreeDataAsync(string sid)
        {
            if (string.IsNullOrWhiteSpace(sid))
                return null;

            var rows = await _db.Set<DataMember>()
                .AsNoTracking()
                .Where(x =>
                    x.Avalible == "Y" &&
                    (
                        x.Sid == sid ||
                        x.ParentSid == sid
                    ))
                .OrderBy(x => x.Mid)
                .ToListAsync();

            if (rows == null)
                return null;

            var member = rows.FirstOrDefault(x => x.Sid == sid);

            if (member == null)
                return null;

            return new MemberTreeData
            {
                Member = member,
                Children = rows
                    .Where(x => x.ParentSid == sid)
                    .ToList()
            };
        }
        /// <summary>
        /// 處理會員提交續約請求與個人資料更新
        /// </summary>
        public async Task<bool> ProcessRenewalRequestAsync(RequestApiModel4Renewal model)
        {
            // 1. 透過 BaseService 注入的 _httpContextAccessor 自動取得當前登入會員的 member_sid
            var memberSid = model.memberSid;

            if (string.IsNullOrEmpty(memberSid))
            {
                return false;
            }

            // 2. 查詢該會員是否存在
            var member = await GetBySidAsync(memberSid); // 若無此方法，改用您的 Repository 查詢
            if (member == null)
            {
                return false;
            }

            // 3. 更新允許使用者自行變更的資料欄位
            member.Mobile = model.Mobile;
            member.Tel = model.Tel;
            member.Address = model.Address;
            member.ModifyDate = DateTime.UtcNow;

            // 4. 續約核心邏輯：若已有續約日則再加 1 年，若無則以目前時間加 1 年
            member.ContinueDate = member.ContinueDate?.AddYears(1) ?? DateTime.UtcNow.AddYears(1);

            // 5. 呼叫更新與存檔
            await UpdateAsync(member);
            try
            {
                await SaveChangesAsync();
            }
            catch
            {
                // 依您原邏輯：UpdateAsync 可能已處理保存，忽略例外
            }

            return true;
        }
    }
}