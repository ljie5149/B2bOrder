using System;

namespace B2bOrder.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 將可空值的 DateTime 轉換為該日期的起始時間 (00:00:00)
        /// </summary>
        public static DateTime? ToStartOfDay(this DateTime? date)
        {
            if (!date.HasValue) return null;
            return date.Value.Date; // .Date 屬性會將時間部分歸零
        }

        /// <summary>
        /// 將可空值的 DateTime 轉換為該日期的結束時間 (23:59:59)
        /// </summary>
        public static DateTime? ToEndOfDay(this DateTime? date)
        {
            if (!date.HasValue) return null;
            return date.Value.Date.AddDays(1).AddTicks(-1);
        }
    }
}