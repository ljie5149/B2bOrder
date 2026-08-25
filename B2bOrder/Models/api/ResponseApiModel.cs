namespace B2bOrder.Models.api
{
    public class ApiResponse<T>
    {
        public bool Status { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        #region 成功回應 (Success)

        // 快捷方法：成功（無回傳資料）
        // 將型別改為 ApiResponse<object>，讓 Controller 呼叫時不用特別指定 <object>
        public static ApiResponse<object> Success(string message = "操作成功", string code = "0x0200")
        {
            return new ApiResponse<object>
            {
                Status = true,
                Code = code,
                Message = message,
                Data = null
            };
        }

        // 快捷方法：成功（有回傳資料）
        public static ApiResponse<T> Success(T data, string message = "操作成功", string code = "0x0200")
        {
            return new ApiResponse<T>
            {
                Status = true,
                Code = code,
                Message = message,
                Data = data
            };
        }

        #endregion

        #region 失敗回應 (Fail)

        // 快捷方法：失敗（最常用，通常傳入錯誤訊息與錯誤代碼）
        public static ApiResponse<object> Fail(string message = "操作失敗", string code = "0x0400")
        {
            return new ApiResponse<object>
            {
                Status = false,
                Code = code,
                Message = message,
                Data = null
            };
        }

        // 快捷方法：失敗（如果失敗時，仍有關聯的資料或錯誤細節需要傳給前端）
        public static ApiResponse<T> Fail(T data, string message = "操作失敗", string code = "0x0400")
        {
            return new ApiResponse<T>
            {
                Status = false,
                Code = code,
                Message = message,
                Data = data
            };
        }

        #endregion
    }
}
