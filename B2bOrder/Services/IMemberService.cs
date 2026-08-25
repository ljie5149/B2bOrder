using B2bOrder.Hubs;
using B2bOrder.Models;
using B2bOrder.Models.api;
using B2bOrder.Models.Common;
using B2bOrder.Models.Db;
using B2bOrder.Models.Tree;
using B2bOrder.Responsitories;
using ClosedXML.Excel;
using CsvHelper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using static B2bOrder.Controllers.MemberController;

namespace B2bOrder.Services
{
    public interface IMemberService : IBaseService
    {
        Task<IEnumerable<DataMember>> GetAllAsync();
        Task<DataMember?> GetByIdAsync(int nid);
        Task<DataMember?> GetBySidAsync(string sid);
        Task<IEnumerable<DataMember>> GetChildrenAsync(string parentSid);
        Task<IEnumerable<DataMember>> GetFullChildrenAsync(string memberSid);
        Task<DataMember?> GetByMidAsync(string mid);
        Task<DataMember?> GetByEmailAsync(string email);
        Task CreateAsync(DataMember entity);
        Task UpdateAsync(DataMember entity, bool renew = false);
        Task DeleteAsync(int nid);
        Task SaveChangesAsync();
        Task<MemberTreeData?> GetMemberTreeDataAsync(string sid);
        /// <summary>
        /// 處理會員提交續約請求與個人資料更新
        /// </summary>
        /// <param name="model">前端傳入的會員中心 ViewModel</param>
        /// <returns>續約成功回傳 true，若查無會員或未登入回傳 false</returns>
        Task<bool> ProcessRenewalRequestAsync(MemberCenterViewModel model);
        /// <summary>
        /// 處理會員更新個人資料（包含時區轉換與紀錄軌跡）
        /// </summary>
        /// <param name="model">前端傳入的會員中心 ViewModel</param>
        /// <returns>更新成功回傳 true；未登入或查無會員回傳 false</returns>
        Task<bool> UpdateProfileAsync(MemberCenterViewModel model);
        /// <summary>
        /// 獲取當前登入會員的完整組織樹（自動處理遞迴節點、無窮迴圈防禦與團隊人數計算）
        /// </summary>
        /// <returns>回傳組織樹的根節點；若未登入或查無資料則回傳 null</returns>
        Task<MemberTreeNode?> GetOrganizationTreeAsync(string memberSid);
        /// <summary>
        /// 處理刪除（停用）會員及其下線子會員的樹狀重組邏輯
        /// </summary>
        /// <param name="model">刪除請求的參數</param>
        /// <returns>回傳執行結果與錯誤訊息</returns>
        Task<(bool Success, string Message)> DeleteMemberAsync(DeleteMemberViewModel model);
        #region 新增下線會員
        /// <summary>
        /// 初始化新增下線會員所需的 ViewModel
        /// </summary>
        Task<CreateMemberViewModel?> PrepareCreateViewModelAsync(string parentSid);

        /// <summary>
        /// 執行建立下線會員的商業邏輯與日誌紀錄
        /// </summary>
        /// <returns>Tuple 代表 (是否成功, 錯誤訊息)</returns>
        Task<(bool Success, string Message)> CreateChildMemberAsync(CreateMemberViewModel model);
        #endregion
        #region 編輯下線會員
        /// <summary>
        /// 獲取指定會員的編輯初始資料（包含上線帳號 Mid 與日期格式化）
        /// </summary>
        Task<object?> GetMemberForEditAsync(string sid);

        /// <summary>
        /// 執行編輯會員資料的商業邏輯與日誌紀錄
        /// </summary>
        /// <returns>Tuple 代表 (是否成功, 錯誤訊息)</returns>
        Task<(bool Success, string Message)> UpdateChildMemberAsync(EditMemberViewModel model);
        #endregion
        #region 續約會員 (由組織圖的 modal 提交)
        /// <summary>
        /// 處理後台或系統觸發的會員續約商業邏輯與日誌紀錄
        /// </summary>
        /// <param name="model">續約請求的 ViewModel</param>
        /// <returns>Tuple 代表 (是否成功, 錯誤訊息)</returns>
        Task<(bool Success, string Message)> RenewMemberAsync(RenewMemberViewModel model);
        #endregion
        #region 會員列表（含搜尋與分頁）
        /// <summary>
        /// 獲取當前會員及其所有下線的篩選、排序、分頁清單
        /// </summary>
        Task<MemberPagedResultDto?> GetPagedMemberListAsync(
            string keyword, string status, string sortField, string sortDir, int page, int pageSize);
        #endregion
        #region 匯出會員資料（CSV 或 Excel）
        /// <summary>
        /// 根據條件生成匯出檔案的二進位資料與格式資訊
        /// </summary>
        /// <returns>包含 (檔案位元組陣列, MIME 類型, 檔案名稱) 的 Tuple</returns>
        Task<(byte[] FileBytes, string ContentType, string FileName)> ExportMembersAsync(
            string memberSid,
            string type, string scope, string keyword, string status);
        #endregion
        #region 匯入會員資料（CSV 或 Excel）
        /// <summary>
        /// 處理 CSV 或 Excel 檔案會員匯入、即時進度推播與資料庫更新
        /// </summary>
        Task<ImportResult> ImportMembersAsync(IFormFile file, string operatorSid, string operatorMid);
        #endregion
    }
    public class MemberService : BaseService, IMemberService
    {
        private readonly IHubContext<MemberImportHub> _hub; // 請替換成您實際的 Hub 類別名稱
        private readonly IDataMemberRepository _repo;
        public MemberService(IServiceProvider serviceProvider, IDataMemberRepository repo, IHubContext<MemberImportHub> hub) : base(serviceProvider)
        {
            _repo = repo;
            _hub = hub;
        }

        public async Task<IEnumerable<DataMember>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<DataMember?> GetByIdAsync(int nid) => await _repo.GetByIdAsync(nid);

        public async Task<DataMember?> GetBySidAsync(string sid) => await _repo.GetBySidAsync(sid);

        public async Task<IEnumerable<DataMember>> GetChildrenAsync(string parentSid) => await _repo.GetChildrenAsync(parentSid);
        public async Task<IEnumerable<DataMember>> GetFullChildrenAsync(string memberSid)
        {
            var result = Enumerable.Empty<DataMember>();
            try
            {
                result = await _repo.GetFullChildrenAsync(memberSid);
            }
            catch (Exception ex)
            {
                // 在這裡下斷點，查看 ex.Message 或 ex.InnerException.Message
                // 通常真正的原因（如：InvalidCastException 或 Timeout）會藏在 InnerException 裡面！
                throw;
            }
            return result;
        }
        public async Task<DataMember?> GetByMidAsync(string mid) => await _repo.GetByMidAsync(mid);
        public async Task<DataMember?> GetByEmailAsync(string email) => await _repo.GetByEmailAsync(email);
        public async Task CreateAsync(DataMember entity)
        {
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
        }

        public async Task UpdateAsync(DataMember entity, bool renew = false)
        {
            if (renew)
            {
                try
                {
                    var member =
                        await GetBySidAsync(
                            entity.Sid
                        );

                    if (member == null)
                    {
                        throw new UnauthorizedAccessException("續約申請資料不能為空。");
                    }

                    entity.ContinueDate = DateTime.Today.AddYears(1); // (member.ContinueDate ?? DateTime.Today).AddYears(1);
                    entity.RealContinueDate = DateTime.Now;
                    entity.ModifyDate = DateTime.Now;
                    // 調用服務層處理續約邏輯（例如建立一筆續約審核單，或直接展延並記錄等）
                }
                catch (Exception ex)
                {
                    throw new UnauthorizedAccessException($"提交續約申請失敗: {ex.Message}");
                }
            }

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
        public async Task<MemberTreeData?> GetMemberTreeDataAsync(string sid)
        {
            return await _repo.GetMemberTreeDataAsync(sid);
        }
        /// <summary>
        /// 處理會員提交續約請求與個人資料更新
        /// </summary>
        public async Task<bool> ProcessRenewalRequestAsync(MemberCenterViewModel model)
        {
            // 1. 透過 BaseService 注入的 _httpContextAccessor 自動取得當前登入會員的 member_sid
            var httpContext = _httpContextAccessor.HttpContext;
            var memberSid = httpContext?.User.FindFirst("member_sid")?.Value;

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

            // 6. 統一紀錄操作軌跡日誌
            await WriteLog(
                actionType: ActionTypes.Renewal,
                targetSid: memberSid,
                targetMid: member.Mid,
                moduleName: ModuleNames.MemberManager,
                actionName: ActionNames.RequesgtMember,
                description: $"會員續約申請：{member.Mid}({memberSid})"
            );

            return true;
        }
        /// <summary>
        /// 處理會員更新個人資料（包含時區轉換與紀錄軌跡）
        /// </summary>
        public async Task<bool> UpdateProfileAsync(MemberCenterViewModel model)
        {
            // 1. 透過 BaseService 注入的 _httpContextAccessor 自動取得當前登入會員的 member_sid
            var httpContext = _httpContextAccessor.HttpContext;
            var memberSid = httpContext?.User.FindFirst("member_sid")?.Value;

            if (string.IsNullOrEmpty(memberSid))
            {
                return false;
            }

            // 2. 查詢該會員是否存在
            var member = await GetBySidAsync(memberSid);
            if (member == null)
            {
                return false;
            }

            // 3. 更新允許修改的欄位
            member.Name = model.Name;
            member.Email = model.Email;
            member.Mobile = model.Mobile;
            member.Tel = model.Tel;
            member.Address = model.Address;
            member.Birthday = model.Birthday;
            member.ModifyDate = DateTime.UtcNow;

            // 4. 處理 ContinueDate 時區轉換
            if (model.ContinueDate.HasValue)
            {
                var c = model.ContinueDate.Value;
                if (c.Kind == DateTimeKind.Unspecified)
                {
                    // 將不帶時區的時間視為使用者本機時間，再轉為 UTC 存入資料庫
                    c = DateTime.SpecifyKind(c, DateTimeKind.Local);
                }
                member.ContinueDate = c.ToUniversalTime();
            }

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

            // 6. 統一紀錄操作軌跡日誌
            await WriteLog(
                actionType: ActionTypes.EditMember,
                targetSid: memberSid,
                targetMid: member.Mid,
                moduleName: ModuleNames.MemberManager,
                actionName: ActionNames.updateMember,
                description: $"更新個人資料：{member.Mid}({memberSid})"
            );

            return true;
        }
        #region 織織圖
        /// <summary>
        /// 獲取當前登入會員的完整組織樹
        /// </summary>
        public async Task<MemberTreeNode?> GetOrganizationTreeAsync(string memberSid)
        {
            // 1. 透過 BaseService 注入的 _httpContextAccessor 自動取得當前登入會員的 member_sid

            if (string.IsNullOrEmpty(memberSid))
            {
                return null;
            }

            // 2. 遞迴建置樹狀結構
            var rootNode = await BuildMemberTreeAsync(memberSid);
            if (rootNode == null)
            {
                return null;
            }

            // 3. 標記為根節點
            rootNode.IsRoot = true;

            // 4. 遞迴計算團隊人數
            CalculateAndSetTeamCount(rootNode);

            return rootNode;
        }

        /// <summary>
        /// 遞迴建立組織樹節點
        /// </summary>
        private async Task<MemberTreeNode?> BuildMemberTreeAsync(string sid, HashSet<string>? visited = null, int depth = 0)
        {
            // 防禦深度過深
            if (depth > 100) return null;

            visited ??= new HashSet<string>();

            // 防禦循環指派（無窮迴圈）
            if (!visited.Add(sid)) return null;

            // 呼叫您現有的資料庫撈取方法
            var data = await GetMemberTreeDataAsync(sid);
            if (data == null || data.Member == null) return null;

            var node = new MemberTreeNode
            {
                Sid = data.Member.Sid,
                Mid = data.Member.Mid,
                Name = data.Member.Name ?? "",
                Role = data.Member.Role,
                Children = new List<MemberTreeNode>()
            };

            // 遞迴處理子節點
            foreach (var child in data.Children)
            {
                var childNode = await BuildMemberTreeAsync(child.Sid, visited, depth + 1);
                if (childNode != null)
                {
                    node.Children.Add(childNode);
                }
            }

            return node;
        }

        /// <summary>
        /// 遞迴計算並設定每個節點的子團隊總人數
        /// </summary>
        private int CalculateAndSetTeamCount(MemberTreeNode node)
        {
            int total = 0;

            foreach (var child in node.Children)
            {
                total += 1; // 計入子節點本身
                total += CalculateAndSetTeamCount(child); // 計入子節點之下的所有下線
            }

            node.TeamCount = total;
            return total;
        }
        /// <summary>
        /// 處理刪除（停用）會員及其下線子會員的樹狀重組邏輯
        /// </summary>
        public async Task<(bool Success, string Message)> DeleteMemberAsync(DeleteMemberViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Sid))
            {
                return (false, "會員不存在");
            }

            // 1. 透過 BaseService 注入的 _httpContextAccessor 取得當前登入者的 member_sid (即根會員)
            var httpContext = _httpContextAccessor.HttpContext;
            var loginSid = httpContext?.User.FindFirst("member_sid")?.Value;

            if (model.Sid == loginSid)
            {
                return (false, "根會員不可刪除");
            }

            // 2. 檢查目標會員是否存在
            var member = await GetBySidAsync(model.Sid);
            if (member == null)
            {
                return (false, "會員不存在");
            }

            // 3. 取得目標會員的所有直接下線
            var children = (await GetChildrenAsync(model.Sid)).ToList();
            var handleTypeLabel = "";

            // 4. 根據前端選擇的下線處理策略進行重組
            switch (model.HandleType)
            {
                case "MOVE_PARENT":
                    handleTypeLabel = "移轉至上線";
                    foreach (var child in children)
                    {
                        child.ParentSid = member.ParentSid;
                        await UpdateAsync(child);
                    }
                    break;

                case "DISABLE_ALL":
                    handleTypeLabel = "全部停用";
                    foreach (var child in children)
                    {
                        child.Avalible = "D";
                        child.ModifyDate = DateTime.UtcNow; // 建議統一用 UtcNow，或依您原資料庫習慣用 DateTime.Now
                        await UpdateAsync(child);
                    }
                    break;

                case "MOVE_TARGET":
                    if (string.IsNullOrWhiteSpace(model.TargetMid))
                    {
                        return (false, "請指定接收會員");
                    }

                    handleTypeLabel = $"移轉至指定會員 {model.TargetMid}";
                    var targetMember = await GetByMidAsync(model.TargetMid);
                    if (targetMember == null)
                    {
                        return (false, "接收會員不存在");
                    }

                    foreach (var child in children)
                    {
                        child.ParentSid = targetMember.Sid;
                        await UpdateAsync(child);
                    }
                    break;

                default:
                    handleTypeLabel = "未指定處理方式";
                    break;
            }

            // 5. 停用目標會員本體
            member.Avalible = "D";
            member.ModifyDate = DateTime.UtcNow;
            await UpdateAsync(member);

            // 6. 寫入操作日誌
            await WriteLog(
                actionType: ActionTypes.DeleteMember,
                targetSid: member.Sid,
                targetMid: member.Mid,
                moduleName: ModuleNames.MemberManager,
                actionName: ActionNames.updateMember,
                description: $"刪除會員：{member.Mid}, 選擇：{handleTypeLabel}"
            );

            return (true, "成功");
        }
        #endregion
        #region 新增下線會員
        /// <summary>
        /// 初始化新增下線會員所需的 ViewModel
        /// </summary>
        public async Task<CreateMemberViewModel?> PrepareCreateViewModelAsync(string parentSid)
        {
            if (string.IsNullOrEmpty(parentSid)) return null;

            var parent = await GetBySidAsync(parentSid);
            if (parent == null) return null;

            return new CreateMemberViewModel
            {
                ParentSid = parent.Sid,
                ParentMid = parent.Mid,
                JoinDate = DateTime.Today
            };
        }

        /// <summary>
        /// 執行建立下線會員的商業邏輯與日誌紀錄
        /// </summary>
        public async Task<(bool Success, string Message)> CreateChildMemberAsync(CreateMemberViewModel model)
        {
            // 1. 商業驗證：檢查上線
            if (string.IsNullOrWhiteSpace(model.ParentSid))
            {
                return (false, "上線會員不存在");
            }

            // 2. 商業驗證：檢查帳號重複
            var exists = await GetByMidAsync(model.Mid);
            if (exists != null)
            {
                return (false, "會員編號已存在");
            }

            // 3. 組裝 DataMember 實體物件
            var member = new DataMember
            {
                Sid = Guid.NewGuid().ToString("N"),
                CreateDate = DateTime.UtcNow, // 建議統一使用 UtcNow，或依系統常規使用 DateTime.Now
                ParentSid = model.ParentSid,
                Mid = model.Mid,
                Name = model.Name,
                Mobile = model.Mobile,
                Email = model.Email,
                Iden = model.IdNumber,
                Role = model.Role,
                JoinDate = model.JoinDate,
                ContinueDate = model.ContinueDate,
                RealContinueDate = model.RealContinueDate,
                Avalible = "Y",
                RegisterStatus = "REGISTERED"
            };

            // 4. 寫入資料庫
            await CreateAsync(member);
            await SaveChangesAsync();

            // 5. 紀錄操作日誌
            await WriteLog(
                actionType: ActionTypes.CreateMember,
                targetSid: member.Sid,
                targetMid: member.Mid,
                moduleName: ModuleNames.MemberManager,
                actionName: ActionNames.CreateMember,
                description: $"新增下線會員：{member.Mid} {member.Name}"
            );

            return (true, "成功");
        }
        #endregion

        #region 編輯下線會員
        /// <summary>
        /// 獲取指定會員的編輯初始資料
        /// </summary>
        public async Task<object?> GetMemberForEditAsync(string sid)
        {
            if (string.IsNullOrEmpty(sid)) return null;

            var member = await GetBySidAsync(sid);
            if (member == null) return null;

            string parentMid = "";
            if (!string.IsNullOrWhiteSpace(member.ParentSid))
            {
                var parent = await GetBySidAsync(member.ParentSid);
                parentMid = parent?.Mid ?? "";
            }

            // 回傳格式化好的匿名物件（或您可以定義一個專屬的 DTO 物件）
            return new
            {
                sid = member.Sid,
                mid = member.Mid,
                name = member.Name,
                mobile = member.Mobile,
                email = member.Email,
                idNumber = member.Iden,
                role = member.Role,
                joinDate = member.JoinDate?.ToString("yyyy-MM-dd"),
                continueDate = member.ContinueDate?.ToString("yyyy-MM-dd"),
                parentMid
            };
        }

        /// <summary>
        /// 執行編輯會員資料的商業邏輯與日誌紀錄
        /// </summary>
        public async Task<(bool Success, string Message)> UpdateChildMemberAsync(EditMemberViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Sid))
            {
                return (false, "會員不存在");
            }

            // 1. 檢查目標會員是否存在
            var member = await GetBySidAsync(model.Sid);
            if (member == null)
            {
                return (false, "會員不存在");
            }

            // 2. 更新欄位
            member.Name = model.Name;
            member.Mobile = model.Mobile;
            member.Email = model.Email;
            member.Iden = model.IdNumber;
            member.Role = model.Role;
            member.JoinDate = model.JoinDate;
            member.ContinueDate = model.ContinueDate;
            member.ModifyDate = DateTime.UtcNow; // 建議統一使用 UtcNow，或依系統常規使用 DateTime.Now

            // 3. 呼叫更新與存檔
            await UpdateAsync(member);
            await SaveChangesAsync();

            // 4. 紀錄操作日誌
            await WriteLog(
                actionType: ActionTypes.EditMember,
                targetSid: member.Sid,
                targetMid: member.Mid,
                moduleName: ModuleNames.MemberManager,
                actionName: ActionNames.EditMember,
                description: $"編輯會員 ({member.Mid})"
            );

            return (true, "成功");
        }
        #endregion

        #region 續約會員 (由組織圖的 modal 提交)
        /// <summary>
        /// 處理後台或系統觸發的會員續約商業邏輯與日誌紀錄
        /// </summary>
        public async Task<(bool Success, string Message)> RenewMemberAsync(RenewMemberViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Sid))
            {
                return (false, "會員不存在");
            }

            // 1. 檢查目標會員是否存在
            var member = await GetBySidAsync(model.Sid);
            if (member == null)
            {
                return (false, "會員不存在");
            }

            // 2. 呼叫您既有帶有續約參數分支的 UpdateAsync 核心邏輯
            await UpdateAsync(member, true); // 這裡依您實際定義的參數名稱為主（如：true）
            await SaveChangesAsync();

            // 3. 紀錄操作日誌
            await WriteLog(
                actionType: ActionTypes.Renewal,
                targetSid: member.Sid,
                targetMid: member.Mid,
                moduleName: ModuleNames.MemberManager,
                actionName: ActionNames.RenewMember,
                description: $"會員續約 ({member.Mid})"
            );

            return (true, "成功");
        }
        #endregion

        #region 會員列表（含搜尋與分頁）
        /// <summary>
        /// 獲取當前會員及其所有下線的篩選、排序、分頁清單
        /// </summary>
        public async Task<MemberPagedResultDto?> GetPagedMemberListAsync(
            string keyword, string status, string sortField, string sortDir, int page, int pageSize)
        {
            // 1. 透過 BaseService 注入的 _httpContextAccessor 自動取得當前登入會員的 member_sid
            var httpContext = _httpContextAccessor.HttpContext;
            var memberSid = httpContext?.User.FindFirst("member_sid")?.Value;

            if (string.IsNullOrEmpty(memberSid)) return null;

            var loginMember = await GetBySidAsync(memberSid);
            if (loginMember == null) return null;

            // 2. 利用遞迴，撈出該會員底下「所有層級」的子孫會員
            var allMembers = new List<DataMember>();
            await GetAllDescendantsAsync(memberSid, allMembers);

            // 將登入者本人也加進去名單中
            allMembers.Add(loginMember);

            IEnumerable<DataMember> members = allMembers;

            // 3. 關鍵字篩選
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                members = members.Where(x =>
                    (x.Sid ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (x.Mid ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (x.Name ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            // 4. 狀態篩選
            if (!string.IsNullOrWhiteSpace(status))
            {
                members = members.Where(x => x.Avalible == status);
            }

            // 5. 排序邏輯
            members = sortField?.ToLower() switch
            {
                "sid" => sortDir == "desc" ? members.OrderByDescending(x => x.Sid) : members.OrderBy(x => x.Sid),
                "mid" => sortDir == "desc" ? members.OrderByDescending(x => x.Mid) : members.OrderBy(x => x.Mid),
                "name" => sortDir == "desc" ? members.OrderByDescending(x => x.Name) : members.OrderBy(x => x.Name),
                "role" => sortDir == "desc" ? members.OrderByDescending(x => x.Role) : members.OrderBy(x => x.Role),
                "joindate" => sortDir == "desc" ? members.OrderByDescending(x => x.JoinDate) : members.OrderBy(x => x.JoinDate),
                "continuedate" => sortDir == "desc" ? members.OrderByDescending(x => x.ContinueDate) : members.OrderBy(x => x.ContinueDate),
                _ => members.OrderBy(x => x.Mid)
            };

            // 6. 計算總數與分頁處理
            var totalCount = members.Count();
            var pagedData = members
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new MemberPagedResultDto
            {
                Data = pagedData,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 內部遞迴方法：撈取當前節點的所有子孫會員
        /// </summary>
        private async Task GetAllDescendantsAsync(string parentSid, List<DataMember> resultList)
        {
            var directChildren = await GetChildrenAsync(parentSid);
            if (directChildren != null && directChildren.Any())
            {
                foreach (var child in directChildren.Cast<DataMember>())
                {
                    resultList.Add(child);
                    // 遞迴呼叫：繼續往下撈這個子會員的下線
                    await GetAllDescendantsAsync(child.Sid, resultList);
                }
            }
        }
        #endregion

        #region 匯出會員資料（CSV 或 Excel）
        public async Task<(byte[] FileBytes, string ContentType, string FileName)> ExportMembersAsync(
            string memberSid,
            string type, string scope, string keyword, string status)
        {
            // 1. 撈取基礎資料與範圍篩選
            var loginMember = await GetBySidAsync(memberSid);
            var allChildrens = await GetFullChildrenAsync(memberSid);

            // 2. 合併會員本人與下線（直接以 IEnumerable 串接）
            IEnumerable<DataMember> membersQuery = allChildrens;
            if (loginMember != null)
            {
                membersQuery = new[] { loginMember }.Concat(allChildrens);
            }

            // 3. 延遲執行 (Lazy Evaluation) 進行過濾
            if (scope == "current")
            {
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    membersQuery = membersQuery.Where(x =>
                        (x.Sid ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        (x.Mid ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        (x.Name ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrWhiteSpace(status))
                {
                    membersQuery = membersQuery.Where(x => x.Avalible == status);
                }
            }

            var data = membersQuery.OrderBy(x => x.Mid).ToList();
            // 🔥 效能優化 (N+1問題解決)：一次性建立 Sid -> Mid 的字典檔快取，避免在迴圈中重複查資料庫
            var allMembersMap = (await GetAllAsync()).ToDictionary(x => x.Sid, x => x.Mid ?? "");

            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            // 2. 根據型態決定呼叫對應的生成方法
            if (type?.ToLower() == "csv")
            {
                var csvBytes = GenerateCsvBytes(data, allMembersMap);
                return (csvBytes, "text/csv", $"會員清單_{timestamp}.csv");
            }
            else
            {
                var excelBytes = GenerateExcelBytes(data, allMembersMap);
                return (excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"會員清單_{timestamp}.xlsx");
            }
        }

        private byte[] GenerateCsvBytes(List<DataMember> data, Dictionary<string, string> allMembersMap)
        {
            var sb = new StringBuilder();
            sb.AppendLine("MID,姓名,角色,手機,Email,身分證,加入日期,續約日期,上線,狀態");
            //sb.AppendLine("MID,姓名,角色,加入日期,續約日期,上線,狀態");

            foreach (var m in data)
            {
                string parentMid = !string.IsNullOrWhiteSpace(m.ParentSid) && allMembersMap.TryGetValue(m.ParentSid, out var mid)
                    ? mid
                    : "";

                sb.AppendLine(
                    $"{m.Mid}," +
                    $"{m.Name}," +
                    $"{m.Role}," +
                    $"{m.Mobile}," +
                    $"{m.Email}," +
                    $"{m.Iden}," +
                    $"{m.JoinDate:yyyy-MM-dd}," +
                    $"{m.ContinueDate:yyyy-MM-dd}," +
                    $"{parentMid}," +
                    $"{m.Avalible}");
            }

            // 加上 UTF-8 BOM 避免 Excel 開啟 CSV 時中文亂碼
            return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        }

        private byte[] GenerateExcelBytes(List<DataMember> data, Dictionary<string, string> allMembersMap)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("會員資料");

            string[] headers = { "MID", "姓名", "角色", "手機", "Email", "身分證", "加入日期", "續約日期", "上線", "狀態" };
            //string[] headers = { "MID", "姓名", "角色", "加入日期", "續約日期", "上線", "狀態" };
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(1, i + 1).Value = headers[i];
            }

            int row = 2, icol = 1;
            foreach (var m in data)
            {
                string parentMid = !string.IsNullOrWhiteSpace(m.ParentSid) && allMembersMap.TryGetValue(m.ParentSid, out var mid)
                    ? mid
                    : "";
                icol = 1;
                ws.Cell(row, icol++).Value = m.Mid;
                ws.Cell(row, icol++).Value = m.Name;
                ws.Cell(row, icol++).Value = m.Role;
                ws.Cell(row, icol++).Value = m.Mobile;
                ws.Cell(row, icol++).Value = m.Email;
                ws.Cell(row, icol++).Value = m.Iden;
                ws.Cell(row, icol++).Value = m.JoinDate;
                ws.Cell(row, icol++).Value = m.ContinueDate;
                ws.Cell(row, icol++).Value = parentMid;
                ws.Cell(row, icol++).Value = m.Avalible;
                row++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return stream.ToArray();
        }
        #endregion

        #region 匯入會員資料（CSV 或 Excel）
        /// <summary>
        /// 會員匯入核心商業邏輯 (包含快取優化與進度推播)
        /// </summary>
        public async Task<ImportResult> ImportMembersAsync(IFormFile file, string operatorSid, string operatorMid)
        {
            var result = new ImportResult();
            var ext = Path.GetExtension(file.FileName).ToLower();

            // 1. 一次性抓取全表做快取，完美解決迴圈內 N+1 Query 的效能地獄
            var allMembers = await GetAllAsync();
            var midToSidMap = allMembers.ToDictionary(x => x.Mid, x => x.Sid);
            var midToEntityMap = allMembers.ToDictionary(x => x.Mid, x => x);

            // 2. 分流解析檔案
            if (ext == ".csv")
            {
                result = await ProcessCsvImportAsync(file, midToSidMap, midToEntityMap, operatorSid);
            }
            else if (ext == ".xlsx")
            {
                result = await ProcessExcelImportAsync(file, midToSidMap, midToEntityMap, operatorSid);
            }

            // 3. 儲存變更並寫入 Log
            await SaveChangesAsync();

            await WriteLog(
                actionType: ActionTypes.ImportMember,
                targetSid: operatorSid,
                targetMid: operatorMid,
                moduleName: ModuleNames.ImportExport,
                actionName: ActionNames.ImportMember,
                description: $"會員匯入 檔案:{file.FileName} 新增:{result.InsertCount} 更新:{result.UpdateCount} 失敗:{result.SkipCount}"
            );

            return result;
        }

        private async Task<ImportResult> ProcessCsvImportAsync(
            IFormFile file,
            Dictionary<string, string> midToSidMap,
            Dictionary<string, DataMember> midToEntityMap,
            string operatorSid)
        {
            var result = new ImportResult();
            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();

            var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var total = lines.Length - 1; // 扣除表頭

            stream.Position = 0;
            using var csv = new CsvReader(new StreamReader(stream), CultureInfo.InvariantCulture);
            csv.Read();
            csv.ReadHeader();

            var current = 0;
            while (csv.Read())
            {
                current++;
                await ReportProgressAsync(current, total);

                var mid = csv.GetField(0);
                var parentMid = csv.GetField(8);

                if (!TryGetParentSid(parentMid, midToSidMap, mid, result, out var parentSid))
                    continue;

                try
                {
                    var isInsert = SaveImportMemberInternal(
                        mid, csv.GetField(1), csv.GetField(2), csv.GetField(3), csv.GetField(4), csv.GetField(5),
                        DateTime.Parse(csv.GetField(6)), DateTime.Parse(csv.GetField(7)),
                        parentSid, midToEntityMap, operatorSid);

                    if (isInsert) result.InsertCount++; else result.UpdateCount++;
                }
                catch (Exception ex)
                {
                    result.SkipCount++;
                    result.Errors.Add($"MID:{mid} => {ex.Message}");
                }
            }
            await ReportProgressAsync(total, total);
            return result;
        }

        private async Task<ImportResult> ProcessExcelImportAsync(
            IFormFile file,
            Dictionary<string, string> midToSidMap,
            Dictionary<string, DataMember> midToEntityMap,
            string operatorSid)
        {
            var result = new ImportResult();
            using var stream = file.OpenReadStream();
            using var wb = new XLWorkbook(stream);
            var ws = wb.Worksheet(1);
            var rows = ws.RowsUsed().Skip(1).ToList();
            var total = rows.Count;

            var current = 0;
            foreach (var row in rows)
            {
                current++;
                await ReportProgressAsync(current, total);

                var mid = row.Cell(1).GetString();
                var parentMid = row.Cell(9).GetString();

                if (!TryGetParentSid(parentMid, midToSidMap, mid, result, out var parentSid))
                    continue;

                try
                {
                    var isInsert = SaveImportMemberInternal(
                        mid, row.Cell(2).GetString(), row.Cell(3).GetString(), row.Cell(4).GetString(),
                        row.Cell(5).GetString(), row.Cell(6).GetString(), row.Cell(7).GetDateTime(),
                        row.Cell(8).GetDateTime(), parentSid, midToEntityMap, operatorSid);

                    if (isInsert) result.InsertCount++; else result.UpdateCount++;
                }
                catch (Exception ex)
                {
                    result.SkipCount++;
                    result.Errors.Add($"MID:{mid} => {ex.Message}");
                }
            }
            await ReportProgressAsync(total, total);
            return result;
        }

        private bool TryGetParentSid(string parentMid, Dictionary<string, string> midToSidMap, string mid, ImportResult result, out string? parentSid)
        {
            parentSid = null;
            if (!string.IsNullOrWhiteSpace(parentMid))
            {
                if (!midToSidMap.TryGetValue(parentMid, out parentSid))
                {
                    result.SkipCount++;
                    result.Errors.Add($"MID:{mid} 找不到上線會員({parentMid})");
                    return false;
                }
            }
            return true;
        }

        private bool SaveImportMemberInternal(
            string mid, string name, string role, string mobile, string email, string iden,
            DateTime? joinDate, DateTime? continueDate, string? parentSid,
            Dictionary<string, DataMember> midToEntityMap, string operatorSid)
        {
            bool isInsert = !midToEntityMap.TryGetValue(mid, out var member);

            if (isInsert)
            {
                member = new DataMember
                {
                    Sid = Guid.NewGuid().ToString("N"),
                    Mid = mid,
                    Avalible = "Y",
                    RegisterStatus = "REGISTERED",
                    RegisterSource = "IMPORT",
                    CreateDate = DateTime.Now
                };
                FillMember(member, name, role, mobile, email, iden, joinDate, continueDate, parentSid);
                CreateAsync(member); // 調用基礎 BaseService 的本地快取新增
                midToEntityMap[mid] = member; // 動態更新快取，防止同批檔案內有重複 mid 的資料
            }
            else
            {
                FillMember(member!, name, role, mobile, email, iden, joinDate, continueDate, parentSid);
                member!.ModifyDate = DateTime.Now;
                member.EditSid = operatorSid;
                UpdateAsync(member); // 調用基礎 BaseService 的本地快取更新
            }

            return isInsert;
        }

        private void FillMember(DataMember member, string name, string role, string mobile, string email, string iden, DateTime? joinDate, DateTime? continueDate, string? parentSid)
        {
            member.Name = name;
            member.Role = role;
            member.Mobile = mobile;
            member.Email = email;
            member.Iden = iden;
            member.JoinDate = joinDate;
            member.ContinueDate = continueDate;
            member.ParentSid = parentSid;
        }

        private async Task ReportProgressAsync(int current, int total)
        {
            if (current % 10 == 0 || current == total)
            {
                await _hub.Clients.All.SendAsync("ImportProgress", new { current, total });
            }
        }
        #endregion
    }
}