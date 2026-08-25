using Microsoft.AspNetCore.Mvc;
using B2bOrder.Services;
using B2bOrder.Models;

namespace B2bOrder.Controllers
{
    public class SysConfigController : Controller
    {
        private readonly ISysConfigService _configService;
        private readonly IWebHostEnvironment _env;

        public SysConfigController(
            ISysConfigService configService,
            IWebHostEnvironment env)
        {
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        // GET: /SysConfig 或 /SysConfig/Index
        public async Task<IActionResult> Index()
        {
            // 1. 建立並填充基本設定資料
            var viewModel = new NoticeViewModel
            {
                SystemName = await _configService.GetStringValueAsync("sys_name", "B2bOrder"),
                SystemTimezone = await _configService.GetStringValueAsync("sys_timezone", "UTC+08:00"),
                SystemLanguage = await _configService.GetStringValueAsync("sys_language", "zh-TW"),
                DateFormat = await _configService.GetStringValueAsync("date_format", "YYYY-MM-DD"),
                TimeFormat = await _configService.GetStringValueAsync("time_format", "24小時制 (HH:mm:ss)"),
                PageSize = await _configService.GetIntValueAsync("page_size", 20),

                Enable2FA = await _configService.GetBoolValueAsync("enable_2fa", false),
                MinPasswordLength = await _configService.GetIntValueAsync("min_password_len", 8),
                PasswordExpiryDays = await _configService.GetIntValueAsync("pwd_expiry_days", 90),
                SessionTimeout = await _configService.GetIntValueAsync("session_timeout", 30),
                LoginFailLockCount = await _configService.GetIntValueAsync("login_fail_lock", 5),
                AccountLockDuration = await _configService.GetIntValueAsync("lock_duration", 15),
                AllowMultiDevice = await _configService.GetBoolValueAsync("allow_multi_device", true),
                EnableIpWhitelist = await _configService.GetBoolValueAsync("enable_ip_whitelist", false),

                CompanyName = await _configService.GetStringValueAsync("company_name", "B2bOrder Co., Ltd."),
                CompanyEmail = await _configService.GetStringValueAsync("company_email", "service@b2border.com"),
                CompanyPhone = await _configService.GetStringValueAsync("company_phone", "02-1234-5678"),
                CompanyAddress = await _configService.GetStringValueAsync("company_address", "台北市信義區信義路五段7號8樓"),
            };

            // 2. 核心修正：呼叫 Service 取得模組清單，灌入同一個主 ViewModel
            var modules = await _configService.GetAllModulesAsync();
            viewModel.Modules = modules.ToList();

            // 3. 設定預設停留的 Tab（若無特別指定則停在基本設定 basic）
            if (ViewBag.ActiveTab == null)
            {
                ViewBag.ActiveTab = "basic";
            }

            return View(viewModel);
        }

        // 儲存基本資訊設定
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveBasicSettings(NoticeViewModel model)
        {
            if (!ModelState.IsValid) return View("Index", model);

            await _configService.UpdateConfigValueAsync("sys_name", model.SystemName);
            await _configService.UpdateConfigValueAsync("sys_timezone", model.SystemTimezone);
            await _configService.UpdateConfigValueAsync("sys_language", model.SystemLanguage);
            await _configService.UpdateConfigValueAsync("date_format", model.DateFormat);
            await _configService.UpdateConfigValueAsync("time_format", model.TimeFormat);
            await _configService.UpdateConfigValueAsync("page_size", model.PageSize.ToString());

            TempData["SuccessMessage"] = "基本資訊設定儲存成功！";
            return RedirectToAction(nameof(Index));
        }

        // 儲存登入與存取設定
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAccessSettings(NoticeViewModel model)
        {
            await _configService.UpdateConfigValueAsync("enable_2fa", model.Enable2FA ? "Y" : "N");
            await _configService.UpdateConfigValueAsync("min_password_len", model.MinPasswordLength.ToString());
            await _configService.UpdateConfigValueAsync("pwd_expiry_days", model.PasswordExpiryDays.ToString());
            await _configService.UpdateConfigValueAsync("session_timeout", model.SessionTimeout.ToString());
            await _configService.UpdateConfigValueAsync("login_fail_lock", model.LoginFailLockCount.ToString());
            await _configService.UpdateConfigValueAsync("lock_duration", model.AccountLockDuration.ToString());
            await _configService.UpdateConfigValueAsync("allow_multi_device", model.AllowMultiDevice ? "Y" : "N");
            await _configService.UpdateConfigValueAsync("enable_ip_whitelist", model.EnableIpWhitelist ? "Y" : "N");

            TempData["SuccessMessage"] = "安全存取設定儲存成功！";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// 功能設定頁面首頁 (GET) - 移除了直接操作 DbContext 的程式碼
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ModuleConfig()
        {
            var viewModel = new ModuleConfigViewModel();

            // 改由呼叫 Service 層方法獲取所有功能模組，維持分層職責
            var moduleList = await _configService.GetAllModulesAsync();
            viewModel.Modules = moduleList.ToList();

            return View(viewModel);
        }

        // 修正：當儲存功能模組狀態後，應該導回 Index 並指定停留在功能設定頁籤 (module)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveModuleConfig(Dictionary<string, string> moduleStates)
        {
            try
            {
                var dbModules = await _configService.GetAllModulesAsync();
                foreach (var dbMod in dbModules)
                {
                    bool isAvailable = moduleStates.TryGetValue(dbMod.ModuleCode, out var state) && state == "Y";
                    await _configService.UpdateModuleStatusAsync(dbMod.ModuleCode, isAvailable);
                }
                TempData["SuccessMessage"] = "功能模組狀態已成功更新！";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"儲存失敗：{ex.Message}";
            }

            // 傳記號給 Index，讓前端自動切換到功能設定
            ViewBag.ActiveTab = "module";
            return await Index();
        }

        /// <summary>
        /// 非同步切換單一功能開關 (API POST - 供前端 Switch 變更時立即以 Ajax 觸發)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ToggleModuleStatus([FromBody] ToggleModuleInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.ModuleCode))
            {
                return Json(new { success = false, message = "無效的參數" });
            }

            // 直接由 Service 完成更新動作，Service 內部會自動回傳成功與否的布林值
            bool isSuccess = await _configService.UpdateModuleStatusAsync(input.ModuleCode, input.IsAvailable);

            if (!isSuccess)
            {
                return Json(new { success = false, message = "找不到指定的功能模組或更新失敗" });
            }

            return Json(new { success = true, message = "狀態已變更" });
        }

        /// <summary>
        /// 快速操作：全部啟用、全部停用、重設預設 (POST)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkAction(string actionType)
        {
            if (string.IsNullOrWhiteSpace(actionType))
            {
                TempData["ErrorMessage"] = "無效的快速操作指令";
                return RedirectToAction(nameof(ModuleConfig));
            }

            // 呼叫 Service 的批次操作封裝
            bool isSuccess = await _configService.BulkUpdateModulesStatusAsync(actionType);

            if (isSuccess)
            {
                TempData["SuccessMessage"] = actionType.ToUpper() switch
                {
                    "ALL_ENABLE" => "已成功啟用所有功能模組！",
                    "ALL_DISABLE" => "已成功停用所有功能模組！",
                    "RESET_DEFAULT" => "已成功將功能模組重設為預設狀態！",
                    _ => "批次操作成功完成！"
                };
            }
            else
            {
                TempData["ErrorMessage"] = "批次更新時發生錯誤";
            }

            return RedirectToAction(nameof(ModuleConfig));
        }
    }

    /// <summary>
    /// 傳入非同步 API 的資料模型 (若您有獨立的模型定義檔案，此結構可移出)
    /// </summary>
    public class ToggleModuleInput
    {
        public string ModuleCode { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
    }
}