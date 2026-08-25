using B2bOrder.Models;
using B2bOrder.Models.Common;
using B2bOrder.Models.Db;
using B2bOrder.Models.Tree;
using B2bOrder.Services;
using B2bOrder.Services.Common;
using MailKit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace B2bOrder.Controllers
{
    public class MemberController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly ISysUserService _sysUserService;
        private readonly IEmailService _mailService;
        private readonly IMemberExpireNoticeService _expireNoticeService;


        private readonly string _module_name = "會員管理";
        public MemberController(
            IMemberService memberService,
            ISysUserService sysUserService,
            IEmailService mailService,
            IMemberExpireNoticeService memberExpireNoticeService
            )
        {
            _memberService  = memberService;
            _sysUserService = sysUserService;
            _expireNoticeService = memberExpireNoticeService;
            _mailService = mailService;
        }

        // 列表 (選用)
        public async Task<IActionResult> Index()
        {
            var memberSid = User.FindFirst("member_sid")?.Value;

            if (string.IsNullOrEmpty(memberSid))
                return RedirectToAction(nameof(Login));

            var login_member =
                await _memberService.GetBySidAsync(memberSid);

            if (login_member == null)
            {
                return Json(new
                {
                    success = false,
                    message = "會員不存在"
                });
            }
            // 1. 取得子會員，並顯式轉型成 DataMember 集合
            var children = await _memberService.GetChildrenAsync(memberSid);

            // 修正點：宣告為更具彈性的 IEnumerable<DataMember>，以便後續進行 LINQ 篩選與排序
            IEnumerable<DataMember> members = children?.Cast<DataMember>() ?? Array.Empty<DataMember>();

            // 2. 轉換為 List 暫時將登入者加進去
            var memberList = members.ToList();
            memberList.Add((DataMember)login_member);
            members = memberList; // 放回集合中繼續做篩選
            return View(members);
        }

        #region GET: 註冊頁
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // POST: 註冊（以 sys_user + data_member 建立）——相容 Register.cshtml 的欄位命名 (Pwd / ConfirmPwd 等)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // 取表單（支援 view 使用 Pwd/ConfirmPwd 或 model 使用 Password/ConfirmPassword）
            var form = Request.HasFormContentType ? Request.Form : null;

            var mid = (model?.Mid ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(mid) && form != null)
                mid = form["Mid"].FirstOrDefault()?.Trim() ?? string.Empty;

            // password fields: 支援 model.Password 或 form["Pwd"]
            var pwd = (model?.Password ?? string.Empty);
            if (string.IsNullOrEmpty(pwd) && form != null)
                pwd = form["Pwd"].FirstOrDefault() ?? string.Empty;

            var confirm = (model?.ConfirmPassword ?? string.Empty);
            if (string.IsNullOrEmpty(confirm) && form != null)
                confirm = form["ConfirmPwd"].FirstOrDefault() ?? string.Empty;

            var name        = model?.Name ?? form?["Name"].FirstOrDefault();
            var email       = model?.Email ?? form?["Email"].FirstOrDefault();
            var mobile      = model?.Mobile ?? form?["Mobile"].FirstOrDefault();
            var parentMid   = model?.ParentMid ?? form?["ParentMid"].FirstOrDefault();
            var adminVerify = model?.AdminVerifyCode ?? form?["AdminVerifyCode"].FirstOrDefault();
            var idNumber    = model?.IdNumber ?? form?["IdNumber"].FirstOrDefault();
            var role        = model?.Role ?? form?["Role"].FirstOrDefault() ?? MemberRoles.Regular;

            if (!MemberRoles.AllRoles.Contains(role))
            {
                ModelState.AddModelError(
                    "Role",
                    $"會員角色錯誤，目前允許：{string.Join("、", MemberRoles.RegisterRoles.Select(MemberRoles.GetName))}"
                );
            }

            // 讀取 JoinDate（支援 model binding 或 form 欄位 "JoinDate"）
            DateTime? joinDate = model?.JoinDate;
            if (!joinDate.HasValue && form != null && form.ContainsKey("JoinDate"))
            {
                var jd = form["JoinDate"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(jd) && DateTime.TryParse(jd, out var parsed))
                    joinDate = parsed;
            }

            bool agreeTerms = false;
            if (model?.AgreeTerms != null)
                agreeTerms = model.AgreeTerms == true;
            else if (form != null)
                agreeTerms = form.ContainsKey("AgreeTerms");

            // 伺服器端基礎驗證（以 view 欄位名稱為主以便錯誤訊息顯示）
            if (string.IsNullOrWhiteSpace(mid))
                ModelState.AddModelError("Mid", "請輸入會員編號。");

            if (string.IsNullOrEmpty(pwd))
                ModelState.AddModelError("Pwd", "請輸入密碼。");

            if (string.IsNullOrEmpty(confirm))
                ModelState.AddModelError("ConfirmPwd", "請再次輸入密碼。");

            if (!string.IsNullOrEmpty(pwd) && !string.IsNullOrEmpty(confirm) && !string.Equals(pwd, confirm, StringComparison.Ordinal))
                ModelState.AddModelError(string.Empty, "兩次密碼輸入不相符。");

            if (!agreeTerms)
                ModelState.AddModelError("AgreeTerms", "請同意隱私權政策與使用條款。");

            if (!ModelState.IsValid)
            {
                // 回填非敏感欄位給 view 的 model（不要回填密碼）
                var vm = new RegisterViewModel
                {
                    Mid = mid,
                    Name = name,
                    Email = email,
                    Mobile = mobile,
                    ParentMid = parentMid,
                    AdminVerifyCode = adminVerify,
                    IdNumber = idNumber,
                    AgreeTerms = agreeTerms
                };
                return View(vm);
            }

            var res = await _sysUserService.Register(
                            new SysUserViewModel
                            {
                                account = mid,
                                password = pwd,
                                adminVerify = adminVerify,
                                parentMid = parentMid,
                                name = name,
                                email = email,
                                mobile = mobile,
                                idNumber = idNumber,
                                role = role,
                                isApi = false
                            });

            // 情況1：會員存在但登入帳號不存在
            if (res.Remark == "1")
            {
                TempData["RegisterAccount"] = mid;
                TempData["RegisterEmail"]   = email;

                TempData["Success"] =
                    "帳號建立成功";

                return RedirectToAction(
                    nameof(RegisterSuccess)
                );
            }

            // 情況2：會員與帳號都存在
            if (res == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "此會員編號已被使用。"
                );

                return View(model);
            }

            //if (res.Remark == "2")
            {
                TempData["RegisterAccount"] = mid;
                TempData["RegisterEmail"] = email;
                TempData["Success"] = "註冊成功";

                return RedirectToAction(
                    nameof(RegisterSuccess)
                );
            }
        }

        // GET: 註冊成功頁面
        [HttpGet]
        public IActionResult RegisterSuccess()
        {
            ViewBag.Account = TempData["RegisterAccount"]?.ToString();
            ViewBag.Email = TempData["RegisterEmail"]?.ToString();

            return View();
        }
        #endregion

        #region GET: 登入頁
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        // POST: 登入（以 sys_user 驗證）
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            model.isApi = false; // 標記為 一般 登入
            var sysUser = await _sysUserService.Login(model);
            if (sysUser == null)
            {
                ModelState.AddModelError(string.Empty, "帳號或密碼錯誤。");
                return View(model);
            }

            if (sysUser.Remark == "帳號或密碼錯誤")
            {
                ModelState.AddModelError(string.Empty, "帳號或密碼錯誤。");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, sysUser.Account),
                new Claim("account", sysUser.Account),
                new Claim("member_sid", sysUser.MemberSid ?? string.Empty)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var props = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe
            };

            if (model.RememberMe)
            {
                // 設定持久 cookie 的有效期限（可調整天數）
                props.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30);
            }

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

            //if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            //    return Redirect(returnUrl);

            // 已登入成功，導向 LoginSuccess 頁面
            //return RedirectToAction(nameof(LoginSuccess));

            return RedirectToAction("Index", "Home");
        }

        // GET: 登入成功頁面
        [HttpGet]
        public IActionResult LoginSuccess()
        {
            return View();
        }
        #endregion

        #region GET: 忘記密碼
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }
        // POST: 忘記密碼 (產生 token 存於 sys_user，實務應 email 發送)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {

            if (string.IsNullOrWhiteSpace(model.AccountValue))
            {
                ModelState.AddModelError(nameof(model.AccountValue), "請輸入會員編號或電子信箱。");
            }

            if (!ModelState.IsValid)
                return View(model);

            // 1. 產生重設 Token
            var token = await _sysUserService.GeneratePasswordResetTokenAsync(model.AccountValue);

            //if (token != null)
            //{
            //    // 2. 透過 EmailService 寄出郵件
            //    // 假設你的系統可透過 model.AccountValue 取得 Email 及重設網址
            //    var request = HttpContext.Request;
            //    var resetLink = $"{request.Scheme}://{request.Host}/Member/ResetPassword/{token}";

            //    var subject = "【系統通知】重設您的帳戶密碼";
            //    var messageBody = $@"
            //                    <p>您好：</p>
            //                    <p>請點擊下方連結以重設您的密碼：</p>
            //                    <p><a href='{resetLink}' target='_blank'>{resetLink}</a></p>
            //                    <p>此連結將於 30 分鐘後失效。</p>";

            //    // 呼叫寄信服務 (依你的 _mailService 介面調整)
            //    await _mailService.SendAsync(model.AccountValue, subject, messageBody);
            //}

            // 3. 將 AccountValue 透過 TempData 或 Query String 帶到 Confirmation 頁面
            TempData["AccountValue"] = model.AccountValue;

            // 安全性考量：不暴露帳號是否存在
            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        #endregion

        #region GET: 顯示重設密碼頁（token 必要）
        // 支援 Query string (?token=xxx) 或 Route 網址 (/ResetPassword/xxx)
        [HttpGet("Member/ResetPassword/{token}")]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
                return RedirectToAction(nameof(ForgotPassword));

            var vm = new ResetPasswordViewModel { Token = token };
            return View(vm);
        }
        #endregion

        #region POST: 重設密碼（以 token 尋找 sys_user 並更新 pwd）
        // POST: 重設密碼
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 呼叫商業邏輯層進行驗證與變更密碼
            bool isSuccess = await _sysUserService.ResetPasswordByTokenAsync(model);

            if (!isSuccess)
            {
                ModelState.AddModelError(string.Empty, "重設密碼失敗（token 無效或已過期）。");
                return View(model);
            }

            // 成功後導向完成頁面
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        #endregion

        #region POST: 登出
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // 呼叫 Service 處理日誌與登出
            await _sysUserService.LogoutAsync();

            // 導向首頁
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region GET: 請求續約
        [HttpGet]
        public async Task<IActionResult> RenewalRequest()
        {
            var memberSid = User.FindFirst("member_sid")?.Value;
            if (string.IsNullOrEmpty(memberSid))
                return RedirectToAction(nameof(Login));

            var member = await _memberService.GetBySidAsync(memberSid);
            if (member == null)
                return NotFound();

            var vm = new MemberCenterViewModel
            {
                Sid = member.Sid,
                Mid = member.Mid,
                Name = member.Name,
                Email = member.Email,
                Mobile = member.Mobile,
                Tel = member.Tel,
                Address = member.Address,
                Birthday = member.Birthday,
                JoinDate = member.JoinDate,
                ContinueDate = member.ContinueDate
            };

            return View(vm);
        }
        #endregion

        #region POST: 提交續約請求
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RenewalRequest(MemberCenterViewModel model)
        {
            // 1. 檢查 Model 基本驗證
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 2. 呼叫 Service 執行續約、資料更新與 Log
            bool isSuccess = await _memberService.ProcessRenewalRequestAsync(model);

            if (!isSuccess)
            {
                // 若回傳 false 代表未登入（或憑證失效），導回登入頁（或回傳 NotFound）
                return RedirectToAction(nameof(Login));
            }

            // 3. 設定成功提示並重新導向（PRG 模式防止重複整理提交）
            TempData["Success"] = "續約請求已送出！";
            return RedirectToAction(nameof(RenewalRequest));
        }
        #endregion

        #region GET: 個人資料管理
        [HttpGet]
        public async Task<IActionResult> Center()
        {
            var memberSid = User.FindFirst("member_sid")?.Value;
            if (string.IsNullOrEmpty(memberSid))
                return RedirectToAction(nameof(Login));

            var member = await _memberService.GetBySidAsync(memberSid);
            if (member == null)
                return NotFound();

            var vm = new MemberCenterViewModel
            {
                Sid = member.Sid,
                Mid = member.Mid,
                Name = member.Name,
                Email = member.Email,
                Mobile = member.Mobile,
                Tel = member.Tel,
                Address = member.Address,
                Birthday = member.Birthday,
                JoinDate = member.JoinDate,
                ContinueDate = member.ContinueDate
            };

            return View(vm);
        }
        #endregion

        #region POST: 更新個人資料（包含 ContinueDate，使用 MemberCenterViewModel）
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Center(MemberCenterViewModel model)
        {
            // 1. 檢查 Model 基本驗證
            if (!ModelState.IsValid)
            {
                // 回傳時保留前端輸入（包含 ContinueDate）
                return View(model);
            }

            // 2. 呼叫 Service 執行資料更新、時區轉換與 Log
            bool isSuccess = await _memberService.UpdateProfileAsync(model);

            if (!isSuccess)
            {
                // 若回傳 false 代表未登入或憑證失效（原邏輯為導回登入頁或 NotFound）
                // 為了嚴謹，您亦可在 Service 細分原因，此處依常規處理：
                var memberSid = User.FindFirst("member_sid")?.Value;
                if (string.IsNullOrEmpty(memberSid))
                    return RedirectToAction(nameof(Login));

                return NotFound();
            }

            // 3. 設定成功提示，並以 PRG 模式重新導向至 GET Action，避免整理重複提交
            TempData["Success"] = "個人資料已更新。";
            return RedirectToAction(nameof(Center));
        }
        #endregion

        #region POST: 變更密碼（由 Center 頁面的 modal 提交）
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var memberSid = User.FindFirst("member_sid")?.Value;
            if (string.IsNullOrEmpty(memberSid))
                return RedirectToAction(nameof(Login));

            // 1. 如果欄位基本驗證不通過，直接走重新載入頁面邏輯
            if (!ModelState.IsValid)
            {
                return await ReturnCenterViewWithModal(memberSid, model);
            }

            try
            {
                // 2. 呼叫 Service 執行密碼變更核心邏輯（內含舊密碼檢查、更新、Log）
                await _sysUserService.ChangePasswordAsync(model);

                TempData["Success"] = "密碼已更新。";
                return RedirectToAction(nameof(Center));
            }
            catch (UnauthorizedAccessException)
            {
                // 舊密碼不正確，塞入錯誤訊息並顯示 Modal
                ModelState.AddModelError("OldPassword", "舊密碼不正確。");
                return await ReturnCenterViewWithModal(memberSid, model);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// 提取重複的 UI 封裝邏輯：當驗證失敗時，重新載入 Center View 並開啟變更密碼 Modal
        /// </summary>
        private async Task<IActionResult> ReturnCenterViewWithModal(string memberSid, ChangePasswordViewModel model)
        {
            var memberForInvalid = await _memberService.GetBySidAsync(memberSid);
            var vmInvalid = new MemberCenterViewModel
            {
                Sid = memberForInvalid?.Sid,
                Mid = memberForInvalid?.Mid,
                Name = memberForInvalid?.Name,
                Email = memberForInvalid?.Email,
                Mobile = memberForInvalid?.Mobile,
                Tel = memberForInvalid?.Tel,
                Address = memberForInvalid?.Address,
                Birthday = memberForInvalid?.Birthday
            };

            ViewBag.ShowChangePasswordModal = true;
            ViewData["ChangePasswordModel"] = model;
            return View("Center", vmInvalid);
        }
        #endregion

        #region GET: 組織圖
        [HttpGet]
        public async Task<IActionResult> Organization()
        {
            var memberSid = User.FindFirst("member_sid")?.Value;
            if (string.IsNullOrEmpty(memberSid))
                return RedirectToAction(nameof(Login));

            // 1. 呼叫 Service 一鍵取得處理完畢（含人數加總）的完整組織樹
            var rootNode = await _memberService.GetOrganizationTreeAsync(memberSid);

            if (rootNode == null)
                return NotFound();

            // 2. 組裝前端需要的頁面 ViewModel
            var model = new MemberTreePageViewModel
            {
                Tree = rootNode,
                CreateMember = new CreateMemberViewModel(),
                EditMember = new EditMemberViewModel()
            };

            // 3. 組裝彈出視窗（Modal）所需的資料
            ViewBag.ModalModel = new MemberModalViewModel
            {
                CreateMember = model.CreateMember,
                EditMember = model.EditMember
            };

            return View(model);
        }
        #endregion

        #region POST: 刪除會員
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] DeleteMemberViewModel model)
        {
            // 呼叫 Service 處理所有複雜的樹狀重組與軟刪除邏輯
            var (success, message) = await _memberService.DeleteMemberAsync(model);

            if (!success)
            {
                return Json(new { success = false, message });
            }

            return Json(new { success = true });
        }
        #endregion

        #region 新增下線會員 (由組織圖的 modal 提交)
        // GET: 載入新增會員頁面 (或 Modal)
        [HttpGet]
        public async Task<IActionResult> CreateMember(string sid)
        {
            var vm = await _memberService.PrepareCreateViewModelAsync(sid);

            if (vm == null)
                return NotFound();

            return View(vm);
        }

        // POST: 建立新會員
        [HttpPost]
        public async Task<IActionResult> CreateMember([FromBody] CreateMemberViewModel model)
        {
            // 1. UI 驗證不通過，封裝並回傳格式化錯誤訊息字典
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState.Where(x => x.Value.Errors.Any())
                                       .ToDictionary(
                                           x => x.Key,
                                           x => x.Value.Errors.First().ErrorMessage
                                       )
                });
            }

            // 2. 呼叫 Service 執行核心商業驗證、建置、寫入資料庫與紀錄 Log
            var (success, message) = await _memberService.CreateChildMemberAsync(model);

            if (!success)
            {
                return Json(new { success = false, message });
            }

            return Json(new { success = true });
        }
        #endregion

        #region 編輯會員 (由組織圖的 modal 提交)
        // GET: 獲取編輯會員的 JSON 資料
        [HttpGet]
        public async Task<IActionResult> EditMember(string sid)
        {
            var data = await _memberService.GetMemberForEditAsync(sid);

            if (data == null)
                return NotFound();

            return Json(data);
        }

        // POST: 儲存編輯會員
        [HttpPost]
        public async Task<IActionResult> EditMember([FromBody] EditMemberViewModel model)
        {
            // 1. UI 驗證不通過，封裝並回傳格式化錯誤訊息字典
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState.Where(x => x.Value.Errors.Any())
                                       .ToDictionary(
                                           x => x.Key,
                                           x => x.Value.Errors.First().ErrorMessage
                                       )
                });
            }

            // 2. 呼叫 Service 執行核心商業邏輯（包含檢查、更新、Log）
            var (success, message) = await _memberService.UpdateChildMemberAsync(model);

            if (!success)
            {
                return Json(new { success = false, message });
            }

            return Json(new { success = true });
        }
        #endregion

        #region 取得會員詳細資料
        /// <summary>
        /// 取得會員詳細資料 (供 AJAX 檢視使用)
        /// </summary>
        /// <param name="sid">會員安全識別碼</param>
        [HttpGet]
        public async Task<IActionResult> GetMemberDetail(string sid)
        {
            if (string.IsNullOrEmpty(sid))
            {
                return Json(new { success = false, message = "參數錯誤：無效的會員識別碼" });
            }

            try
            {
                // 1. 根據 sid 撈取會員資料（請替換為您專案實際的資料撈取邏輯）
                var member = await _memberService.GetBySidAsync(sid);

                if (member == null)
                {
                    return Json(new { success = false, message = "找不到該會員資料" });
                }

                string ParentSid = member.ParentSid ?? "", ParentMid = "";
                DataMember ParentMember = null;
                if (!string.IsNullOrEmpty(ParentSid))
                {
                    ParentMember = await _memberService.GetBySidAsync(ParentSid);
                }
                if (ParentMember != null) ParentMid = ParentMember.Mid ?? "";

                // 2. 包裝成前端需要的資料結構
                var resultData = new
                {
                    mid = member.Mid,
                    parentMid = ParentMid,
                    name = member.Name,
                    mobile = member.Mobile,
                    email = member.Email,
                    idNumber = member.Iden,
                    // 轉成 ISO 8601 字串格式 (yyyy-MM-ddTHH:mm:ss)，方便前端 JS 用 split('T')[0] 切出日期
                    joinDate = member.JoinDate?.ToString("yyyy-MM-ddTHH:mm:ss"),
                    continueDate = member.ContinueDate?.ToString("yyyy-MM-ddTHH:mm:ss"),
                    // 呼叫您現有的靜態方法，將 Role Enum/int 轉換為角色中文名稱
                    roleName = MemberRoles.GetName(member.Role)
                };

                return Json(new { success = true, data = resultData });
            }
            catch (Exception ex)
            {
                // 實際專案建議加上 Logger 記錄錯誤
                // _logger.LogError(ex, "取得會員資料時發生異常");
                return Json(new { success = false, message = "系統讀取資料時發生錯誤" });
            }
        }
        #endregion

        #region 續約會員 (由組織圖的 modal 提交)
        public class RenewMemberViewModel
        {
            public string Sid { get; set; }
        }
        // POST: 會員續約
        [HttpPost]
        public async Task<IActionResult> RenewMember([FromBody] RenewMemberViewModel model)
        {
            // 呼叫 Service 執行核心商業驗證、續約狀態變更與紀錄 Log
            var (success, message) = await _memberService.RenewMemberAsync(model);

            if (!success)
            {
                return Json(new { success = false, message });
            }

            return Json(new { success = true });
        }
        #endregion

        #region 會員列表（含搜尋與分頁）
        // GET: 會員清單列表
        [HttpGet]
        public async Task<IActionResult> List(
            string keyword = "",
            string status = "",
            string sortField = "mid",
            string sortDir = "asc",
            int page = 1)
        {
            var memberSid = User.FindFirst("member_sid")?.Value;
            if (string.IsNullOrEmpty(memberSid))
                return RedirectToAction(nameof(Login));

            const int pageSize = SysDefine.ShowRecordsPerPage;

            // 1. 呼叫 Service 一鍵取得所有經過遞迴、篩選、排序、分頁後的完好資料與總數
            var pagedResult = await _memberService.GetPagedMemberListAsync(keyword, status, sortField, sortDir, page, pageSize);

            if (pagedResult == null)
            {
                // 若找不到登入會員，直接回傳 Json
                return Json(new { success = false, message = "會員不存在" });
            }

            // 2. 狀態帶回前端（維持 UI 的分頁及搜尋控制狀態欄位）
            ViewBag.Keyword = keyword;
            ViewBag.Status = status;
            ViewBag.SortField = sortField;
            ViewBag.SortDir = sortDir;
            ViewBag.Page = page;
            ViewBag.Total = pagedResult.TotalCount;
            ViewBag.PageSize = pageSize;

            // 3. 組合最後畫面所需的 ViewModel
            var model = new MemberListViewModel
            {
                Members = pagedResult.Data.ToList(),
                EditMember = new EditMemberViewModel()
            };

            return View(model);
        }
        #endregion

        #region 匯出會員資料（CSV 或 Excel）
        // GET: 匯出會員資料 (CSV / Excel)
        [HttpGet]
        public async Task<IActionResult> Export(
            string type,
            string scope,
            string keyword = "",
            string status = "")
        {
            // 檢查登入狀態（依您前面 Action 的安全規範，此處可選擇保留或由 Filter 處理）
            var memberSid = User.FindFirst("member_sid")?.Value;
            if (string.IsNullOrEmpty(memberSid))
                return RedirectToAction(nameof(Login));

            // 1. 呼叫 Service 執行篩選、快取優化與二進位檔案製程
            var (fileBytes, contentType, fileName) = await _memberService.ExportMembersAsync(memberSid, type, scope, keyword, status);

            // 2. 直接以 File 格式輸出供瀏覽器下載
            return File(fileBytes, contentType, fileName);
        }
        #endregion

        #region 匯入會員資料（CSV 或 Excel）
        // POST: 會員批次匯入 (CSV / Excel)
        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            // 1. 驗證登入身分
            var memberSid = User.FindFirst("member_sid")?.Value;
            if (string.IsNullOrEmpty(memberSid))
                return RedirectToAction(nameof(Login));

            var loginMember = await _memberService.GetBySidAsync(memberSid);
            if (loginMember == null)
            {
                return Json(new { success = false, message = "會員不存在" });
            }

            // 2. 驗證前端上傳檔案
            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "請選擇檔案" });
            }

            var ext = Path.GetExtension(file.FileName).ToLower();
            if (ext != ".csv" && ext != ".xlsx")
            {
                return Json(new { success = false, message = "不支援的檔案格式" });
            }

            // 3. 呼交 Service 封裝的核心匯入製程 (包含檔案串流解析、效能快取、進度即時推播、軌跡 Log)
            var result = await _memberService.ImportMembersAsync(file, loginMember.Sid, loginMember.Mid);

            // 4. 回傳處理報告
            return Json(new
            {
                success = true,
                insertCount = result.InsertCount,
                updateCount = result.UpdateCount,
                skipCount = result.SkipCount,
                errors = result.Errors
            });
        }
        #endregion

        #region 會員到期通知列表（含搜尋與分頁）
        // GET: 獲取會員到期狀態與通知紀錄看板
        [HttpGet]
        public async Task<IActionResult> ExpireNotice(
            string tab = "member",
            string keyword = "",
            string sortField = "continueDate",
            string sortDir = "asc",
            int page = 1)
        {
            var memberSid = User.FindFirst("member_sid")?.Value;
            if (string.IsNullOrEmpty(memberSid))
                return RedirectToAction(nameof(Login));

            const int pageSize = SysDefine.ShowRecordsPerPage;

            // 1. 呼叫 Service 一指搞定複雜的核心計算、分流、多條件篩選與分頁
            var model = await _expireNoticeService.GetExpireNoticePageDataAsync(
                memberSid, tab, keyword, sortField, sortDir, page, pageSize);

            if (model == null)
            {
                return Json(new { success = false, message = "會員不存在" });
            }

            // 2. 狀態帶回前端 UI 維持排版控制
            ViewBag.Tab = tab;
            ViewBag.Keyword = keyword;
            ViewBag.SortField = sortField;
            ViewBag.SortDir = sortDir;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Total = model.TotalCount; // 從組裝好的 Model 直接取出對應分流的總筆數

            return View(model);
        }
        #endregion

        #region 手動發送到期通知
        [HttpPost]
        public async Task<IActionResult> SendLineNotice(string sid)
        {
            if (string.IsNullOrEmpty(sid))
            {
                return Json(new { success = false, message = "無效的會員識別碼" });
            }

            // 一指呼叫封裝完整的服務層
            var result = await _expireNoticeService.SendManualLineNoticeAsync(sid);

            return Json(new
            {
                success = result.Success,
                message = result.Message
            });
        }
        #endregion

        #region FCM 發送到期通知
        [HttpPost]
        public async Task<IActionResult> SendFcmNotice(string sid)
        {
            var (success, message) = await _expireNoticeService.SendManualFcmNoticeAsync(sid);

            if (!success)
            {
                return Json(new { success = false, message });
            }

            return Json(new { success = true });
        }
        #endregion

        #region Email 發送到期通知
        [HttpPost]
        public async Task<IActionResult> SendEmailNotice(string sid)
        {
            var (success, message) = await _expireNoticeService.SendManualEmailNoticeAsync(sid);

            if (!success)
            {
                return Json(new { success = false, message });
            }

            return Json(new { success = true });
        }
        #endregion

        #region 批次發送到期通知
        [HttpPost]
        public async Task<IActionResult> BatchLineNotice([FromBody] List<string> sids)
        {
            var (success, successCount, failureCount) = await _expireNoticeService.SendBatchLineNoticeAsync(sids);

            return Json(new
            {
                success = success,
                count = successCount,
                error_count = failureCount
            });
        }
        #endregion

        #region 批次 FCM 發送到期通知
        [HttpPost]
        public async Task<IActionResult> BatchFcmNotice([FromBody] List<string> sids)
        {
            var (success, successCount, failureCount) = await _expireNoticeService.SendBatchFcmNoticeAsync(sids);

            return Json(new
            {
                success = success,
                count = successCount,
                error_count = failureCount
            });
        }
        #endregion

        #region 批次 Email 發送到期通知
        [HttpPost]
        public async Task<IActionResult> BatchEmailNotice([FromBody] List<string> sids)
        {
            var (success, successCount, failureCount) = await _expireNoticeService.SendBatchEmailNoticeAsync(sids);

            return Json(new
            {
                success = success,
                count = successCount,
                error_count = failureCount
            });
        }
        #endregion

        #region 匯出到期會員清單
        [HttpGet]
        public async Task<IActionResult> ExportExpireNotice(string type = "excel", string keyword = "")
        {
            var memberSid = User.FindFirst("member_sid")?.Value;
            if (string.IsNullOrEmpty(memberSid))
                return RedirectToAction(nameof(Login));

            var fileModel = await _expireNoticeService.ExportExpireNoticeFileAsync(memberSid, type, keyword);
            return File(
                fileModel.FileContents,
                fileModel.ContentType,
                fileModel.FileName
            );
        }
        #endregion

        #region 匯出到期通知紀錄
        [HttpGet]
        public async Task<IActionResult> ExportExpireHistory(string type = "excel", string keyword = "")
        {
            var memberSid = User.FindFirst("member_sid")?.Value;

            if (string.IsNullOrEmpty(memberSid))
                return RedirectToAction(nameof(Login));

            var fileModel = await _expireNoticeService.ExportExpireHistoryFileAsync(memberSid, type, keyword);

            return File(
                fileModel.FileContents,
                fileModel.ContentType,
                fileModel.FileName
            );
        }
        #endregion
    }
}