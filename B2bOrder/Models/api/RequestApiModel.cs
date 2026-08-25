using B2bOrder.Models.Db;

namespace B2bOrder.Models.api
{
    public class RequestApiModel4Standard
    {
        public string memberSid { get; set; } = string.Empty;
    }
    public class RequestApiModel4CalendarGet
    {
        public string memberSid { get; set; } = string.Empty;
    }
    public class RequestApiModel4CalendarPost: RequestApiModel4CalendarGet
    {
        public string title { get; set; } = string.Empty;
        public string className { get; set; } = string.Empty;
    }
    public class RequestApiModel4CalendarPut : RequestApiModel4CalendarGet
    {
        public string sid { get; set; } = string.Empty;
        public string title { get; set; } = string.Empty;
        public string className { get; set; } = string.Empty;
    }
    public class RequestApiModel4CalendarTimeline: RequestApiModel4CalendarGet
    {
        public string sid { get; set; } = string.Empty;
        public string start { get; set; } = string.Empty;
        public string end { get; set; } = string.Empty;
        public bool allDay { get; set; } = false;
    }
    public class RequestApiModel4CalendarComplete: RequestApiModel4CalendarGet
    {
        public string sid { get; set; } = string.Empty;
        public bool isDone { get; set; } = false;
    }
    public class RequestApiModel4CalendarDelete: RequestApiModel4CalendarGet
    {
        public string sid { get; set; } = string.Empty;
    }
    public class RequestApiModel4ChangePassword
    {
        public string memberSid { get; set; } = string.Empty;
        public ChangePasswordViewModel? changePasswordData { get; set; } = null;
        public ResetPasswordViewModel? resetPasswordData { get; set; } = null;
    }
    public class RequestApiModel4CreateAnnouncement : CreateAnnouncementModel
    {
        public string memberSid { get; set; } = string.Empty;
    }
    public class RequestApiModel4EditAnnouncement : EditAnnouncementModel
    {
    }
    public class RequestApiModel4Dashboard : RequestApiModel4Standard
    {
        public int months { get; set; } = -1;
    }

    // 🟢 專門給 API 登入接收 JSON 用的實體模型 (DTO)
    public class RequestApiModel4Login : LoginViewModel
    {
    }
    public class RequestApiModel4Register : SysUserViewModel
    {
    }

    public class RequestApiModel4Renewal : MemberCenterViewModel
    {
        public string memberSid { get; set; } = string.Empty;
    }
    public class MemberPagedResultDto
    {
        public IEnumerable<DataMember> Data { get; set; } = new List<DataMember>();
        public int TotalCount { get; set; }
    }
    public class RequestApiModel4SysUserNid : RequestApiModel4Standard
    {
        public int nid { get; set; } = -1;
    }
    public class RequestApiModel4SysUser : RequestApiModel4SysUserNid
    {
        public SysUser sysUser { get; set; } = new SysUser();
    }
}
