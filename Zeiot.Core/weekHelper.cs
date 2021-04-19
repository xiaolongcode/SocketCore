using System;

namespace Zeiot.Core
{
    public class WeekHelper
    {
        /// <summary>
        /// 根据日期获取星期
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int ConvertDateToWeek(DateTime date)
        {
            string dt = "";
            int week = 00;
            try
            {
                dt = date.DayOfWeek.ToString();
            }
            catch { }
            switch (dt)
            {
                case "Monday":
                    week = 01;
                    break;
                case "Tuesday":
                    week = 02;
                    break;
                case "Wednesday":
                    week = 03;
                    break;
                case "Thursday":
                    week = 04;
                    break;
                case "Friday":
                    week = 05;
                    break;
                case "Saturday":
                    week = 06;
                    break;
                case "Sunday":
                    week = 07;
                    break;
            }
            return week;
        }
        /// <summary>
        /// 根据日期获取星期
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ConvertDateToWeek(string date)
        {
            string dt = "";
            string week = "";
            try
            {
                dt = DateTime.Parse(date).DayOfWeek.ToString();
            }
            catch { }
            switch (dt)
            {
                case "Monday":
                    week = "星期一";
                    break;
                case "Tuesday":
                    week = "星期二";
                    break;
                case "Wednesday":
                    week = "星期三";
                    break;
                case "Thursday":
                    week = "星期四";
                    break;
                case "Friday":
                    week = "星期五";
                    break;
                case "Saturday":
                    week = "星期六";
                    break;
                case "Sunday":
                    week = "星期日";
                    break;
            }
            return week;
        }
        /// <summary>
        /// 根据日期获取月日
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ConvertDateToMMDD(string date) {
            string dt = "";
            try
            {
                dt = DateTime.Parse(date).ToString("M月dd日");
            }
            catch { }
            return dt;
        }
    }
}
