using B2bOrder.Models;
using B2bOrder.Models.api;
using B2bOrder.Models.Common;
using B2bOrder.Models.Db;
using B2bOrder.Responsitories;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace B2bOrder.Services
{
    public interface ISysUserService : IBaseService
    {
        Task<IEnumerable<SysUser>> GetAllAsync();
        Task<SysUser?> GetByIdAsync(int nid);
        Task<SysUser?> GetBySidAsync(string sid);
        Task<SysUser?> GetByAccountAsync(string account);
        Task<SysUser?> GetByMemberSidAsync(string memberSid);
        Task<SysUser?> Login(LoginViewModel input);
        Task<SysUser?> Register(SysUserViewModel input);

        // 新增：處理忘記密碼並產生 Token 的商業邏輯
        Task<string?> GeneratePasswordResetTokenAsync(string identifier, string callbackUrlTemplate);
        // 新增：處理重設密碼的商業邏輯
        Task<bool> ResetPasswordByTokenAsync(ResetPasswordViewModel model);
        // 新增：處理登出商業邏輯與清除驗證狀態
        Task LogoutAsync();
        // 新增：處理忘記密碼並產生 Token 的商業邏輯
        Task<string?> GeneratePasswordResetTokenAsync(string identifier);
        /// <summary>
        /// 會員自主變更密碼（驗證舊密碼並更新為新密碼）
        /// </summary>
        /// <param name="model">變更密碼的 ViewModel</param>
        /// <exception cref="UnauthorizedAccessException">當舊密碼不正確時拋出</exception>
        /// <exception cref="KeyNotFoundException">找不到系統使用者時拋出</exception>
        Task ChangePasswordAsync(ChangePasswordViewModel model);

        Task<SysUser?> CreateAsync(SysUserViewModel entity);
        Task UpdateAsync(SysUser entity, RequestApiModel4ChangePassword? changePwd = null, bool updatePwd = false);
        Task DeleteAsync(int nid);
        Task SaveChangesAsync();

        /// <summary>
        /// 更新會員最後全部標為已讀的時間戳記
        /// </summary>
        Task<bool> UpdateLastReadAllNoticesTimeAsync(string memberSid, DateTime readAllTime);
    }
    public class SysUserService : BaseService, ISysUserService
    {
        private readonly ISysUserRepository _repo;
        private readonly IMemberService _memberService;
        public SysUserService(IServiceProvider serviceProvider, ISysUserRepository repo, IMemberService memberService) : base(serviceProvider)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _memberService = memberService;
        }

        public async Task<IEnumerable<SysUser>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<SysUser?> GetByMemberSidAsync(string memberSid) => await _repo.GetByMemberSidAsync(memberSid);

        public async Task<SysUser?> GetByIdAsync(int nid) => await _repo.GetByIdAsync(nid);

        public async Task<SysUser?> GetBySidAsync(string sid) => await _repo.GetBySidAsync(sid);

        public async Task<SysUser?> GetByAccountAsync(string account) => await _repo.GetByAccountAsync(account);

        public async Task<SysUser?> Login(LoginViewModel input)
        {
            // 1. 驗證帳號是否存在
            var sysUser = await GetByAccountAsync(input.Mid);
            if (sysUser == null)
            {
                return null;
            }

            // 2. 驗證密碼
            var verify = _passwordHasher.VerifyHashedPassword(sysUser, sysUser.Pwd, input.Password);
            if (verify != PasswordVerificationResult.Success)
            {
                sysUser.Remark = "帳號或密碼錯誤";
                return sysUser;
            }

            // 更新最後登入時間
            sysUser.LastLoginDate = DateTime.UtcNow;
            sysUser.Remark = "";
            await UpdateAsync(sysUser);

            // 4. 寫入系統 Log (沿用你原有的 Log 呼叫方法)
            await WriteLog(
                actionType: (input.isApi) ? ActionTypes.ApiLogin : ActionTypes.Login,
                targetSid: sysUser.MemberSid,
                targetMid: sysUser.Account,
                moduleName: ModuleNames.MemberManager,
                actionName: ActionNames.Login,
                description: $"登入成功：{input.Mid}"
            );
            return sysUser;
        }
        public async Task<SysUser?> Register(SysUserViewModel input)
        {
            var mid = input.account;

            // 檢查帳號是否已存在（data_member.mid / sys_user.account）
            var existsMember = await _memberService.GetByMidAsync(mid);
            var existsSysUser = await GetByAccountAsync(mid);

            // 情況1：會員存在但登入帳號不存在
            if (existsMember != null &&
                existsSysUser == null)
            {
                var resultStep1 = await CreateAsync(
                    new SysUserViewModel
                    {
                        account = mid,
                        password = input.password,
                        memberSid = existsMember.Sid,
                        adminVerify = input.adminVerify
                    }
                );

                await WriteLog(
                    actionType: (input.isApi) ? ActionTypes.ApiRegister : ActionTypes.Register,
                    targetSid: existsMember.Sid,
                    targetMid: existsMember.Mid,
                    moduleName: ModuleNames.MemberManager,
                    actionName: ActionNames.Register,
                    description: $"已是組織一員，註冊成為會員：{mid}({existsMember.Name}) 帳號建立成功"
                );
                resultStep1.Remark = "1";
                return resultStep1;
            }

            // 情況2：會員與帳號都存在
            if (existsMember != null &&
                existsSysUser != null)
            {
                await _memberService.WriteLog(
                    actionType: (input.isApi) ? ActionTypes.ApiRegister : ActionTypes.Register,
                    targetSid: existsMember.Sid,
                    targetMid: existsMember.Mid,
                    moduleName: ModuleNames.MemberManager,
                    actionName: ActionNames.Register,
                    description: $"註冊會員：{mid}({existsMember.Name}) 此會員編號已被使用，無法再次註冊。",
                    "失敗"
                );
                return null;
            }

            // 情況3：完全不存在，建立 DataMember（data_member）
            var dataMember =
                new DataMember
                {
                    Sid = Guid.NewGuid()
                                .ToString("N"),

                    CreateDate = DateTime.UtcNow,

                    Mid = mid,

                    Name = input.name,

                    Email = input.email,

                    Mobile = input.mobile,

                    Iden = input.idNumber,

                    Role = input.role,

                    Avalible = "Y",

                    RegisterStatus = "REGISTERED"
                };

            if (!string.IsNullOrWhiteSpace(input.parentMid))
            {
                var parent =
                    await _memberService.GetByMidAsync(input.parentMid);

                if (parent != null)
                    dataMember.ParentSid = parent.Sid;
            }

            // 建立 data_member
            await _memberService.CreateAsync(
                dataMember
            );

            if (existsMember == null)
            {
                existsMember = await _memberService.GetByMidAsync(mid);
            }

            // 建立 sys_user
            var result = await CreateAsync(
                new SysUserViewModel
                {
                    account = mid,
                    password = input.password,
                    memberSid = existsMember.Sid,
                    adminVerify = input.adminVerify
                }
            );

            await WriteLog(
                actionType: (input.isApi) ? ActionTypes.ApiRegister : ActionTypes.Register,
                targetSid: dataMember.Sid,
                targetMid: dataMember.Mid,
                moduleName: ModuleNames.MemberManager,
                actionName: ActionNames.Register,
                description: $"註冊成功：{mid}({dataMember.Name})。"
            );
            result.Remark = "2";
            return result;
        }
        /// <summary>
        /// 處理忘記密碼邏輯：產生 Token、儲存、並發送重設密碼信件與記錄 Log
        /// </summary>
        /// <param name="identifier">帳號</param>
        /// <param name="callbackUrlTemplate">前端重設密碼網址範本，例如：https://yourdomain/Member/ResetPassword?token={0}</param>
        /// <returns>回傳產生的 Token 字串；若找不到使用者或查無會員信箱則回傳 null</returns>
        public async Task<string?> GeneratePasswordResetTokenAsync(string identifier, string callbackUrlTemplate)
        {
            // 1. 先透過 Email 查詢 data_member，取得會員資訊（主要是為了拿到 member）
            var member = await _memberService.GetByEmailAsync(identifier);

            var sysUser = await GetByAccountAsync(identifier);
            if (member == null)
            {
                if (sysUser == null)
                {
                    return null;
                }

                // 2. 透過 sysUser.MemberSid 取得對應的 data_member 資訊（為了拿 Email 與寫 PushLog）
                member = await _memberService.GetByMidAsync(sysUser.Account);
            }
            else
            {
                sysUser = await GetByAccountAsync(member.Mid);
            }
            // 備註：若 _memberService 沒有 GetByMidAsync，也可以用您的現有方法取得，主要是為了拿到 member.Email

            if (member == null || string.IsNullOrWhiteSpace(member.Email))
            {
                // 找不到會員資料或沒有填寫 Email，記錄系統 Log 後返回
                await WriteLog(
                    actionType: "ForgotPassword",
                    targetSid: member.Sid,
                    targetMid: member.Mid,
                    moduleName: "MemberManager",
                    actionName: "ForgotPassword",
                    description: $"忘記密碼要求失敗：帳號 {identifier} 查無對應會員或 Email 為空。",
                    result: "失敗"
                );
                return null;
            }

            // 3. 產生 32 碼 GUID Token
            var token = Guid.NewGuid().ToString("N");
            sysUser.PasswordResetToken = token;
            sysUser.PasswordResetExpiry = DateTime.UtcNow.AddHours(2);

            // 4. 更新 sys_user 到資料庫
            await UpdateAsync(sysUser);

            // 5. 組裝信件內容與重設連結
            var resetLink = string.Format(callbackUrlTemplate, token);
            var subject = "【系統通知】重設您的帳戶密碼";
            var messageBody = $@"
                                <p>您好：</p>
                                <p>我們收到了您重設密碼的請求。請點擊下方連結以重設您的密碼：</p>
                                <p><a href='{resetLink}' target='_blank'>{resetLink}</a></p>
                                <p>此重設連結將於 2 小時後失效。如果您並未提出此請求，請忽略此信件。</p>
                                <br/>
                                <p>系統自動發送，請勿直接回信。</p>";

            string sendResult = "成功";
            string errMessage = "";

            try
            {
                // 6. 呼叫 BaseService 的 _emailService 發信
                // 註：請依您實作的 IEmailService 方法名稱調整（例如 SendAsync 或 SendEmailAsync）
                await _emailService.SendAsync(member.Email, subject, messageBody);
            }
            catch (Exception ex)
            {
                sendResult = "失敗";
                errMessage = ex.Message;
            }

            // 7. 統一紀錄推播/發信日誌 (WritePushLog)
            await WritePushLog(
                member: member,
                notifyType: "EMAIL",         // 假設通知類型為 EMAIL
                pushType: "PASSWORD_RESET",  // 假設推播/發信類別為密碼重設
                title: subject,
                message: $"已發送重設密碼連結至信箱，Token: {token}",
                result: sendResult,
                responseMessage: errMessage
            );

            // 8. 統一紀錄操作軌跡日誌 (WriteLog)
            await WriteLog(
                actionType: "ForgotPassword",
                targetSid: sysUser.MemberSid,
                targetMid: sysUser.Account,
                moduleName: "MemberManager",
                actionName: "ForgotPassword",
                description: $"使用者要求忘記密碼，已產生 Token 並嘗試發送 Email。結果：{sendResult}",
                result: sendResult,
                moreDescription: errMessage
            );

            return token;
        }
        /// <summary>
        /// 透過 Token 驗證並重設使用者密碼
        /// </summary>
        /// <param name="model">前端傳入的重設密碼 ViewModel</param>
        /// <returns>重設成功回傳 true，Token 無效、過期或失敗回傳 false</returns>
        public async Task<bool> ResetPasswordByTokenAsync(ResetPasswordViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Token)) return false;

            // 1. 修正：從 Repository 透過 Token 找 sys_user (原 Controller 誤用 GetByAccountAsync)
            // 註：若您的 _repo 還沒有這個方法，請在 Repository 加上：FirstOrDefaultAsync(u => u.PasswordResetToken == token)
            var sysUser = await _repo.GetByResetTokenAsync(model.Token);

            // 2. 驗證使用者是否存在、Token 是否過期
            if (sysUser == null || sysUser.PasswordResetExpiry == null || sysUser.PasswordResetExpiry < DateTime.UtcNow)
            {
                if (sysUser != null)
                {
                    await WriteLog(
                        actionType: "ResetPassword",
                        targetSid: sysUser.MemberSid,
                        targetMid: sysUser.Account,
                        moduleName: "MemberManager",
                        actionName: "ResetPassword",
                        description: "重設密碼失敗：Token 已過期。",
                        result: "失敗"
                    );
                }
                return false;
            }

            // 3. 組裝原本 UpdateAsync 所需的參數
            var param = new RequestApiModel4ChangePassword
            {
                memberSid = sysUser.MemberSid,
                resetPasswordData = model
            };

            try
            {
                // 4. 呼叫現有的 UpdateAsync (內含密碼雜湊、Token清空、存檔邏輯)
                await UpdateAsync(sysUser, param, updatePwd: true);

                // 5. 紀錄成功日誌
                await WriteLog(
                    actionType: "ResetPassword",
                    targetSid: sysUser.MemberSid,
                    targetMid: sysUser.Account,
                    moduleName: "MemberManager",
                    actionName: "ResetPassword",
                    description: "使用者透過忘記密碼連結成功重設密碼。",
                    result: "成功"
                );

                return true;
            }
            catch (Exception ex)
            {
                // 紀錄異常日誌
                await WriteLog(
                    actionType: "ResetPassword",
                    targetSid: sysUser.MemberSid,
                    targetMid: sysUser.Account,
                    moduleName: "MemberManager",
                    actionName: "ResetPassword",
                    description: $"重設密碼發生異常：{ex.Message}",
                    result: "失敗"
                );
                return false;
            }
        }
        /// <summary>
        /// 處理會員登出邏輯：自動讀取憑證、記錄操作日誌、並清除 Cookie 驗證狀態
        /// </summary>
        public async Task LogoutAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return;

            // 1. 從當前 HTTP 上下文的使用者憑證 (Claims) 取得帳號與 member_sid
            var account = httpContext.User.FindFirst("account")?.Value;
            var memberSid = httpContext.User.FindFirst("member_sid")?.Value;

            // 2. 寫入登出系統 Log (使用您原有的 ActionTypes, ModuleNames, ActionNames 常量)
            await WriteLog(
                actionType: ActionTypes.Logout,
                targetSid: memberSid,
                targetMid: account,
                moduleName: ModuleNames.MemberManager,
                actionName: ActionNames.Logout,
                description: $"會員登出：{account}({memberSid})"
            );

            // 3. 執行清除 Cookie 驗證狀態 (需引用 Microsoft.AspNetCore.Authentication 與 Microsoft.AspNetCore.Authentication.Cookies)
            await httpContext.SignOutAsync(
                Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme
            );
        }
        /// <summary>
        /// 處理忘記密碼邏輯：若使用者存在，則產生 Token 並儲存。
        /// </summary>
        /// <param name="identifier">帳號或識別碼</param>
        /// <returns>回傳產生的 Token 字串；若找不到使用者則回傳 null</returns>
        public async Task<string?> GeneratePasswordResetTokenAsync(string identifier)
        {
            // 1. 先透過 Email 查詢 data_member，取得會員資訊（主要是為了拿到 member）
            var member = await _memberService.GetByEmailAsync(identifier);

            var sysUser = await GetByAccountAsync(identifier);
            if (member == null)
            {
                if (sysUser == null)
                {
                    return null;
                }

                // 2. 透過 sysUser.MemberSid 取得對應的 data_member 資訊（為了拿 Email 與寫 PushLog）
                member = await _memberService.GetByMidAsync(sysUser.Account);
            }
            else
            {
                sysUser = await GetByAccountAsync(member.Mid);
            }
            // 備註：若 _memberService 沒有 GetByMidAsync，也可以用您的現有方法取得，主要是為了拿到 member.Email

            if (member == null || string.IsNullOrWhiteSpace(member.Email))
            {
                // 找不到會員資料或沒有填寫 Email，記錄系統 Log 後返回
                await WriteLog(
                    actionType: "ForgotPassword",
                    targetSid: member.Sid,
                    targetMid: member.Mid,
                    moduleName: "MemberManager",
                    actionName: "ForgotPassword",
                    description: $"忘記密碼要求失敗：帳號 {identifier} 查無對應會員或 Email 為空。",
                    result: "失敗"
                );
                return null;
            }

            // 3. 產生 32 碼 GUID Token
            var token = Guid.NewGuid().ToString("N");
            sysUser.PasswordResetToken = token;
            sysUser.PasswordResetExpiry = DateTime.UtcNow.AddHours(2);

            // 4. 更新 sys_user 到資料庫
            await UpdateAsync(sysUser);

            // 5. 組裝信件內容與重設連結
            // 假設你有注入 IHttpContextAccessor _httpContextAccessor
            var request = _httpContextAccessor.HttpContext?.Request;
            var baseUrl = $"{request?.Scheme}://{request?.Host}";

            // 組合完整的 ResetPassword 網址
            var resetLink = $"{baseUrl}/Member/ResetPassword/{token}";
            var subject = "【系統通知】重設您的帳戶密碼";
            var messageBody = $@"
                                <p>您好：</p>
                                <p>我們收到了您重設密碼的請求。請點擊下方連結以重設您的密碼：</p>
                                <p><a href='{resetLink}' target='_blank'>{resetLink}</a></p>
                                <p>此重設連結將於 2 小時後失效。如果您並未提出此請求，請忽略此信件。</p>
                                <br/>
                                <p>系統自動發送，請勿直接回信。</p>";

            string sendResult = "成功";
            string errMessage = "";

            try
            {
                // 6. 呼叫 BaseService 的 _emailService 發信
                // 註：請依您實作的 IEmailService 方法名稱調整（例如 SendAsync 或 SendEmailAsync）
                await _emailService.SendAsync(member.Email, subject, messageBody);
            }
            catch (Exception ex)
            {
                sendResult = "失敗";
                errMessage = ex.Message;
            }

            // 7. 統一紀錄推播/發信日誌 (WritePushLog)
            await WritePushLog(
                member: member,
                notifyType: "EMAIL",         // 假設通知類型為 EMAIL
                pushType: "PASSWORD_RESET",  // 假設推播/發信類別為密碼重設
                title: subject,
                message: $"已發送重設密碼連結至信箱，Token: {token}",
                result: sendResult,
                responseMessage: errMessage
            );

            // 8. 統一紀錄操作軌跡日誌 (WriteLog)
            await WriteLog(
                actionType: "ForgotPassword",
                targetSid: sysUser.MemberSid,
                targetMid: sysUser.Account,
                moduleName: "MemberManager",
                actionName: "ForgotPassword",
                description: $"使用者要求忘記密碼，已產生 Token 並嘗試發送 Email。結果：{sendResult}",
                result: sendResult,
                moreDescription: errMessage
            );

            return token;
        }
        /// <summary>
        /// 會員自主變更密碼（驗證舊密碼並更新為新密碼）
        /// </summary>
        public async Task ChangePasswordAsync(ChangePasswordViewModel model)
        {
            // 1. 透過 BaseService 注入的 _httpContextAccessor 自動取得當前登入會員的 member_sid
            var httpContext = _httpContextAccessor.HttpContext;
            var memberSid = httpContext?.User.FindFirst("member_sid")?.Value;

            if (string.IsNullOrEmpty(memberSid))
            {
                throw new UnauthorizedAccessException("使用者未登入。");
            }

            // 2. 修正原本全撈的有效率問題：直接用 memberSid 找到對應的 sys_user
            var sysUser = await _repo.GetByMemberSidAsync(memberSid);
            if (sysUser == null)
            {
                throw new KeyNotFoundException("找不到該會員的系統使用者資料。");
            }

            // 3. 組裝您現有 UpdateAsync 結構所需要的引數架構
            var param = new RequestApiModel4ChangePassword
            {
                memberSid = memberSid,
                changePasswordData = new ChangePasswordViewModel // 配合您現有的 DTO 物件結構
                {
                    OldPassword = model.OldPassword,
                    NewPassword = model.NewPassword
                }
            };

            try
            {
                // 4. 呼叫現有的 UpdateAsync (該方法內已包含舊密碼的 _passwordHasher 驗證、新密碼 Hash 與 SaveChanges)
                await UpdateAsync(sysUser, param, updatePwd: true);

                // 5. 統一紀錄操作軌跡日誌
                await WriteLog(
                    actionType: ActionTypes.ChangePassword,
                    targetSid: memberSid,
                    targetMid: sysUser.Account,
                    moduleName: ModuleNames.MemberManager,
                    actionName: ActionNames.changePassword,
                    description: $"密碼已更新：{sysUser.Account}({memberSid})"
                );
            }
            catch (UnauthorizedAccessException ex)
            {
                // 如果舊密碼驗證失敗，記錄失敗日誌並繼續往外拋讓 Controller 處理 UI
                await WriteLog(
                    actionType: ActionTypes.ChangePassword,
                    targetSid: memberSid,
                    targetMid: sysUser.Account,
                    moduleName: ModuleNames.MemberManager,
                    actionName: ActionNames.changePassword,
                    description: $"密碼更新失敗（舊密碼不正確）：{sysUser.Account}({memberSid})",
                    result: "失敗"
                );
                throw;
            }
        }
        public async Task<SysUser?> CreateAsync(SysUserViewModel input)
        {
            var entity =
                new SysUser
                {
                    Sid = Guid.NewGuid()
                              .ToString("N"),

                    CreateDate = DateTime.UtcNow,

                    Account = input.account,

                    MemberSid = input.memberSid,

                    RegisterVerifyKey =
                        string.IsNullOrWhiteSpace(input.adminVerify)
                        ? null
                        : input.adminVerify,
                    Pwd = input.password,
                    Avalible = "Y"
                };
            entity.Pwd =
                _passwordHasher.HashPassword(
                    entity,
                    entity.Pwd
                );
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return entity;
        }
        public async Task UpdateAsync(SysUser entity, RequestApiModel4ChangePassword? apiChangePwd = null, bool updatePwd = false)
        {
            // 判斷 1：判斷傳入的 sysUser 是否為空
            if (entity == null)
            {
                throw new KeyNotFoundException($"找不到該會員的系統使用者資料。");
            }

            if (updatePwd)
            {
                // 判斷 1：從資料庫撈出目前尚未被修改的原始使用者資料
                var sysUser = await _repo.GetByMemberSidAsync(entity.MemberSid);
                if (sysUser == null)
                {
                    // 找不到資料時，拋出 KeyNotFoundException
                    throw new KeyNotFoundException($"找不到該會員的系統使用者資料 (MemberSid: {entity.MemberSid})。");
                }

                if (apiChangePwd.changePasswordData == null && apiChangePwd.resetPasswordData == null)
                {
                    throw new ArgumentNullException(nameof(apiChangePwd.changePasswordData), "未提供變更密碼所需的資料。");
                }
                string newPasswordPlain = string.Empty;
                if (apiChangePwd.changePasswordData != null)
                {
                    // 變更密碼流程
                    var changePwd = apiChangePwd.changePasswordData;
                    // 判斷 2：驗證舊密碼是否正確
                    // 約定：Controller 已將前端輸入的「舊密碼明文」暫存於 entity.Remark 中傳入
                    string oldPasswordPlain = changePwd.OldPassword;
                    newPasswordPlain = changePwd.NewPassword;

                    var verify = _passwordHasher.VerifyHashedPassword(sysUser, sysUser.Pwd, oldPasswordPlain);
                    if (verify != PasswordVerificationResult.Success)
                    {
                        // 舊密碼不正確時，拋出 UnauthorizedAccessException
                        throw new UnauthorizedAccessException("舊密碼不正確。");
                    }
                }
                else if (apiChangePwd.resetPasswordData != null)
                {
                    // 重設密碼流程
                    var resetPwd = apiChangePwd.resetPasswordData;
                    newPasswordPlain = resetPwd.Password;
                    // 判斷 2：驗證重設密碼的 Token 是否有效
                    if (sysUser.PasswordResetToken != resetPwd.Token || sysUser.PasswordResetExpiry == null || sysUser.PasswordResetExpiry < DateTime.UtcNow)
                    {
                        // Token 無效或過期時，拋出 UnauthorizedAccessException
                        throw new UnauthorizedAccessException("重設密碼的 Token 無效或已過期。");
                    }
                    sysUser.PasswordResetToken = null;
                    sysUser.PasswordResetExpiry = null;
                    entity.PasswordResetToken = null;
                    entity.PasswordResetExpiry = null;
                    entity.PasswordResetToken = "";
                }

                // 判斷通過後：將 entity.Pwd (此時為前端傳入的新密碼明文) 進行雜湊加密
                entity.Pwd = _passwordHasher.HashPassword(entity, newPasswordPlain);
            }

            // 執行原始更新與儲存
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

        /// <summary>
        /// 更新會員最後全部標為已讀的時間戳記
        /// </summary>
        public async Task<bool> UpdateLastReadAllNoticesTimeAsync(string memberSid, DateTime readAllTime)
        {
            if (string.IsNullOrEmpty(memberSid)) return false;

            var sysUser = await _repo.GetByMemberSidAsync(memberSid);
            if (sysUser == null) return false;

            sysUser.LastReadAllNoticesTime = readAllTime;

            await _repo.UpdateAsync(sysUser);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}