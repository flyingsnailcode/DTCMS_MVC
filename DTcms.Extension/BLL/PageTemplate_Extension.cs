using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Reflection;
using DTcms.Common;

namespace DTcms.Extension.Common
{
    /// <summary>
    /// Template为页面模板类.
    /// </summary>
    public class PageTemplate_Extension
    {
        public static Regex[] r = new Regex[23];

        static PageTemplate_Extension()
        {
            RegexOptions options = RegexOptions.None;
            //嵌套模板标签(兼容)
            r[0] = new Regex(@"<%template ((skin=\""([^\[\]\{\}\s]+)\""(?:\s+))?)src=(?:\/|\"")([^\[\]\{\}\s]+)(?:\/|\"")(?:\s*)%>", options);
            //模板路径标签(新增)
            r[1] = new Regex(@"<%templateskin((=(?:\"")([^\[\]\{\}\s]+)(?:\""))?)(?:\s*)%>", options);
            //命名空间标签
            r[2] = new Regex(@"<%namespace (?:""?)([\s\S]+?)(?:""?)%>", options);
            //C#代码标签
            r[3] = new Regex(@"<%csharp%>([\s\S]+?)<%/csharp%>", options);
            //loop循环(抛弃)
            r[4] = new Regex(@"<%loop ((\(([^\[\]\{\}\s]+)\) )?)([^\[\]\{\}\s]+) ([^\[\]\{\}\s]+)%>", options);
            //foreach循环(新增)
            r[5] = new Regex(@"<%foreach(?:\s*)\(([^\[\]\{\}\s]+) ([^\[\]\{\}\s]+) in ([^\[\]\{\}\s]+)\)(?:\s*)%>", options);
            //for循环(新增)
            r[6] = new Regex(@"<%for\(([^\(\)\[\]\{\}]+)\)(?:\s*)%>", options);
            //if语句标签(抛弃)
            r[7] = new Regex(@"<%if (?:\s*)(([^\s]+)((?:\s*)(\|\||\&\&)(?:\s*)([^\s]+))?)(?:\s*)%>", options);
            r[8] = new Regex(@"<%else(( (?:\s*)if (?:\s*)(([^\s]+)((?:\s*)(\|\||\&\&)(?:\s*)([^\s]+))*))?)(?:\s*)%>", options);
            //if语句标签(新增)
            r[9] = new Regex(@"<%if\((([^\s]+)((?:\s*)(\|\||\&\&)(?:\s*)([^\s]+))?)\)(?:\s*)%>", options);
            r[10] = new Regex(@"<%else(( (?:\s*)if\((([^\s]+)((?:\s*)(\|\||\&\&)(?:\s*)([^\s]+))?))?\))(?:\s*)%>", options);
            //循环与判断结束标签(兼容)
            r[11] = new Regex(@"<%\/(?:loop|foreach|for|if)(?:\s*)%>", options);
            //continue标签
            r[12] = new Regex(@"<%continue(?:\s*)%>");
            //break标签
            r[13] = new Regex(@"<%break(?:\s*)%>");
            //request标签
            r[14] = new Regex(@"(\{request\[([^\[\]\{\}\s]+)\]\})", options);
            //截取字符串标签
            r[15] = new Regex(@"(<%cutstring\(([^\s]+?),(.\d*?)\)%>)", options);
            //url链接标签
            r[16] = new Regex(@"(<%linkurl\(([^\s]*?)\)%>)", options);
            //声明赋值标签(兼容)
            r[17] = new Regex(@"<%set ((\(?([\w\.<>]+)(?:\)| ))?)(?:\s*)\{?([^\s\{\}]+)\}?(?:\s*)=(?:\s*)(.*?)(?:\s*)%>", options);
            //数据变量标签
            r[18] = new Regex(@"(\{([^\[\]\{\}\s]+)\[([^\[\]\{\}\s]+)\]\})", options);
            //普通变量标签
            r[19] = new Regex(@"({([^\[\]/\{\}=:'\s]+)})", options);
            //时间格式转换标签
            r[20] = new Regex(@"(<%datetostr\(([^\s]+?),(.*?)\)%>)", options);
            //整型转换标签
            r[21] = new Regex(@"(\{strtoint\(([^\s]+?)\)\})", options);
            //直接输出标签
            r[22] = new Regex(@"<%(?:write |=)(?:\s*)(.*?)(?:\s*)%>", options);
        }
        /// <summary>
        /// 获得模板字符串，从设置中的模板路径来读取模板文件.
        /// </summary>
        /// <param name="sitePath">站点目录</param>
        /// <param name="tempPath">模板目录</param>
        /// <param name="skinName">模板名</param>
        /// <param name="templateName">模板文件的文件名称</param>
        /// <param name="fromPage">源页面名称</param>
        /// <param name="inherit">该页面继承的类</param>
        /// <param name="buildPath">生成目录名</param>
        /// <param name="channelName">频道名称</param>
        /// <param name="nest">嵌套次数</param>
        /// <returns>string值,如果失败则为"",成功则为模板内容的string</returns>
        public static string GetTemplate(string sitePath, string tempPath, string skinName, string templet, string fromPage, string inherit, string buildPath, string channelName, string pageSize,int _site_id, int nest)
        {
            StringBuilder strReturn = new StringBuilder(220000); //返回的字符
            string templetFullPath = Utils.GetMapPath(string.Format("{0}{1}/{2}/{3}", sitePath, tempPath, skinName, templet)); //取得模板文件物理路径

            //超过5次嵌套退出
            if (nest < 1)
            {
                nest = 1;
            }
            else if (nest > 5)
            {
                return "";
            }

            //检查模板文件是否存在
            if (!File.Exists(templetFullPath))
            {
                return "";
            }

            //开始读写文件
            using (StreamReader objReader = new StreamReader(templetFullPath, Encoding.UTF8))
            {
                StringBuilder extNamespace = new StringBuilder(); //命名空间标签转换容器
                StringBuilder textOutput = new StringBuilder(70000);
                textOutput.Append(objReader.ReadToEnd());
                objReader.Close();

                //替换Csharp标签
                foreach (Match m in r[3].Matches(textOutput.ToString()))
                {
                    textOutput.Replace(m.Groups[0].ToString(), m.Groups[0].ToString().Replace("", "\r\r"));
                }
                //替换命名空间标签
                foreach (Match m in r[2].Matches(textOutput.ToString()))
                {
                    extNamespace.Append("@using " + m.Groups[1] + ";");
                    textOutput.Replace(m.Groups[0].ToString(), string.Empty);
                }
                //替换特殊标记
                //textOutput.Replace("<%", "\r<%");
                //textOutput.Replace("%>", "%>\r");
                //textOutput.Replace("<%csharp%>\r", "<%csharp%>").Replace("\r<%/csharp%>", "<%/csharp%>");

                //开始查找替换标签
                string[] strlist = Utils.SplitString(textOutput.ToString(), "\r");
                bool isCodeBlock = false;
                for (int i = 0; i < strlist.Length; i++)
                {
                    if (strlist[i] == "")
                        continue;

                    strReturn.Append(ConvertTags(nest, channelName, pageSize, sitePath, tempPath, skinName, strlist[i], ref isCodeBlock)); //搜索替换标签
                }

                //如果是第一层则写入文件
                if (nest == 1)
                {
                    //定义页面常量
                    string channelStr = string.Empty; //频道名称
                    string constStr = string.Empty; //分页大小
                    string siteidStr = string.Empty; //站点ID
                    if (channelName != string.Empty)
                    {
                        channelStr = "const string channel = \"" + channelName + "\";\r\n";
                    }
                    if (pageSize != string.Empty && Utils.StrToInt(pageSize, 0) > 0)
                    {
                        constStr = "const int pagesize = " + pageSize + ";\r\n";
                    }
                    //网站ID
                    if (_site_id > 0)
                    {
                        siteidStr = "const int site_id = " + _site_id + ";\r\n";
                    }
                    //页面头部声明
                    string template = string.Empty;
                    StringBuilder sb = new StringBuilder();
                    if (templet.IndexOf('_') != 0)
                    {
                        sb.Append(string.Format("<!--本页面代码由DTcms非官方Mvc版模板引擎生成于 {0}-->\r\n", DateTime.Now));
                        sb.Append(string.Format(
                          "@using System.Collections.Generic;\r\n" +
                          "@using System.Text;\r\n" +
                          "@using System.Data;\r\n" +
                          "@using DTcms.Common;\r\n" +
                          //"@using DTcms.Web.Mvc.Controllers;\r\n" +
                          "@using DTcms.Web.Mvc.UI.Controllers;{0}\r\n", extNamespace.ToString()));
                        sb.Append(string.Format(
                          "@{{\r\n" +
                          "DTcms.Model.channel_site site = ViewData[\"site\"] as DTcms.Model.channel_site;\r\n" +
                          "DTcms.Model.siteconfig config = ViewData[\"config\"] as DTcms.Model.siteconfig;\r\n" +
                          "DTcms.Model.userconfig uconfig = ViewData[\"uconfig\"] as DTcms.Model.userconfig;\r\n" +
                          "{0}Controller c = ViewData[\"controller\"] as {0}Controller;\r\n", inherit));

                        Assembly assembly = null;
                        if (inherit.IndexOf("UI") > 0)
                        {
                            assembly = Assembly.Load("DTcms.Web.Mvc.UI");
                        }
                        else
                        {
                            assembly = Assembly.Load("DTcms.Web.Mvc");
                        }
                        Type type = assembly.GetType(inherit + "Controller", true);

                        if (type != null)
                        {
                            Type base_type = type.BaseType;
                            if (base_type.ToString() != "DTcms.Web.Mvc.UI.Controllers.BaseController")
                            {
                                System.Reflection.BindingFlags flag = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
                                System.Reflection.FieldInfo[] fields = base_type.GetFields(flag);
                                foreach (System.Reflection.FieldInfo fi in fields)
                                {
                                    string _str = string.Format("{0} {1} = c.{1};\r\n", fi.FieldType.ToString(), fi.Name);
                                    _str = _str.Replace("`1[", "<").Replace("]", ">");
                                    sb.Append(_str);
                                }
                            }
                        }
                        if (type != null)
                        {
                            System.Reflection.BindingFlags flag = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
                            System.Reflection.FieldInfo[] fields = type.GetFields(flag);
                            foreach (System.Reflection.FieldInfo fi in fields)
                            {
                                if (fi.Name != "channel")
                                {
                                    string _str = string.Format("{0} {1} = c.{1};\r\n", fi.FieldType.ToString(), fi.Name);
                                    _str = _str.Replace("`1[", "<").Replace("]", ">");
                                    sb.Append(_str);
                                }
                            }
                        }
                        sb.Append(string.Format("ViewBag.Title = site.seo_title;\r\n{0}{1}{3}" + "}}\r\n{2}", channelStr, constStr, strReturn.ToString(), siteidStr));
                        template = sb.ToString();
                    }
                    else
                    {
                        template = string.Format(
                        "<!--本页面代码由DTcms非官方Mvc版模板引擎生成于 {2}-->\r\n" +
                         "@using System.Collections.Generic;\r\n" +
                         "@using System.Text;\r\n" +
                         "@using System.Data;\r\n" +
                         "@using DTcms.Common;{1}\r\n" +
                         //"@using DTcms.Web.Mvc.Controllers;\r\n" +
                         "@using DTcms.Web.Mvc.UI.Controllers;\r\n" +
                         "@{{\r\n" +
                         "DTcms.Model.channel_site site = ViewData[\"site\"] as DTcms.Model.channel_site;\r\n" +
                         "DTcms.Model.siteconfig config = ViewData[\"config\"] as DTcms.Model.siteconfig;\r\n" +
                         "DTcms.Model.userconfig uconfig = ViewData[\"uconfig\"] as DTcms.Model.userconfig;\r\n" +
                         "BaseController c = ViewData[\"controller\"] as BaseController;\r\n" +
                         "ViewBag.Title = site.seo_title;\r\n{3}{5}{7}" +
                         "}}\r\n{6}",
                         inherit, extNamespace.ToString(), DateTime.Now, channelStr, strReturn.Capacity, constStr, strReturn.ToString(), siteidStr);
                    }
                    string pageDir = Utils.GetMapPath(string.Format("{0}Areas/Web/Views/{1}/", sitePath, buildPath)); //生成文件的目录路径
                    string outputPath = pageDir + fromPage; //生成文件的物理路径
                                                            //如果物理路径不存在则创建
                    if (!Directory.Exists(pageDir))
                    {
                        Directory.CreateDirectory(pageDir);
                    }
                    //保存写入文件
                    File.WriteAllText(outputPath, template, Encoding.UTF8);
                    //using (FileStream fs = new FileStream(outputPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    //{
                    //    Byte[] info = Encoding.UTF8.GetBytes(template);
                    //    fs.Write(info, 0, info.Length);
                    //    fs.Close();
                    //}
                }
            }

            return strReturn.ToString();
        }

        /// <summary>
        /// 转换标签
        /// </summary>
        /// <param name="nest">深度</param>
        /// <param name="channelName">频道名称</param>
        /// <param name="sitePath">站点目录</param>
        /// <param name="skinName">模板名称</param>
        /// <param name="inputStr">模板内容</param>
        /// <returns></returns>
        private static string ConvertTags(int nest, string channelName, string pageSize, string sitePath, string tempPath, string skinName, string inputStr, ref bool isCodeBlock)
        {
            string strReturn = "";
            string strTemplate = string.Empty;
            strTemplate = inputStr;
            bool IsCodeLine = false;

            #region 解析嵌套标签====================================================OK
            foreach (Match m in r[0].Matches(strTemplate))
            {
                IsCodeLine = true;
                if (m.Groups[3].ToString() != string.Empty)
                {
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("@RenderPage(\"Shared/{0}\")", m.Groups[3].ToString().Replace("html", "cshtml")));
                }
                else
                {
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("@RenderPage(\"Shared/{0}\")", m.Groups[4].ToString().Replace("html", "cshtml")));
                }
            }
            #endregion

            #region 解析模板路径标签================================================OK
            foreach (Match m in r[1].Matches(strTemplate))
            {
                //IsCodeLine = true;
                if (m.Groups[3].ToString() != string.Empty)
                {
                    //strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("templateBuilder.Append(\"{0}{1}/{2}\");", sitePath, "templates", m.Groups[3].ToString()));
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("{0}{1}/{2}", sitePath, "templates", m.Groups[3].ToString()));
                }
                else
                {
                    //strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("templateBuilder.Append(\"{0}{1}/{2}\");", sitePath, tempPath, skinName));
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("{0}{1}/{2}", sitePath, tempPath, skinName));
                }
            }
            #endregion

            #region 解析csharp标签==================================================OK
            foreach (Match m in r[3].Matches(strTemplate))
            {
                IsCodeLine = true;
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(), m.Groups[1].ToString());
            }
            #endregion

            #region 解析loop标签====================================================OK
            foreach (Match m in r[4].Matches(strTemplate))
            {
                IsCodeLine = true;
                isCodeBlock = true;
                if (m.Groups[3].ToString() == "")
                {
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(),
                        string.Format("@{{int {0}__loop__id=0;}}\r\n@foreach(DataRow {0} in {1}.Rows){{\r\n{0}__loop__id++;\r\n", m.Groups[4], m.Groups[5]));
                }
                else
                {
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(),
                        string.Format("int {1}__loop__id=0;\r\nforeach({0} {1} in {2}){{\r\n{1}__loop__id++;\r\n", m.Groups[3], m.Groups[4], m.Groups[5]));
                }
            }
            #endregion

            #region 解析foreach标签=================================================OK
            foreach (Match m in r[5].Matches(strTemplate))
            {
                IsCodeLine = true;
                if (isCodeBlock)
                {
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(),
                            string.Format("foreach({0} {1} in {2}){{", m.Groups[1], m.Groups[2], m.Groups[3]));
                }
                else
                {
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(),
                            string.Format("@foreach({0} {1} in {2}){{", m.Groups[1], m.Groups[2], m.Groups[3]));
                }
                isCodeBlock = true;
            }
            #endregion

            #region 解析for标签=====================================================OK
            foreach (Match m in r[6].Matches(strTemplate))
            {
                IsCodeLine = true;
                if (isCodeBlock)
                {
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(),
                        string.Format("for({0}){{", m.Groups[1]));
                }
                else
                {
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(),
                        string.Format("@for({0}){{", m.Groups[1]));
                }
                isCodeBlock = true;
            }
            #endregion

            #region 解译判断语句if==================================================OK
            foreach (Match m in r[9].Matches(strTemplate))
            {
                IsCodeLine = true;
                string str0 = m.Groups[0].ToString();
                string str1 = m.Groups[1].ToString().Replace("\\\"", "\"");
                if (isCodeBlock)
                {
                    if (str1.IndexOf("get_") == 0)
                    {
                        strTemplate = strTemplate.Replace(str0, "if (c." + str1 + "){");
                    }
                    else
                    {
                        strTemplate = strTemplate.Replace(str0, "if (" + str1 + "){");
                    }
                }
                else
                {
                    if (str1.IndexOf("get_") == 0)
                    {
                        strTemplate = strTemplate.Replace(str0, "@if (c." + str1 + "){");
                    }
                    else
                    {
                        strTemplate = strTemplate.Replace(str0, "@if (" + str1 + "){");
                    }
                }
                isCodeBlock = true;
            }
            foreach (Match m in r[10].Matches(strTemplate))
            {
                IsCodeLine = true;
                if (m.Groups[1].ToString() == string.Empty)
                {
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "}else{");
                }
                else
                {
                    string str3 = m.Groups[3].ToString().Replace("\\\"", "\"");
                    if (str3.IndexOf("get_") == 0)
                    {
                        strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "}else if (c." + str3 + "){");
                    }
                    else
                    {
                        strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "}else if (" + str3 + "){");
                    }
                }
                isCodeBlock = true;
            }
            if (!IsCodeLine)
            {
                foreach (Match m in r[7].Matches(strTemplate))
                {
                    IsCodeLine = true;
                    string str0 = m.Groups[0].ToString();
                    string str1 = m.Groups[1].ToString().Replace("\\\"", "\"");
                    if (isCodeBlock)
                    {
                        if (str1.IndexOf("get_") == 0)
                        {
                            strTemplate = strTemplate.Replace(str0, "if (c." + str1 + "){");
                        }
                        else
                        {
                            strTemplate = strTemplate.Replace(str0, "if (" + str1 + "){");
                        }
                    }
                    else
                    {
                        if (str1.IndexOf("get_") == 0)
                        {
                            strTemplate = strTemplate.Replace(str0, "@if (c." + str1 + "){");
                        }
                        else
                        {
                            strTemplate = strTemplate.Replace(str0, "@if (" + str1 + "){");
                        }
                    }
                    isCodeBlock = true;
                }
                foreach (Match m in r[8].Matches(strTemplate))
                {
                    IsCodeLine = true;
                    if (m.Groups[1].ToString() == string.Empty)
                    {
                        strTemplate = strTemplate.Replace(m.Groups[0].ToString(),
                        "}else{");
                    }
                    else
                    {
                        string str0 = m.Groups[0].ToString();
                        string str3 = m.Groups[3].ToString().Replace("\\\"", "\"");
                        if (str3.IndexOf("get_") == 0)
                        {
                            strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "}else if (c." + str3 + "){");
                        }
                        else
                        {
                            strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "}else if (" + str3 + "){");
                        }
                    }
                    isCodeBlock = true;
                }
            }
            #endregion

            #region 解析循环判断结束标签============================================OK
            foreach (Match m in r[11].Matches(strTemplate))
            {
                IsCodeLine = true;
                isCodeBlock = false;
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "}");
            }
            #endregion

            #region 解析continue,break标签==========================================OK
            foreach (Match m in r[12].Matches(strTemplate))
            {
                IsCodeLine = true;
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "continue;");
            }
            foreach (Match m in r[13].Matches(strTemplate))
            {
                IsCodeLine = true;
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "break;");
            }
            #endregion

            #region 解析截取字符串标签==============================================OK
            foreach (Match m in r[15].Matches(strTemplate))
            {
                IsCodeLine = true;
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(),
                              string.Format("@Utils.DropHTML({0},{1});", m.Groups[2], m.Groups[3].ToString().Trim()));
            }
            #endregion

            #region 解析时间格式转换================================================OK
            foreach (Match m in r[20].Matches(strTemplate))
            {
                IsCodeLine = true;
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(),
                              string.Format("@Utils.ObjectToDateTime({0}).ToString(\"{1}\");", m.Groups[2], m.Groups[3].ToString().Replace("\\\"", string.Empty)));
            }
            #endregion

            #region 字符串转换整型==================================================OK
            foreach (Match m in r[21].Matches(strTemplate))
            {
                IsCodeLine = true;
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(),
                    "Utils.StrToInt(" + m.Groups[2] + ", 0)");
            }
            #endregion

            #region 解析url链接标签=================================================OK
            foreach (Match m in r[16].Matches(strTemplate))
            {
                IsCodeLine = true;
                isCodeBlock = false;
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(),
                              string.Format("@c.linkurl({0})", m.Groups[2]).Replace("\\\"", "\""));
            }
            #endregion

            #region 解析赋值标签====================================================OK
            foreach (Match m in r[17].Matches(strTemplate))
            {
                IsCodeLine = true;
                string type = "";
                if (m.Groups[3].ToString() != string.Empty)
                {
                    type = m.Groups[3].ToString();
                }
                string str5 = m.Groups[5].ToString();
                if (isCodeBlock)
                {
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(),
                       string.Format("{0} {1} = {2};", type, m.Groups[4], str5.IndexOf("get_") == 0 ? "c." + str5 : str5).Replace("\\\"", "\""));
                }
                else
                {
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(),
                       string.Format("@{{{0} {1} = {2};}}", type, m.Groups[4], str5.IndexOf("get_") == 0 ? "c." + str5 : str5).Replace("\\\"", "\""));
                }
                strTemplate = strTemplate.Replace("linkurl", "c.linkurl");

            }
            #endregion

            #region 解析request标签=================================================OK
            foreach (Match m in r[14].Matches(strTemplate))
            {
                if (IsCodeLine)
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "DTRequest.GetString(\"" + m.Groups[2] + "\")");
                else
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("@DTRequest.GetString(\"{0}\")", m.Groups[2]));
            }
            #endregion

            #region 解析直接输出标签================================================OK
            foreach (Match m in r[22].Matches(strTemplate))
            {
                IsCodeLine = true;
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(),
                   string.Format("{0}{1}.ToString();", m.Groups[1].ToString().IndexOf("get_") == 0 ? "@c." + m.Groups[1].ToString() : m.Groups[1].ToString(), m.Groups[2]).Replace("\\\"", "\""));
            }
            #endregion

            #region 解析数据变量标签================================================OK
            foreach (Match m in r[18].Matches(strTemplate))
            {
                if (IsCodeLine)
                {
                    if (Utils.IsNumeric(m.Groups[3].ToString()))
                        strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "Utils.ObjectToStr(" + m.Groups[2] + "[" + m.Groups[3] + "])");
                    else
                    {
                        if (m.Groups[3].ToString() == "_id")
                            strTemplate = strTemplate.Replace(m.Groups[0].ToString(), m.Groups[2] + "__loop__id");
                        else
                            strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "Utils.ObjectToStr(" + m.Groups[2] + "[\"" + m.Groups[3] + "\"])");
                    }
                    strTemplate = strTemplate.Replace(">Utils.ObjectToStr", ">@Utils.ObjectToStr");
                }
                else
                {
                    string _str = strTemplate.Replace(m.Groups[0].ToString(), "").Trim();
                    if (_str == "")
                    {
                        strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "<label>" + m.Groups[0].ToString() + "</label>");
                    }
                    else if (_str == "+")
                    {
                        strTemplate = strTemplate.Replace("+" + m.Groups[0].ToString(), "<label>+" + m.Groups[0].ToString() + "</label>");
                    }
                    if (Utils.IsNumeric(m.Groups[3].ToString()))
                        strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("@{0}[{1}]", m.Groups[2], m.Groups[3]));
                    else
                    {
                        if (m.Groups[3].ToString() == "_id")
                            strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("@{0}__loop__id.ToString()", m.Groups[2]));
                        else
                            strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("@({0}[\"{1}\"])", m.Groups[2], m.Groups[3]));
                    }
                }
            }
            #endregion

            #region 解析普通变量标签================================================OK
            foreach (Match m in r[19].Matches(strTemplate))
            {
                string str1 = m.Groups[0].ToString();
                string str2 = m.Groups[2].ToString().Trim();
                if (IsCodeLine)
                {
                    strTemplate = strTemplate.Replace(str1, string.Format("{0}", str2));
                }
                else
                {
                    if (str2 == "pagelist" || str2 == "model.content" || str2 == "model.zhaiyao")
                    {
                        strTemplate = strTemplate.Replace(str1, string.Format("@Html.Raw({0})", str2));
                    }
                    else
                    {
                        strTemplate = strTemplate.Replace(str1, string.Format("@({0})", str2));
                    }
                }
            }
            #endregion

            #region 最后处理========================================================OK
            if (IsCodeLine)
            {
                strReturn = strTemplate;
            }
            else
            {
                if (strTemplate.Trim() != "")
                {
                    strReturn = strTemplate.Replace("\r\r\r", "\\r\\n");
                    strReturn = strReturn.Replace("\\r\\n<?xml", "<?xml");
                    strReturn = strReturn.Replace("\\r\\n<!DOCTYPE", "<!DOCTYPE");
                }
                isCodeBlock = false;//不是代码行(单纯Html标签行),设置设置isCodeBlock为false,令后续代码if,for,foreach代码行添加@字符
            }
            if (!IsCodeLine)
            {//如果不是标签,添加@:文本输出标记
                int count = IndexOfChinese(strReturn);
                if (count >= 0)
                {
                    Regex regex = new Regex("[\u4e00-\u9fa5]+");
                    Match m = regex.Match(strReturn);
                    string s = m.Groups[0].ToString();
                    strReturn = strReturn.Replace(s, "@Html.Raw(\"" + s + "\")");
                }
            }
            #endregion
            return strReturn;
        }

        /// <summary>
        /// 判断文本是否为单纯中文
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static int IndexOfChinese(string text)
        {
            text = text.Trim();
            int result = -1;
            byte[] chars = Encoding.ASCII.GetBytes(text);
            for (int i = 0; i < chars.Length; i++)
            {
                int asciicode = (int)chars[i];
                if (asciicode >= 33 && asciicode <= 127 && asciicode != 63)
                {
                    result = -1;
                    break;
                }
                else
                {
                    if (result == -1)
                    {
                        result = i;
                    }
                }
            }
            /*for (int i = 0; i < text.Length; i++) {
               char c = 'a';

               if (Regex.IsMatch(text[i].ToString(), @"(?i)^[0-9a-z]+$")) {
                  result = false;
                  break;
               }
            }*/
            return result;
        }
    }
}