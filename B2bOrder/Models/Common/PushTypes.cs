namespace B2bOrder.Models.Common
{
    public static class PushTypes
    {
        public const string Line = "LINE";

        public const string Email = "EMAIL";

        public const string Fcm = "FCM";

        public const string Sms = "SMS";

        public static string getName(string input)
        {
            return input switch
            {
                Line    => "LINE",
                Email   => "Email",
                Fcm     => "FCM",
                Sms     => "SMS",
                _       => input
            };
        }
    }
}
