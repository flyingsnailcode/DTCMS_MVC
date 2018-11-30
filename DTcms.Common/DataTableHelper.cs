using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DTcms.Common
{
    /// <summary>    
    /// 实体转换辅助类    
    /// </summary>    
    public class DataTableHelper<T> where T : new()
    {
        public static List<T> ConvertToModel(DataTable dt)
        {
            // 定义集合    
            List<T> list = new List<T>();

            // 获得此模型的类型   
            Type type = typeof(T);
            string colNames = "";

            foreach (DataRow dr in dt.Rows)
            {
                T model = new T();
                // 获得此模型的公共属性      
                PropertyInfo[] ps = model.GetType().GetProperties();
                foreach (PropertyInfo p in ps)
                {
                    // 检查DataTable是否包含此列    
                    colNames = p.Name;
                    if (dt.Columns.Contains(colNames))
                    {
                        // 判断此属性是否有Setter      
                        if (!p.CanWrite)
                            continue;

                        object value = dr[colNames];
                        if (value != DBNull.Value)
                        {
                            //先获取字符型的值
                            string strValue = value.ToString().Trim();
                            if (p.PropertyType == typeof(string))
                            {
                                p.SetValue(model, strValue, null);
                            }
                            else if (p.PropertyType == typeof(int))
                            {
                                p.SetValue(model, int.Parse(strValue), null);
                            }
                            else if (p.PropertyType == typeof(long))
                            {
                                p.SetValue(model, long.Parse(strValue), null);
                            }
                            else if (p.PropertyType == typeof(DateTime))
                            {
                                p.SetValue(model, DateTime.Parse(strValue), null);
                            }
                            else if (p.PropertyType == typeof(float))
                            {
                                p.SetValue(model, float.Parse(strValue), null);
                            }
                            else if (p.PropertyType == typeof(double))
                            {
                                p.SetValue(model, double.Parse(strValue), null);
                            }
                            else if (p.PropertyType == typeof(decimal))
                            {
                                p.SetValue(model, decimal.Parse(strValue), null);
                            }
                            else if (p.PropertyType == typeof(bool))
                            {
                                p.SetValue(model, (strValue.ToUpper() == "TRUE" || strValue == "1"), null);
                            }
                        }
                    }
                }
                list.Add(model);
            }
            return list;
        }

        /// <summary>
        /// 从DataSet中取数字（数字必须是第一张表第一行第一列）
        /// </summary>
        /// <param name="ds">数据集</param>
        /// <returns></returns>
        public static int GetIntFromFristCell(DataSet ds)
        {
            int result = 0;

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                try
                {
                    result = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                }
                catch (Exception ex)
                { }
            }
            return result;
        }
    }
}
