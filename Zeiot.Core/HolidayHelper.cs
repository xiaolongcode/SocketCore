using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeiot.Core
{
    public class HolidayHelper
    {
        private static string host= "https://jisuwnl.market.alicloudapi.com/calendar";
        private static string appcode = "c24bfbfc93604a87b74bf2aff81e0d57";

        /// <summary>
        /// 查询阿里云接口 获取当前日期是工作日还是休息日
        /// </summary>
        /// <param name="date">日期 2021-05-01</param>
        /// <returns>返回值 true 工作日</returns>
        public static bool GetAliyunHoliday(string date,out string retweek)
        {
            string url = host + "/query";
            string postDataStr = "date=" + date;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("Authorization", "APPCODE " + appcode);
            string retstr = HttpHelper.HttpGet(url, postDataStr, dic);
            dynamic obj = JsonHelper.JosnStringConvertToDynamic(retstr);
            string status = obj["status"];
            if (status == "0")
            {
                bool b = true;
                string week = obj["result"]["week"];
                retweek = week;
                if (week == "六" || week == "日")
                {
                    b = false;
                }
                if (retstr.Contains("workholiday"))
                {
                    string workholiday = obj["result"]["workholiday"];
                    if (workholiday == "1")
                    {
                        b = false;
                    }
                    else
                    {
                        b = true;
                    }
                }
                return b;
            }
            string weeks = WeekHelper.ConvertDateToWeek(date);
            retweek = weeks;
            if (weeks == "" || weeks == "")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static int GetAliyunHolidayAll()
        {
            /*
             {"status":"0","msg":"ok","result":{"2015-01-01":"1月1日至3日放假调休，共3天。1月4日（星期日）上班。",
            "2015-02-18":"2月18日至24日放假调休，共7天。2月15日（星期日）、2月28日（星期六）上班。",
            "2015-04-05":"4月5日放假，4月6日（星期一）补休。",
            "2015-05-01":"5月1日放假，与周末连休。",
            "2015-06-20":"6月20日放假，6月22日（星期一）补休。",
            "2015-09-03":"9月3日至9月5日（星期六）放假三天，9月6日（星期日）上班",
            "2015-09-27":"9月27日放假。",
            "2015-10-01":"10月1日至7日放假调休，共7天。10月10日（星期六）上班。"}}
             */
            string url = host + "/holiday";
            string postDataStr = "";
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("Authorization", "APPCODE " + appcode);
            string retstr = HttpHelper.HttpGet(url, postDataStr, dic);
            dynamic obj= JsonHelper.JosnStringConvertToDynamic(retstr);
            string status = obj["status"];
            return 0;
        }
    }
}
