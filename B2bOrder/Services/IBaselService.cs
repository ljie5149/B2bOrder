using B2bOrder.Models.Db;
using B2bOrder.Services.Common;
using Microsoft.AspNetCore.Identity;

namespace B2bOrder.Services
{
    public interface IBaseService
    {
        /// <summary>
        /// 統一紀錄操作軌跡日誌
        /// </summary>
        Task WriteLog(
            string actionType,
            string? targetSid = null,
            string? targetMid = null,
            string? moduleName = null,
            string? actionName = null,
            string? description = null,
            string result = "成功",
            string? moreDescription = null,
            bool fSkip = false);

        /// <summary>
        /// 統一紀錄推播/發信日誌
        /// </summary>
        Task WritePushLog(
            DataMember member,
            string notifyType,
            string pushType,
            string title,
            string message,
            string result,
            string responseMessage = "");
    }

    public class BaseService : IBaseService
    {
        
        public readonly IFCMService              _fcmService;
        public readonly ILineService             _lineService;
        public readonly ILogActionService        _logActionService;
        public readonly ILogPushService          _logPushService;
        public readonly IEmailService            _emailService;
        public readonly IHttpContextAccessor     _httpContextAccessor;
        public readonly IPasswordHasher<SysUser> _passwordHasher;
        //public readonly ITimeZoneService         _tmService;

        // 統一只注入 IServiceProvider 與 IHttpContextAccessor
        public BaseService(IServiceProvider serviceProvider)
        {
            _fcmService             = serviceProvider.GetRequiredService<IFCMService>();
            _lineService            = serviceProvider.GetRequiredService<ILineService>();
            _logActionService       = serviceProvider.GetRequiredService<ILogActionService>();
            _logPushService         = serviceProvider.GetRequiredService<ILogPushService>();
            _emailService           = serviceProvider.GetRequiredService<IEmailService>();
            _httpContextAccessor    = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            _passwordHasher         = serviceProvider.GetRequiredService<IPasswordHasher<SysUser>>();
            //_tmService              = serviceProvider.GetRequiredService<ITimeZoneService>();
        }


        public async Task WriteLog(
            string actionType,
            string? targetSid = null,
            string? targetMid = null,
            string? moduleName = null,
            string? actionName = null,
            string? description = null,
            string result = "成功",
            string? moreDescription = null,
            bool fSkip = false)
        {
            // 修正：透過 _httpContextAccessor 安全地取得當前登入使用者的 Claims
            var httpContext = _httpContextAccessor.HttpContext;
            if (fSkip)
                return;

            await _logActionService.WriteAsync(
                httpContext,
                actionType: actionType,
                targetSid: targetSid,
                targetMid: targetMid,
                moduleName: moduleName,
                actionName: actionName,
                description: description,
                moreDescription: moreDescription,
                result: result
            );
        }

        public async Task WritePushLog(
            DataMember member,
            string notifyType,
            string pushType,
            string title,
            string message,
            string result,
            string responseMessage = "")
        {
            // 修正：透過 _httpContextAccessor 安全地取得當前登入使用者的 Claims
            var currentUser = _httpContextAccessor.HttpContext?.User;
            var createSid = currentUser?.FindFirst("member_sid")?.Value;

            await _logPushService.CreateAsync(
                new LogPush
                {
                    MemberSid = member.Sid,
                    MemberMid = member.Mid,
                    NotifyType = notifyType,
                    PushType = pushType,
                    Title = title,
                    Message = message,
                    SendDate = DateTime.Now,
                    Result = result,
                    ResponseMessage = responseMessage,
                    CreateSid = createSid
                });
        }
    }
}