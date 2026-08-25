namespace B2bOrder.Models.Common
{
    public static class MemberRoles
    {
        public const string Regular = "Stc";
        public const string Distributor = "Smn";
        public const string Admin = "Ctl";

        public static readonly HashSet<string> AllRoles = new()
        {
            Regular,
            Distributor,
            Admin
        };

        public static readonly HashSet<string> RegisterRoles = new()
        {
            Regular,
            Distributor
        };

        /// <summary>
        /// 角色中文名稱
        /// </summary>
        public static readonly Dictionary<string, string> Names = new()
        {
            { Regular, "一般會員" },
            { Distributor, "直銷商" },
            { Admin, "管理員" }
        };

        /// <summary>
        /// 取得角色名稱
        /// </summary>
        public static string GetName(string? role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return "未知角色";

            return Names.TryGetValue(role, out var name)
                ? name
                : "未知角色";
        }
        public static string GetBadgeClass(string? role)
        {
            return role switch
            {
                MemberRoles.Regular => "bg-primary",
                MemberRoles.Distributor => "bg-success",
                MemberRoles.Admin => "bg-danger",
                _ => "bg-secondary"
            };
        }
    }
}
