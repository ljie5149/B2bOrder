using B2bOrder.Data;
using B2bOrder.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace B2bOrder.Repositories
{
    public interface ISysConfigRepository
    {
        // ==========================================
        // 1. 原有的 SysConfig (系統參數) 操作
        // ==========================================
        Task<SysConfig?> GetByKeyAsync(string configKey);
        Task<string?> GetValueByKeyAsync(string configKey);
        Task<IEnumerable<SysConfig>> GetByCategoryAsync(string category);
        Task<IEnumerable<SysConfig>> GetAllAsync();
        Task SaveConfigAsync(SysConfig config);
        Task<bool> UpdateValueByKeyAsync(string configKey, string? configValue);

        // ==========================================
        // 2. SysModule (功能模組) 操作
        // ==========================================
        Task<IEnumerable<SysModule>> GetAllModulesAsync();
        Task<SysModule?> GetModuleByCodeAsync(string moduleCode);
        Task<bool> UpdateModuleAvailableAsync(string moduleCode, string available);
        Task<bool> BulkUpdateModulesAvailableAsync(string available);

        // ==========================================
        // 3. 通知設定 (Event / Channel / Setting)
        // ==========================================
        Task<IEnumerable<SysNoticeEvent>> GetNoticeEventsWithChannelsAsync();
        Task<IEnumerable<SysNoticeSetting>> GetAllNoticeSettingsAsync();
        Task<bool> UpdateNoticeEventChannelsAsync(string eventCode, List<string> activeChannels, string receiverType);
        Task<bool> UpdateNoticeSettingAsync(SysNoticeSetting setting);

        // ==========================================
        // 4. SysIntegration (第三方服務整合) 操作
        // ==========================================
        Task<IEnumerable<SysIntegration>> GetAllIntegrationsAsync();
        Task<IEnumerable<SysIntegrationConfig>> GetIntegrationConfigsByServiceAsync(string serviceCode);
        Task<bool> UpdateIntegrationStatusAsync(string serviceCode, string status);
        Task<bool> SaveIntegrationConfigsAsync(string serviceCode, Dictionary<string, string?> configs);

        // ==========================================
        // 5. SysIpWhitelist (IP 白名單) 操作
        // ==========================================
        Task<IEnumerable<SysIpWhitelist>> GetActiveIpWhitelistAsync();
        Task<bool> AddIpToWhitelistAsync(SysIpWhitelist ipItem);
        Task<bool> DeleteIpFromWhitelistAsync(int nid);

        // ==========================================
        // 6. 系統排程工作 (Job / JobLog) 操作
        // ==========================================
        Task<IEnumerable<SysJob>> GetAllJobsAsync();
        Task<bool> UpdateJobStatusAsync(string jobCode, string available);
        Task<bool> UpdateJobExecutionTimeAsync(string jobCode, DateTime? lastRun, DateTime? nextRun);
        Task AddJobLogAsync(SysJobLog log);
    }
    public class SysConfigRepository : ISysConfigRepository
    {
        private readonly ApplicationDbContext _context;

        public SysConfigRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // ==========================================
        // 1. 原有的 SysConfig (系統參數) 操作
        // ==========================================

        public async Task<SysConfig?> GetByKeyAsync(string configKey)
        {
            if (string.IsNullOrWhiteSpace(configKey)) return null;
            return await _context.SysConfigs.FirstOrDefaultAsync(c => c.ConfigKey == configKey);
        }

        public async Task<string?> GetValueByKeyAsync(string configKey)
        {
            if (string.IsNullOrWhiteSpace(configKey)) return null;
            return await _context.SysConfigs
                .Where(c => c.ConfigKey == configKey)
                .Select(c => c.ConfigValue)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<SysConfig>> GetByCategoryAsync(string category)
        {
            if (string.IsNullOrWhiteSpace(category)) return Enumerable.Empty<SysConfig>();
            return await _context.SysConfigs
                .Where(c => c.Category == category)
                .ToListAsync();
        }

        public async Task<IEnumerable<SysConfig>> GetAllAsync()
        {
            return await _context.SysConfigs.ToListAsync();
        }

        public async Task SaveConfigAsync(SysConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            var existing = await GetByKeyAsync(config.ConfigKey);
            if (existing == null)
            {
                config.CreateDate = DateTime.Now;
                await _context.SysConfigs.AddAsync(config);
            }
            else
            {
                existing.ConfigName = config.ConfigName;
                existing.ConfigValue = config.ConfigValue;
                existing.ConfigType = config.ConfigType;
                existing.Category = config.Category;
                existing.Script = config.Script;
                existing.ModifyDate = DateTime.Now;

                _context.SysConfigs.Update(existing);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateValueByKeyAsync(string configKey, string? configValue)
        {
            if (string.IsNullOrWhiteSpace(configKey)) return false;

            var config = await GetByKeyAsync(configKey);
            if (config == null) return false;

            config.ConfigValue = configValue;
            config.ModifyDate = DateTime.Now;
            _context.SysConfigs.Update(config);

            return await _context.SaveChangesAsync() > 0;
        }

        // ==========================================
        // 2. SysModule (功能模組設定)
        // ==========================================

        public async Task<IEnumerable<SysModule>> GetAllModulesAsync()
        {
            // 依據 SortNo 排序輸出功能清單
            return await _context.SysModules.OrderBy(m => m.SortNo).ToListAsync();
        }

        public async Task<SysModule?> GetModuleByCodeAsync(string moduleCode)
        {
            if (string.IsNullOrWhiteSpace(moduleCode)) return null;
            return await _context.SysModules.FirstOrDefaultAsync(m => m.ModuleCode == moduleCode);
        }

        public async Task<bool> UpdateModuleAvailableAsync(string moduleCode, string available)
        {
            var module = await GetModuleByCodeAsync(moduleCode);
            if (module == null) return false;

            module.Avalible = available;
            module.ModifyDate = DateTime.Now;
            _context.SysModules.Update(module);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> BulkUpdateModulesAvailableAsync(string available)
        {
            var modules = await _context.SysModules.ToListAsync();
            foreach (var m in modules)
            {
                m.Avalible = available;
                m.ModifyDate = DateTime.Now;
            }
            _context.SysModules.UpdateRange(modules);
            return await _context.SaveChangesAsync() > 0;
        }

        // ==========================================
        // 3. 通知設定 (Event / Channel / Setting)
        // ==========================================

        public async Task<IEnumerable<SysNoticeEvent>> GetNoticeEventsWithChannelsAsync()
        {
            // 同時 Include 管道設定導覽屬性，供設定表格勾選狀態呈現
            return await _context.SysNoticeEvents
                .Include(e => e.NoticeChannels)
                .ToListAsync();
        }

        public async Task<IEnumerable<SysNoticeSetting>> GetAllNoticeSettingsAsync()
        {
            return await _context.SysNoticeSettings.ToListAsync();
        }

        public async Task<bool> UpdateNoticeEventChannelsAsync(string eventCode, List<string> activeChannels, string receiverType)
        {
            // 1. 刪除原有該事件的 Channel
            var oldChannels = await _context.SysNoticeChannels.Where(c => c.EventCode == eventCode).ToListAsync();
            _context.SysNoticeChannels.RemoveRange(oldChannels);

            // 2. 重新新增勾選的 Channel 節點
            foreach (var chType in activeChannels)
            {
                var newChannel = new SysNoticeChannel
                {
                    EventCode = eventCode,
                    ChannelType = chType,
                    ReceiverType = receiverType,
                    Avalible = "Y",
                    CreateDate = DateTime.Now
                };
                await _context.SysNoticeChannels.AddAsync(newChannel);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateNoticeSettingAsync(SysNoticeSetting setting)
        {
            if (setting == null) return false;

            var existing = await _context.SysNoticeSettings.FirstOrDefaultAsync(s => s.Nid == setting.Nid);
            if (existing == null) return false;

            existing.Title = setting.Title;
            existing.Content = setting.Content;
            existing.DaysBefore = setting.DaysBefore;
            existing.Avalible = setting.Avalible;
            existing.ModifyDate = DateTime.Now;

            _context.SysNoticeSettings.Update(existing);
            return await _context.SaveChangesAsync() > 0;
        }

        // ==========================================
        // 4. SysIntegration (第三方服務整合設定)
        // ==========================================

        public async Task<IEnumerable<SysIntegration>> GetAllIntegrationsAsync()
        {
            return await _context.SysIntegrations.ToListAsync();
        }

        public async Task<IEnumerable<SysIntegrationConfig>> GetIntegrationConfigsByServiceAsync(string serviceCode)
        {
            return await _context.SysIntegrationConfigs
                .Where(c => c.ServiceCode == serviceCode)
                .ToListAsync();
        }

        public async Task<bool> UpdateIntegrationStatusAsync(string serviceCode, string status)
        {
            var integration = await _context.SysIntegrations.FirstOrDefaultAsync(i => i.ServiceCode == serviceCode);
            if (integration == null) return false;

            integration.ConnectStatus = status;
            if (status == "CONNECTED")
            {
                integration.LastSyncDate = DateTime.Now;
            }
            integration.ModifyDate = DateTime.Now;

            _context.SysIntegrations.Update(integration);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SaveIntegrationConfigsAsync(string serviceCode, Dictionary<string, string?> configs)
        {
            // 查詢出現有該服務的所有 Param
            var dbConfigs = await _context.SysIntegrationConfigs.Where(c => c.ServiceCode == serviceCode).ToListAsync();

            foreach (var kv in configs)
            {
                var target = dbConfigs.FirstOrDefault(c => c.ConfigKey == kv.Key);
                if (target != null)
                {
                    target.ConfigValue = kv.Value;
                    target.ModifyDate = DateTime.Now;
                    _context.SysIntegrationConfigs.Update(target);
                }
                else
                {
                    var newConfig = new SysIntegrationConfig
                    {
                        ServiceCode = serviceCode,
                        ConfigKey = kv.Key,
                        ConfigValue = kv.Value,
                        CreateDate = DateTime.Now
                    };
                    await _context.SysIntegrationConfigs.AddAsync(newConfig);
                }
            }

            return await _context.SaveChangesAsync() > 0;
        }

        // ==========================================
        // 5. SysIpWhitelist (IP白名單防護)
        // ==========================================

        public async Task<IEnumerable<SysIpWhitelist>> GetActiveIpWhitelistAsync()
        {
            return await _context.SysIpWhitelists.Where(ip => ip.Avalible == "Y").ToListAsync();
        }

        public async Task<bool> AddIpToWhitelistAsync(SysIpWhitelist ipItem)
        {
            if (ipItem == null) return false;
            ipItem.CreateDate = DateTime.Now;
            await _context.SysIpWhitelists.AddAsync(ipItem);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteIpFromWhitelistAsync(int nid)
        {
            var ip = await _context.SysIpWhitelists.FirstOrDefaultAsync(i => i.Nid == nid);
            if (ip == null) return false;

            _context.SysIpWhitelists.Remove(ip);
            return await _context.SaveChangesAsync() > 0;
        }

        // ==========================================
        // 6. 系統排程工作維護 (SysJob / Log)
        // ==========================================

        public async Task<IEnumerable<SysJob>> GetAllJobsAsync()
        {
            return await _context.SysJobs.ToListAsync();
        }

        public async Task<bool> UpdateJobStatusAsync(string jobCode, string available)
        {
            var job = await _context.SysJobs.FirstOrDefaultAsync(j => j.JobCode == jobCode);
            if (job == null) return false;

            job.Avalible = available;
            job.ModifyDate = DateTime.Now;
            _context.SysJobs.Update(job);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateJobExecutionTimeAsync(string jobCode, DateTime? lastRun, DateTime? nextRun)
        {
            var job = await _context.SysJobs.FirstOrDefaultAsync(j => j.JobCode == jobCode);
            if (job == null) return false;

            if (lastRun.HasValue) job.LastRunTime = lastRun;
            if (nextRun.HasValue) job.NextRunTime = nextRun;

            _context.SysJobs.Update(job);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task AddJobLogAsync(SysJobLog log)
        {
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (log.ExecuteTime == default) log.ExecuteTime = DateTime.Now;

            await _context.SysJobLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}