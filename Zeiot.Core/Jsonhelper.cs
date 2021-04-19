using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Json;

namespace Zeiot.Core
{
    /// <summary>
    /// Json正反序列化Helper-王春杰添加
    /// </summary>
    public class JsonHelper
    {

        /// <summary>
        /// 使用JSON.NET 转换对象到JSON字符串
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string ConvertToJosnString(object item)
        {
            if (item != null)
            {
                IsoDateTimeConverter mTimeConverter = new IsoDateTimeConverter();
                mTimeConverter.DateTimeFormat = "yyyy/MM/dd HH:mm:ss";
                return JsonConvert.SerializeObject(item, Formatting.Indented, mTimeConverter);
            }
            return "";
        }

        /// <summary>
        /// 使用JSON.NET 转换JSON字符串到.NET对象
        /// </summary>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public static T ConvertToObject<T>(string strJson)
        {
            if (!string.IsNullOrEmpty(strJson))
            {
                //return ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(strJson);
                return JsonConvert.DeserializeObject<T>(strJson);
            }
            return default(T);

           
        }
        /// <summary>
        /// 使用JSON.NET 转换JSON字符串到dynamic对象
        /// </summary>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public static dynamic DeserializeObject(string strJson)
        {
            if (!string.IsNullOrEmpty(strJson))
            {
                //return ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(strJson);
                return JsonConvert.DeserializeObject(strJson);
            }
            return null;
        }

        /// <summary>
        /// Json序列化,用于发送到客户端
        /// </summary>
        public static string ToJsJson(object item)
        {

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(item.GetType());

            using (MemoryStream ms = new MemoryStream())
            {

                serializer.WriteObject(ms, item);

                StringBuilder sb = new StringBuilder();

                sb.Append(Encoding.UTF8.GetString(ms.ToArray()));

                return sb.ToString();

            }

        }

        /// <summary>
        /// Json反序列化,用于接收客户端Json后生成对应的对象
        /// </summary>
        public static T FromJsonTo<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));

            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));

            T jsonObject = (T)ser.ReadObject(ms);

            ms.Close();

            return jsonObject;

        }

        /// <summary>
        ///  序列化DataTable为Json,用于发送到客户端
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static string DataTableToJson(DataTable dataTable)
        {
            StringBuilder objSb = new StringBuilder();
            JavaScriptSerializer objSer = new JavaScriptSerializer();
            Dictionary<string, object> resultMain = new Dictionary<string, object>();
            int index = 0;
            foreach (DataRow dr in dataTable.Rows)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                foreach (DataColumn dc in dataTable.Columns)
                {
                    result.Add(dc.ColumnName, dr[dc].ToString());
                }
                resultMain.Add(index.ToString(), result);
                index++;
            }
            objSer.Serialize(resultMain, objSb);
            return objSb.ToString();
        }

        /// <summary>
        /// 序列化DataRow为Json,用于发送到客户端
        /// </summary>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        public static string DataRowToJson(DataRow dataRow)
        {
            StringBuilder objSb = new StringBuilder();
            JavaScriptSerializer objSer = new JavaScriptSerializer();
            var dataTable = dataRow.Table;
            var result = new Dictionary<string, object>();
            foreach (DataColumn dc in dataTable.Columns)
            {
                result.Add(dc.ColumnName, dataRow[dc].ToString());
            }
            objSer.Serialize(result, objSb);
            return objSb.ToString();
        }

        /// <summary>
        /// 使用JSON.NET 转换JSON字符串到JObject
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static JObject JosnStringConvertToJObject(string json)
        {
            var jo = JsonConvert.DeserializeObject(json) as JObject;
            return jo;
        }
        /// <summary>
        /// 使用JSON.NET 转换JSON字符串到dynamic
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static dynamic JosnStringConvertToDynamic(string json)
        {
            dynamic dyObject = new System.Dynamic.ExpandoObject();
            if (!string.IsNullOrEmpty(json))
            {
                dyObject = JsonConvert.DeserializeObject<dynamic>(json);
            }
            else
            {
                dyObject = null;
            }
            return dyObject;

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static Dictionary<string, object> JsonToDictionary(string jsonStr)
        {
            try
            {
                var javaScriptSerializer = new JavaScriptSerializer();
                var value =
                    javaScriptSerializer.DeserializeObject(jsonStr) as Dictionary<string, object>;
                return value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string ObjectToJson<T>(T o)
        {
            var javaScriptSerializer = new JavaScriptSerializer();
            var value = javaScriptSerializer.Serialize(o);
            return value;
        }

        public static string TryObjectToJson<T>(T o)
        {
            try
            {
                var javaScriptSerializer = new JavaScriptSerializer();
                var value = javaScriptSerializer.Serialize(o);
                return value;
            }
            catch
            {
                return string.Empty;
            }

        }

        public static T JsonToObject<T>(string str)
        {
            var javaScriptSerializer = new JavaScriptSerializer();
            var value = javaScriptSerializer.Deserialize<T>(str);
            return value;
        }

        /// <summary>
        /// 使用JSON.NET 转换JSON字符串到.NET对象
        /// </summary>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public static T Convert<T, O>(O strJson)
        {
            if (strJson != null)
            {
                return ConvertToObject<T>(ConvertToJosnString(strJson));
            }
            return default(T);
        }

    }
    public class MinifiedNumArrayConverter : JsonConverter
    {

        private void dumpNumArray<T>(JsonWriter writer, T[] array)
        {
            foreach (T n in array)
            {
                var s = n.ToString();
                //此處可考慮改用string.format("{0:#0.####}")[小數後方#數目依最大小數位數決定]
                //感謝網友vencin提供建議
                if (s.EndsWith(".0"))
                    writer.WriteRawValue(s.Substring(0, s.Length - 2));
                else if (s.Contains("."))
                    writer.WriteRawValue(s.TrimEnd('0'));
                else
                    writer.WriteRawValue(s);
            }
        }

        public override void WriteJson(JsonWriter writer, object value,
            JsonSerializer serializer)
        {
            writer.WriteStartArray();
            Type t = value.GetType();
            if (t == dblArrayType)
                dumpNumArray<double>(writer, (double[])value);
            else if (t == decArrayType)
                dumpNumArray<decimal>(writer, (decimal[])value);
            else
                throw new NotImplementedException();
            writer.WriteEndArray();
        }

        private Type dblArrayType = typeof(double[]);
        private Type decArrayType = typeof(decimal[]);

        public override bool CanConvert(Type objectType)
        {
            if (objectType == dblArrayType || objectType == decArrayType)
                return true;
            return false;
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType,
            object existingValue, JsonSerializer serializer)
        {
            object retVal = new Object();
            if (reader.TokenType == JsonToken.StartObject)
            {
            }
            else if (reader.TokenType == JsonToken.StartArray)
            {
                retVal = serializer.Deserialize(reader, objectType);
            }
            return retVal;
        }

    }
}
