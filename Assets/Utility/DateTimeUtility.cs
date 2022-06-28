using System;

namespace RogueGods.Utility
{
    public static class DateTimeUtility
    {
        /// <summary>
        /// 获取当前日期时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            var timeStamp = (long) (DateTime.Now - startTime).TotalSeconds; // 相差秒数
            return timeStamp;
        }
        
        public static long GetTimeStamp(DateTime dateTime)
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            var timeStamp = (long) (dateTime - startTime).TotalSeconds; // 相差秒数
            return timeStamp;
        }
        
        public static long GetTimeStampMilliseconds()
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            var timeStamp = (long) (DateTime.Now - startTime).TotalMilliseconds; // 相差秒数
            return timeStamp;
        }

        public static long GetTimeStampMilliseconds(DateTime dateTime)
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            var timeStamp = (long) (dateTime - startTime).TotalMilliseconds; // 相差秒数
            return timeStamp;
        }
        
        /// <summary>
        /// 根据unix时间戳获取日期
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(long unixTimeStamp)
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            var dt = startTime.AddSeconds(unixTimeStamp);
            return dt;
        }
        
        /// <summary>
        /// 根据unix时间戳获取日期
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeFromMilliSecond(long unixTimeStamp)
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            var dt = startTime.AddMilliseconds(unixTimeStamp);
            return dt;
        }
    }
}