namespace B2bOrder.Models.Common
{
    public static class NotifyTypes
    {
        public const string ExpireNotice =
            "EXPIRE_NOTICE";

        public const string RegisterNotice =
            "REGISTER_NOTICE";

        public const string RenewNotice =
            "RENEW_NOTICE";

        public const string PointNotice =
            "POINT_NOTICE";

        public const string SystemNotice =
            "SYSTEM_NOTICE";

        public const string ActivityNotice =
            "ACTIVITY_NOTICE";

        public static string getName(string input)
        {
            return input switch
            {
                ExpireNotice   => "到期通知",
                RegisterNotice => "註冊通知",
                RenewNotice    => "續期通知",
                PointNotice    => "點數通知",
                SystemNotice   => "系統通知",
                ActivityNotice => "活動通知",
                _              => input
            };
        }
    }
}
