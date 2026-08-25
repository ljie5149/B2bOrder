namespace B2bOrder.Models.Common
{
    public static class ModuleNames
    {
        public const string MemberManager       = "會員管理";
        public const string ExpireNotice        = "到期通知";
        public const string NoticeSetting       = "通知設定";
        public const string IntegrationSetting  = "整合設定";
        public const string ImportExport        = "匯入/匯出";
        public const string SecuritySetting     = "安全設定";
        public const string SystemMaintenance   = "系統維護";
        public const string SystemSetting       = "系統設定";
        public const string SystemNotice        = "系統公告";

        /// <summary>
        /// 所有模組名稱的字串陣列清單 (供後端驗證或下拉選單繫結使用)
        /// </summary>
        public static readonly string[] ListAll = new[]
        {
            MemberManager,
            ExpireNotice,
            NoticeSetting,
            IntegrationSetting,
            ImportExport,
            SecuritySetting,
            SystemMaintenance,
            SystemSetting,
            SystemNotice
        };
        public static readonly string[] List = new[]
        {
            MemberManager,
            ExpireNotice,
            NoticeSetting,
            ImportExport,
            SystemNotice
        };
    }
    public static class ActionNames
    {
        public const string Register                        = "註冊會員";
        public const string Login                           = "登入會員";
        public const string Logout                          = "登出會員";
        public const string updateMember                    = "更新個人資料";
        public const string changePassword                  = "變更密碼";
        public const string deleteMember                    = "刪除會員";
        public const string CreateMember                    = "新增下線";
        public const string EditMember                      = "編輯會員";
        public const string RequesgtMember                  = "申請續約";
        public const string RenewMember                     = "續約";
        public const string CreateNotice                    = "新增公告";
        public const string UpdateNotice                    = "編輯公告";
        public const string DownNotice                      = "下架公告";


        public const string ImportMember                    = "匯入會員資料";
        public const string SendExipredNotice4Line          = "透過Line，發送到期通知";
        public const string SendExipredNotice4FCM           = "透過FCM推播，發送到期通知";
        public const string SendExipredNotice4Email         = "透過Email，發送到期通知";
        public const string SendExipredNotice4BatchLine     = "批次發送Line到期通知";
        public const string SendExipredNotice4BatchFCM      = "批次發送FCM推播到期通知";
        public const string SendExipredNotice4BatchEmail    = "批次發送Email到期通知";

        /// <summary>
        /// 所有模組名稱的字串陣列清單 (供後端驗證或下拉選單繫結使用)
        /// </summary>
        public static readonly string[] List = new[]
        {
            Register,
            Login,
            Logout,
            updateMember,
            changePassword,
            deleteMember,
            CreateMember,
            EditMember,
            RequesgtMember,
            RenewMember,
            ImportMember,
            SendExipredNotice4Line,
            SendExipredNotice4FCM,
            SendExipredNotice4Email,
            SendExipredNotice4BatchLine,
            SendExipredNotice4BatchFCM,
            SendExipredNotice4BatchEmail
        };
    }
}
