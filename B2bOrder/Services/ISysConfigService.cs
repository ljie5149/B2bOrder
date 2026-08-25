using B2bOrder.Models.Db;
using B2bOrder.Repositories;

namespace B2bOrder.Services
{
    public interface ISysConfigService
    {
        /// <summary>
        /// 取得最原始的系統參數設定實體
        /// </summary>
        Task<SysConfig?> GetConfigByKeyAsync(string configKey);

        /// <summary>
        /// 取得字串型態的參數值（若不存在則回傳預設值）
        /// </summary>
        Task<string> GetStringValueAsync(string configKey, string defaultValue = "");

        /// <summary>
        /// 取得整數型態的參數值（若不存在或轉換失敗則回傳預設值）
        /// </summary>
        Task<int> GetIntValueAsync(string configKey, int defaultValue = 0);

        /// <summary>
        /// 取得布林型態的參數值（支援 true/false, Y/N, 1/0 的動態判斷）
        /// </summary>
        Task<bool> GetBoolValueAsync(string configKey, bool defaultValue = false);

        /// <summary>
        /// 依據分類（Category）取得所有設定
        /// </summary>
        Task<IEnumerable<SysConfig>> GetConfigsByCategoryAsync(string category);

        /// <summary>
        /// 更新特定鍵值的參數內容
        /// </summary>
        Task<bool> UpdateConfigValueAsync(string configKey, string? configValue);

        // =========================================================================
        // 新增：功能模組 (SysModule) 商業邏輯方法，完美對應新版 Controller
        // =========================================================================

        /// <summary>
        /// 取得所有功能模組清單 (依據 SortNo 排序)
        /// </summary>
        Task<IEnumerable<SysModule>> GetAllModulesAsync();

        /// <summary>
        /// 更新單一功能模組的啟用狀態
        /// </summary>
        Task<bool> UpdateModuleStatusAsync(string moduleCode, bool isAvailable);

        /// <summary>
        /// 快速批次操作：全部啟用、全部停用、重設預設
        /// </summary>
        Task<bool> BulkUpdateModulesStatusAsync(string actionType);
    }

    public class SysConfigService : BaseService, ISysConfigService
    {
        private readonly ISysConfigRepository _configRepository;

        public SysConfigService(IServiceProvider serviceProvider, ISysConfigRepository repo) : base(serviceProvider)
        {
            _configRepository = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public async Task<SysConfig?> GetConfigByKeyAsync(string configKey)
        {
            return await _configRepository.GetByKeyAsync(configKey);
        }

        public async Task<string> GetStringValueAsync(string configKey, string defaultValue = "")
        {
            var value = await _configRepository.GetValueByKeyAsync(configKey);
            return value ?? defaultValue;
        }

        public async Task<int> GetIntValueAsync(string configKey, int defaultValue = 0)
        {
            var valueStr = await _configRepository.GetValueByKeyAsync(configKey);
            if (string.IsNullOrWhiteSpace(valueStr)) return defaultValue;

            return int.TryParse(valueStr, out int result) ? result : defaultValue;
        }

        public async Task<bool> GetBoolValueAsync(string configKey, bool defaultValue = false)
        {
            var valueStr = await _configRepository.GetValueByKeyAsync(configKey);
            if (string.IsNullOrWhiteSpace(valueStr)) return defaultValue;

            valueStr = valueStr.Trim().ToUpper();

            // 支援多種常見的布林表示方式
            if (valueStr == "TRUE" || valueStr == "Y" || valueStr == "1" || valueStr == "YES")
                return true;

            if (valueStr == "FALSE" || valueStr == "N" || valueStr == "0" || valueStr == "NO")
                return false;

            return defaultValue;
        }

        public async Task<IEnumerable<SysConfig>> GetConfigsByCategoryAsync(string category)
        {
            return await _configRepository.GetByCategoryAsync(category);
        }

        public async Task<bool> UpdateConfigValueAsync(string configKey, string? configValue)
        {
            // 這裡可以加入額外的商業邏輯檢查，例如權限控管或特定 Key 的格式校驗
            return await _configRepository.UpdateValueByKeyAsync(configKey, configValue);
        }

        // =========================================================================
        // 新增實作方法：將資料存取完全委託給先前建立的 SysConfigRepository
        // =========================================================================

        public async Task<IEnumerable<SysModule>> GetAllModulesAsync()
        {
            // 呼叫 Repository 層獲取已依據 SortNo 排序的功能模組
            return await _configRepository.GetAllModulesAsync();
        }

        public async Task<bool> UpdateModuleStatusAsync(string moduleCode, bool isAvailable)
        {
            if (string.IsNullOrWhiteSpace(moduleCode)) return false;

            // 將布林值轉為資料庫定義的 "Y" 或 "N" 狀態字串
            string availableString = isAvailable ? "Y" : "N";
            return await _configRepository.UpdateModuleAvailableAsync(moduleCode, availableString);
        }

        public async Task<bool> BulkUpdateModulesStatusAsync(string actionType)
        {
            if (string.IsNullOrWhiteSpace(actionType)) return false;

            string targetStatus = "Y";

            switch (actionType.ToUpper())
            {
                case "ALL_ENABLE":
                    targetStatus = "Y";
                    break;
                case "ALL_DISABLE":
                    targetStatus = "N";
                    break;
                case "RESET_DEFAULT":
                    // 實務上在此處可依專案規範規劃更複雜的預設狀態組合
                    targetStatus = "Y";
                    break;
                default:
                    return false;
            }

            return await _configRepository.BulkUpdateModulesAvailableAsync(targetStatus);
        }
    }
}