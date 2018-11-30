using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DTcms.Common;

namespace DTcms.Common
{
    /// <summary>
    /// 操作正则表达式的公共类
    /// </summary>    
    public class RegexHelper
    {
        #region 验证输入字符串是否与模式字符串匹配
        /// <summary>
        /// 验证输入字符串是否与模式字符串匹配，匹配返回true
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="pattern">模式字符串</param>        
        public static bool IsMatch(string input, string pattern)
        {
            return IsMatch(input, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证输入字符串是否与模式字符串匹配，匹配返回true
        /// </summary>
        /// <param name="input">输入的字符串</param>
        /// <param name="pattern">模式字符串</param>
        /// <param name="options">筛选条件</param>
        public static bool IsMatch(string input, string pattern, RegexOptions options)
        {
            return Regex.IsMatch(input, pattern, options);
        }
        #endregion

        #region 匹配查询结果
        /// <summary>
        /// 查询字符型结果
        /// </summary>
        /// <param name="input">正则表达式</param>
        /// <param name="pattern">输入字符串</param>  
        public static string toStr(string pattern, string input)
        {
            if (!string.IsNullOrEmpty(pattern) && pattern != null)
            {
                Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
                Match m = r.Match(input);
                if (m.Success)
                {
                    return m.Value;
                }
            }
            return "";
        }
        /// <summary>
        /// 查询整型结果
        /// </summary>
        /// <param name="input">正则表达式</param>
        /// <param name="pattern">输入字符串</param> 
        public static int toInt(string pattern, string input)
        {
            if (!string.IsNullOrEmpty(pattern) && pattern != null)
            {
                return Utils.StrToInt(toStr(pattern, input), 0);
            }
            return 0;
        }
        #endregion

        #region 匹配查询列表
        /// <summary>
        /// 查询所有匹配结果
        /// </summary>
        /// <param name="pattern">正则表达式</param>
        /// <param name="input">输入字符串</param>
        /// <returns></returns>
        public static MatchCollection toMatches(string pattern, string input)
        {
            if (!string.IsNullOrEmpty(pattern) && pattern != null)
            {
                Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
                return r.Matches(input);
            }
            return null;
        }
        /// <summary>
        /// 查询所有匹配结果
        /// </summary>
        /// <param name="pattern">正则表达式</param>
        /// <param name="input">输入字符串</param>
        /// <param name="repeat">重复过滤</param>
        /// <returns></returns>
        public static List<string> toMatches(string pattern, string input, bool repeat)
        {
            List<string> ls = new List<string>();
            if (!string.IsNullOrEmpty(pattern) && pattern != null)
            {
                Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
                foreach (Match m in r.Matches(input))
                {
                    if (repeat && !ls.Contains(m.Groups[0].ToString()))
                    {
                        ls.Add(m.Groups[0].ToString());
                    }
                }
            }
            return ls;
        }
        #endregion
    }
}
