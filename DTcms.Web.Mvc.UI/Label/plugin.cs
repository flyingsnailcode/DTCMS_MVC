using DTcms.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DTcms.Web.Mvc.UI.Controllers
{
    public partial class BaseController : Controller
    {
        /// <summary>
        /// 利用反射调用插件方法
        /// </summary>
        /// <param name="className">类名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="objParas">参数</param>
        /// <returns>DataTable</returns>
        public DataTable get_plugin_method(string assemblyName, string className, string methodName, params object[] objParas)
        {
            DataTable dt = new DataTable();
            try
            {
                Assembly assembly = Assembly.Load(assemblyName);
                object obj = assembly.CreateInstance(assemblyName + "." + className, true);
                Type t = obj.GetType();
                //查找匹配的方法
                foreach (MethodInfo m in t.GetMethods(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (m.Name == methodName && m.GetParameters().Length == objParas.Length)
                    {
                        object obj2 = m.Invoke(obj, objParas);
                        dt = obj2 as DataTable;
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                //插件方法获取失败
            }
            return dt;
        }
        /// <summary>
        /// 利用反射调用插件方法
        /// </summary>
        /// <param name="className">类名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="objParas">参数</param>
        /// <returns>DataTable</returns>
        public string get_plugin_content(string assemblyName, string className, string methodName, params object[] objParas)
        {
            string dt = "";
            try
            {
                Assembly assembly = Assembly.Load(assemblyName);
                object obj = assembly.CreateInstance(assemblyName + "." + className, true);
                Type t = obj.GetType();
                //查找匹配的方法
                foreach (MethodInfo m in t.GetMethods(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (m.Name == methodName && m.GetParameters().Length == objParas.Length)
                    {
                        object obj2 = m.Invoke(obj, objParas);
                        dt = obj2 as string;
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                //插件方法获取失败
            }
            return dt;
        }
    }
}
