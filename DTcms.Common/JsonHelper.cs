using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Web.Mvc;

namespace DTcms.Common
{
    /// <summary>
    /// JSON帮助类
    /// </summary>
    public class JsonHelper
    {
        static readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        /// <summary> 
        /// 对象转JSON 
        /// </summary> 
        /// <param name="obj">对象</param> 
        /// <returns>JSON格式的字符串</returns> 
        public static string ObjectToJSON(object obj)
        {
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            return JsonConvert.SerializeObject(obj, timeConverter);
        }
        /// <summary>  
        /// 返回本对象的Json序列化  
        /// </summary>  
        /// <param name="obj"></param>  
        /// <returns></returns>  
        public static string ToJson(object obj)
        {
            string result = serializer.Serialize(obj);
            result = Regex.Replace(result, @"\\/Date\((\d+)\)\\/", match =>
            {
                DateTime dt = new DateTime(1970, 1, 1);
                dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                dt = dt.ToLocalTime();
                return dt.ToString("yyyy-MM-dd HH:mm:ss");
            });
            return result;
        }

        /// <summary> 
        /// 数据表转键值对集合
        /// 把DataTable转成 List集合, 存每一行 
        /// 集合中放的是键值对字典,存每一列 
        /// </summary> 
        /// <param name="dt">数据表</param> 
        /// <returns>哈希表数组</returns> 
        public static List<Dictionary<string, object>> DataTableToList(DataTable dt)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    dic.Add(dc.ColumnName, dr[dc.ColumnName]);
                }
                list.Add(dic);
            }
            return list;
        }
        public static string DataTableToJson(DataTable dt)
        {
            //统一将DataTable加一列排序列
            dt.Columns.Add("Index", typeof(int));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["Index"] = (i + 1).ToString();
            }

            List<Dictionary<string, object>> dic = new List<Dictionary<string, object>>();
            foreach (DataRow drIn in dt.Rows)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                //Hashtable ht = new Hashtable();
                foreach (DataColumn dc in dt.Columns)
                {
                    if (dc.DataType == typeof(DateTime))
                    {
                        if (!string.IsNullOrEmpty(drIn[dc.ColumnName].ToString()))
                        {
                            string currentDt = Convert.ToDateTime(drIn[dc.ColumnName]).ToString("yyyy-MM-dd HH:mm:ss");
                            result.Add(dc.ColumnName, currentDt);
                        }
                    }
                    else if (dc.DataType.Name.ToLower() == "decimal" || dc.DataType.Name.ToLower() == "float")
                    {
                        if (drIn[dc.ColumnName].ToString() == "")
                        {
                            result.Add(dc.ColumnName, Convert.ToDecimal("0").ToString("#0.00"));
                        }
                        else
                        {
                            result.Add(dc.ColumnName, Convert.ToDecimal(drIn[dc.ColumnName]).ToString("#0.00"));
                        }
                    }
                    else if (dc.DataType != typeof(string))
                    {
                        result.Add(dc.ColumnName, drIn[dc.ColumnName]);
                    }

                    else
                    {
                        if (drIn[dc.ColumnName] != null && drIn[dc.ColumnName] != DBNull.Value)
                        {
                            result.Add(dc.ColumnName, HtmlEncode(drIn[dc.ColumnName].ToString()));
                        }
                        else
                        {
                            result.Add(dc.ColumnName, "");
                        }
                    }
                }
                dic.Add(result);
            }
            return ToJson(dic);
        }
        public static string HtmlEncode(string text)
        {
            return text.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;");
        }
        public static string DataTableToJson(string jsonName, DataTable dt)
        {
            return "{" + jsonName + ":" + DataTableToJson(dt) + "}";
        }
        public static string DataTableToJsonGrid(DataTable dt, int recordCount, int sEcho)
        {
            if (dt != null)
            {
                string jsonName = string.Format("\"draw\":{0},\"recordsTotal\":\"{1}\",\"recordsFiltered\":\"{2}\",\"aaData\"", sEcho, dt.Rows.Count, recordCount);
                return DataTableToJson(jsonName, dt);
            }
            else
            {
                return "{\"draw\":0,\"recordsTotal\":\"0\",\"recordsFiltered\":\"0\",\"aaData\":[]}";
            }
        }

        /// <summary> 
        /// 数据集转键值对数组字典 
        /// </summary> 
        /// <param name="dataSet">数据集</param> 
        /// <returns>键值对数组字典</returns> 
        public static Dictionary<string, List<Dictionary<string, object>>> DataSetToDic(DataSet ds)
        {
            Dictionary<string, List<Dictionary<string, object>>> result = new Dictionary<string, List<Dictionary<string, object>>>();

            foreach (DataTable dt in ds.Tables)
            {
                result.Add(dt.TableName, DataTableToList(dt));
            }
            return result;
        }

        /// <summary> 
        /// 数据表转JSON 
        /// </summary> 
        /// <param name="dataTable">数据表</param> 
        /// <returns>JSON字符串</returns> 
        public static string DataTableToJSON(DataTable dt)
        {
            return ObjectToJSON(DataTableToList(dt));
        }

        /// <summary> 
        /// JSON文本转对象,泛型方法 
        /// </summary> 
        /// <typeparam name="T">类型</typeparam> 
        /// <param name="jsonText">JSON文本</param> 
        /// <returns>指定类型的对象</returns> 
        public static T JSONToObject<T>(string jsonText)
        {
            return jsonText == null ? default(T) : JsonConvert.DeserializeObject<T>(jsonText);
        }

        /// <summary> 
        /// 将JSON文本转换为数据表数据 
        /// </summary> 
        /// <param name="jsonText">JSON文本</param> 
        /// <returns>数据表字典</returns> 
        public static Dictionary<string, List<Dictionary<string, object>>> TablesDataFromJSON(string jsonText)
        {
            return JSONToObject<Dictionary<string, List<Dictionary<string, object>>>>(jsonText);
        }

        /// <summary> 
        /// 将JSON文本转换成数据行 
        /// </summary> 
        /// <param name="jsonText">JSON文本</param> 
        /// <returns>数据行的字典</returns>
        public static Dictionary<string, object> DataRowFromJSON(string jsonText)
        {
            return JSONToObject<Dictionary<string, object>>(jsonText);
        }

        /// <summary>
        /// 获取Json关键词
        /// </summary>
        /// <param name="result"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetString(string result, string key)
        {
            JObject Items = JObject.Parse(result);
            if (Items[key] != null)
            {
                return Items[key].ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 输出Json列表
        /// </summary>
        /// <param name="response"></param>
        public static void WriteJson(HttpContext context, object response)
        {
            string jsonpCallback = context.Request["callback"],
                   json = ObjectToJSON(response);
            if (String.IsNullOrWhiteSpace(jsonpCallback))
            {
                context.Response.AddHeader("Content-Type", "text/plain");
                context.Response.Write(json);
            }
            else
            {
                context.Response.AddHeader("Content-Type", "application/javascript");
                context.Response.Write(String.Format("{0}({1});", jsonpCallback, json));
            }
            context.Response.End();
        }
    }
}