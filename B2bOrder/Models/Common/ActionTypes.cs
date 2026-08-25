namespace B2bOrder.Models.Common
{
    /// <summary>
    /// 操作紀錄分類
    /// </summary>
    public static class ActionTypes
    {
        /// <summary>
        /// 註冊
        /// </summary>
        public const string Register = "REGISTER";

        /// <summary>
        /// 登入
        /// </summary>
        public const string Login = "LOGIN";

        /// <summary>
        /// 登出
        /// </summary>
        public const string Logout = "LOGOUT";

        /// <summary>
        /// 查詢
        /// </summary>
        public const string Query = "QUERY";

        /// <summary>
        /// 新增
        /// </summary>
        public const string Create = "CREATE";

        /// <summary>
        /// 修改
        /// </summary>
        public const string Update = "UPDATE";

        /// <summary>
        /// 刪除
        /// </summary>
        public const string Delete = "DELETE";

        /// <summary>
        /// 匯入
        /// </summary>
        public const string Import = "IMPORT";

        /// <summary>
        /// 匯出
        /// </summary>
        public const string Export = "EXPORT";

        /// <summary>
        /// 上傳
        /// </summary>
        public const string Upload = "UPLOAD";

        /// <summary>
        /// 下載
        /// </summary>
        public const string Download = "DOWNLOAD";

        /// <summary>
        /// 密碼異動
        /// </summary>
        public const string Password = "PASSWORD";

        /// <summary>
        /// 續期
        /// </summary>
        public const string Renewal = "RENEWAL";

        /// <summary>
        /// 通知
        /// </summary>
        public const string Notify = "NOTIFY";

        /// <summary>
        /// 其他
        /// </summary>
        public const string Other = "OTHER";

        public const string CreateMember = "CREATE_MEMBER";
        public const string EditMember = "EDIT_MEMBER";
        public const string DeleteMember = "DELETE_MEMBER";

        public const string ChangePassword = "CHANGE_PASSWORD";

        public const string ImportMember = "import_member";
        public const string ExportMember = "export_member";

        public const string EXPIRE_EMAIL = "EXPIRE_EMAIL";
        public const string EXPIRE_LINE = "EXPIRE_LINE";
        public const string EXPIRE_FCM = "EXPIRE_FCM";

        // ==================== 🟢 新增：API 呼叫對應的 Log 類型 ====================

        /// <summary>
        /// API 註冊
        /// </summary>
        public const string ApiRegister = "API_REGISTER";

        /// <summary>
        /// API 登入
        /// </summary>
        public const string ApiLogin = "API_LOGIN";

        /// <summary>
        /// API 查詢 (包含 GetAll, GetById, GetBySid 等)
        /// </summary>
        public const string ApiQuery = "API_QUERY";

        /// <summary>
        /// API 新增資料
        /// </summary>
        public const string ApiCreate = "API_CREATE";

        /// <summary>
        /// API 修改資料
        /// </summary>
        public const string ApiUpdate = "API_UPDATE";

        /// <summary>
        /// API 刪除資料
        /// </summary>
        public const string ApiDelete = "API_DELETE";

        // ==================== 🟢 新增：Routine 呼叫對應的 Log 類型 ====================

        /// <summary>
        /// API 新增資料
        /// </summary>
        public const string BackendRoutine = "BACKEND_ROUTINE";


        public static string getName(string input)
        {
            string ret = "";
            switch (input)
            {
                case Register:
                    ret = "註冊";
                    break;
                case Login:
                    ret = "登入";
                    break;
                case Logout:
                    ret = "登出";
                    break;
                case Query:
                    ret = "查詢";
                    break;
                case Create:
                    ret = "新增";
                    break;
                case Update:
                    ret = "修改";
                    break;
                case Delete:
                    ret = "刪除";
                    break;
                case Import:
                    ret = "匯入";
                    break;
                case Export:
                    ret = "匯出";
                    break;
                case Upload:
                    ret = "上傳";
                    break;
                case Download:
                    ret = "下載";
                    break;
                case Password:
                    ret = "密碼異動";
                    break;
                case Renewal:
                    ret = "續期";
                    break;
                case Notify:
                    ret = "通知";
                    break;
                case Other:
                    ret = "其他";
                    break;

                // ==================== 🟢 新增：API 顯示名稱對照 ====================
                case ApiRegister:
                    ret = "API註冊";
                    break;
                case ApiLogin:
                    ret = "API登入";
                    break;
                case ApiQuery:
                    ret = "API查詢";
                    break;
                case ApiCreate:
                    ret = "API新增";
                    break;
                case ApiUpdate:
                    ret = "API修改";
                    break;
                case ApiDelete:
                    ret = "API刪除";
                    break;

                // ==================== 🟢 新增：Routine 呼叫對應的 Log 類型 ====================
                case BackendRoutine:
                    ret = "背景執行緒";
                    break;
            }
            return ret;
        }
    }
}