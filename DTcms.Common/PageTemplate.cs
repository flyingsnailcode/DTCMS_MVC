using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Reflection;

namespace DTcms.Common
{
    /// <summary>
    /// Template为页面模板类.
    /// </summary>
    public abstract class PageTemplate
    {
        public static Regex[] r = new Regex[25];

        static PageTemplate()
        {
            RegexOptions options = RegexOptions.None;
            //嵌套模板标签(兼容)
            r[0] = new Regex(@"<%template ((skin=\\""([^\[\]\{\}\s]+)\\""(?:\s+))?)src=(?:\/|\\"")([^\[\]\{\}\s]+)(?:\/|\\"")(?:\s*)%>", options);
            //模板路径标签(新增)
            r[1] = new Regex(@"<%templateskin((=(?:\\"")([^\[\]\{\}\s]+)(?:\\""))?)(?:\s*)%>", options);
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
            //URL链接标签
            r[23] = new Regex(@"(<%queryurl\(([^\s]*?)\)%>)", options);
            //替换
            r[24] = new Regex(@"(<%replace\(([^\s]*?)\)%>)", options);
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
        public static string GetTemplate(string sitePath, string tempPath, string skinName, string templet, string fromPage, string inherit, string buildPath, string channelName, string pageSize, int _site_id, int nest, string name)
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
                
                //替换命名空间标签
                foreach (Match m in r[2].Matches(textOutput.ToString()))
                {
                    extNamespace.Append("@using " + m.Groups[1] + ";");
                    textOutput.Replace(m.Groups[0].ToString(), string.Empty);
                }

                //开始查找替换标签
                string[] strlist = Utils.SplitString(textOutput.ToString(), "\r\r\n");
                for (int i = 0; i < strlist.Length; i++)
                {
                    if (strlist[i] == "")
                        continue;
                    strReturn.Append(ConvertTags(nest, channelName, pageSize, sitePath, tempPath, skinName, strlist[i], _site_id, name)); //搜索替换标签
                }
                //string[] strlist = Utils.SplitString(textOutput.ToString(), "\r");
                //for (int i = 0; i < strlist.Length; i++)
                //{
                //    if (strlist[i] == "")
                //        continue;

                //    strReturn.Append(strlist[i]);//不进行标签替换 直接发挥模板的内容
                //}

                //如果是第一层则写入文件
                if (nest == 1)
                {
                    //定义页面常量
                    string channelStr = string.Empty; //频道名称
                    string constStr = string.Empty; //分页大小
                    string siteStr = string.Empty; //网站ID
                    string validate_channel = string.Empty; //验证频道列表数据
                    //网站ID
                    if (_site_id > 0)
                    {
                        siteStr = "\tconst int site_id = " + _site_id + ";\r\n";
                    }
                    if (channelName != string.Empty)
                    {
                        channelStr = "\tconst string channel = \"" + channelName + "\";\r\n";
                    }
                    if (pageSize != string.Empty && Utils.StrToInt(pageSize, 0) > 0)
                    {
                        constStr = "\tconst int pagesize = " + pageSize + ";\r\n";
                    }
                    if (channelName != string.Empty)
                    {
                        //验证频道列表数据
                        validate_channel = "\tc.validate_channel_data(\"" + channelName + "\");\r\n";
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
                          "@using DTcms.Web.Mvc.UI.Controllers;{0}\r\n", extNamespace.ToString()));
                        sb.Append(string.Format(
                          "@{{\r\n" +
                          "\tDTcms.Model.sites site = ViewBag.site;\r\n" +
                          "\tDTcms.Model.siteconfig config = ViewBag.config;\r\n" +
                          "\tDTcms.Model.userconfig uconfig = ViewBag.uconfig;\r\n" +
                          "\t{0}Controller c = ViewBag.controller;\r\n", inherit));
                        Assembly assembly = null;
                        if (inherit.IndexOf("UI") > 0)
                        {
                            assembly = Assembly.Load("DTcms.Web.Mvc.UI");
                        }
                        else
                        {
                            assembly = Assembly.Load("DTcms.Web.Mvc");
                        }
                        try
                        {
                            Type type1 = assembly.GetType(inherit + "Controller", true);
                        }
                        catch (Exception e)
                        {
                            string ex = e.Message;
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
                                    string _str = string.Format("\t{0} {1} = c.{1};\r\n", fi.FieldType.ToString(), fi.Name);
                                    _str = _str.Replace("`1[", "<").Replace("]", ">");
                                    sb.Append(_str);
                                }
                            }
                        }
                        sb.Append(string.Format("{0}{1}{2}{3}", channelStr, constStr, validate_channel, siteStr));
                        if (type != null)
                        {
                            System.Reflection.BindingFlags flag = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
                            System.Reflection.FieldInfo[] fields = type.GetFields(flag);
                            foreach (System.Reflection.FieldInfo fi in fields)
                            {
                                if (fi.Name != "channel")
                                {
                                    string _str = string.Format("\t{0} {1} = c.{1};\r\n", fi.FieldType.ToString(), fi.Name);
                                    _str = _str.Replace("`1[", "<").Replace("]", ">");
                                    sb.Append(_str);
                                }
                            }
                        }
                        sb.Append("\tViewBag.Title = site.seo_title;\r\n");
                        if (inherit.IndexOf("UI") > 0)
                        {
                            string inherit_web = "DTcms.Web.Mvc.Areas.Web.Controllers." + name.Replace(".cshtml", "");
                            assembly = Assembly.Load("DTcms.Web.Mvc");
                            try
                            {
                                Type type_web2 = assembly.GetType(inherit_web + "Controller", true);
                            }
                            catch (Exception e)
                            {
                                string ex = e.Message;
                            }
                            Type type_web = assembly.GetType(inherit_web + "Controller", true);
                            if (type_web != null)
                            {
                                System.Reflection.BindingFlags flag = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
                                System.Reflection.FieldInfo[] fields = type_web.GetFields(flag);
                                if (fields.Length != 0)
                                {
                                    sb.Append("\t<!--Web -->\r\n");
                                    sb.AppendFormat("\t{0}Controller wc = ViewBag.controller;\r\n", inherit_web);
                                }

                                foreach (System.Reflection.FieldInfo fi in fields)
                                {
                                    if (fi.Name != "channel")
                                    {
                                        string _str = string.Format("\t{0} {1} = wc.{1};\r\n", fi.FieldType.ToString(), fi.Name);
                                        _str = _str.Replace("`1[", "<").Replace("]", ">");
                                        sb.Append(_str);
                                    }
                                }
                            }
                        }

                        sb.Append(string.Format("" + "}}\r\n{0}", strReturn.ToString()));
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
                         "@using DTcms.Web.Mvc.UI.Controllers;\r\n" +
                         "@{{\r\n" +
                         "\tDTcms.Model.sites site = ViewBag.site;\r\n" +
                         "\tDTcms.Model.siteconfig config = ViewBag.config;\r\n" +
                         "\tDTcms.Model.userconfig uconfig = ViewBag.uconfig;\r\n" +
                         "\tBaseController c = ViewBag.controller;\r\n" +
                         "\tViewBag.Title = site.seo_title;\r\n{3}{5}{7}" +
                         "}}\r\n{6}",
                         inherit, extNamespace.ToString(), DateTime.Now, channelStr, strReturn.Capacity, constStr, strReturn.ToString(), siteStr);
                    }
                    string pageDir = Utils.GetMapPath(string.Format("{0}Areas/Web/Views/{1}/", sitePath, buildPath)); //生成文件的目录路径
                    string outputPath = pageDir + fromPage; //生成文件的物理路径

                    if (!Directory.Exists(pageDir))//如果物理路径不存在则创建
                    {
                        Directory.CreateDirectory(pageDir);
                    }
                    //保存写入文件
                    File.WriteAllText(outputPath, template, Encoding.UTF8);
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
        private static string ConvertTags(int nest, string channelName, string pageSize, string sitePath, string tempPath, string skinName, string inputStr, int _site_id, string name)
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
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "\r\n" + GetTemplate(sitePath, "templates", m.Groups[3].ToString(), m.Groups[4].ToString(), string.Empty, string.Empty, string.Empty, channelName, pageSize, _site_id, nest + 1, name) + "\r\n");
                }
                else
                {
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "\r\n" + GetTemplate(sitePath, tempPath, skinName, m.Groups[4].ToString(), string.Empty, string.Empty, string.Empty, channelName, pageSize, _site_id, nest + 1, name) + "\r\n");
                }
            }
            #endregion

            #region 解析模板路径标签================================================OK
            foreach (Match m in r[1].Matches(strTemplate))
            {
                //IsCodeLine = true;
                if (m.Groups[3].ToString() != string.Empty)
                {
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("{0}{1}/{2}", sitePath, "templates", m.Groups[3].ToString()));
                }
                else
                {
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("{0}{1}/{2}", sitePath, tempPath, skinName));
                }
            }
            #endregion

            #region 解析csharp标签==================================================OK
            foreach (Match m in r[3].Matches(strTemplate))
            {
                IsCodeLine = true;
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(), m.Groups[0].ToString().Replace("<%csharp%>", "@{").Replace("<%/csharp%>", "}"));
            }
            #endregion

            #region 解析loop标签====================================================OK
            foreach (Match m in r[4].Matches(strTemplate))
            {
                IsCodeLine = true;
                if (m.Groups[3].ToString() == "")
                {
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("\tint {0}__loop__id=0;\r\n\tforeach(DataRow {0} in {1}.Rows)\r\n\t{{\r\n\t\t{0}__loop__id++;", m.Groups[4], m.Groups[5]));
                }
                else
                {
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("\tint {1}__loop__id=0;\r\n\tforeach({0} {1} in {2})\r\n\t{{\r\n\t\t{1}__loop__id++;", m.Groups[3], m.Groups[4], m.Groups[5]));
                }
            }
            #endregion

            #region 解析foreach标签=================================================OK
            foreach (Match m in r[5].Matches(strTemplate))
            {
                IsCodeLine = true;
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("\tforeach({0} {1} in {2})\r\n\t{{", m.Groups[1], m.Groups[2], m.Groups[3]));
            }
            #endregion

            #region 解析for标签=====================================================OK
            foreach (Match m in r[6].Matches(strTemplate))
            {
                IsCodeLine = true;
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("\r\n\tfor({0})\r\n\t{{", m.Groups[1]));
            }
            #endregion

            #region 解译判断语句if==================================================OK
            foreach (Match m in r[7].Matches(strTemplate))
            {
                IsCodeLine = true;
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "\tif (" + m.Groups[1].ToString().Replace("\\\"", "\"") + ")\r\n{");
            }
            foreach (Match m in r[8].Matches(strTemplate))
            {
                IsCodeLine = true;
                if (m.Groups[1].ToString() == string.Empty)
                {
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "\t}\r\n\telse\r\n\t{");
                }
                else
                {
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "\t}\r\n\telse if (" + m.Groups[3].ToString().Replace("\\\"", "\"") + ")\r\n\t{");
                }
            }
            foreach (Match m in r[9].Matches(strTemplate))
            {
                IsCodeLine = true;
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "if (" + m.Groups[1].ToString().Replace("\\\"", "\"") + ")\r\n\t{");
            }
            foreach (Match m in r[10].Matches(strTemplate))
            {
                IsCodeLine = true;
                if (m.Groups[1].ToString() == string.Empty)
                {
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "\t}\r\n\telse\r\n\t{");
                }
                else
                {
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "\t}\r\n\telse if (" + m.Groups[3].ToString().Replace("\\\"", "\"") + ")\r\n\t{");
                }
            }
            #endregion

            #region 解析循环判断结束标签============================================OK
            foreach (Match m in r[11].Matches(strTemplate))
            {
                IsCodeLine = true;
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "}");
            }
            #endregion

            #region 解析continue,break标签==========================================OK
            foreach (Match m in r[12].Matches(strTemplate))
            {
                IsCodeLine = true;
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "\tcontinue;\r\n");
            }
            foreach (Match m in r[13].Matches(strTemplate))
            {
                IsCodeLine = true;
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "\tbreak;\r\n");
            }
            #endregion

            #region 解析截取字符串标签==============================================OK
            foreach (Match m in r[15].Matches(strTemplate))
            {
                IsCodeLine = true;
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("@Utils.DropHTML({0},{1})", m.Groups[2], m.Groups[3].ToString().Trim()));
            }
            #endregion

            #region 解析时间格式转换================================================OK
            foreach (Match m in r[20].Matches(strTemplate))
            {
                IsCodeLine = true;
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("@Utils.ObjectToDateTime({0}).ToString(\"{1}\")", m.Groups[2], m.Groups[3].ToString().Replace("\\\"", string.Empty)));
            }
            #endregion

            #region 字符串转换整型==================================================OK
            foreach (Match m in r[21].Matches(strTemplate))
            {
                IsCodeLine = true;
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "@Utils.StrToInt(" + m.Groups[2] + ", 0)");
            }
            #endregion

            #region 解析url链接标签=================================================OK
            foreach (Match m in r[16].Matches(strTemplate))
            {
                IsCodeLine = true;
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(),  string.Format("@c.linkurl({0})", m.Groups[2]).Replace("\\\"", "\""));
            }
            #endregion

            #region 解析url链接标签（2）============================================OK
            foreach (Match m in r[23].Matches(strTemplate))
            {
                IsCodeLine = true;
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(),  string.Format("@c.queryurl({0})", m.Groups[2]).Replace("\\\"", "\""));
            }
            #endregion

            #region 解析replace标签==================================================OK
            foreach (Match m in r[24].Matches(strTemplate))
            {
                IsCodeLine = true;
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("@Utils.ReplaceStr({0})", m.Groups[2]).Replace("\\\"", "\""));
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
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("\t{0} {1} = {2};", type, m.Groups[4], m.Groups[5]).Replace("\\\"", "\""));
            }
            #endregion

            #region 解析request标签=================================================OK
            foreach (Match m in r[14].Matches(strTemplate))
            {
                if (IsCodeLine)
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "@DTRequest.GetString(\"" + m.Groups[2] + "\")");
                else
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("\" + @DTRequest.GetString(\"{0}\") + \"", m.Groups[2]));
            }
            #endregion

            #region 解析直接输出标签================================================OK
            foreach (Match m in r[22].Matches(strTemplate))
            {
                IsCodeLine = true;
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("@{0}{1}.ToString()", m.Groups[1], m.Groups[2]).Replace("\\\"", "\""));
            }
            #endregion

            #region 解析数据变量标签================================================OK
            foreach (Match m in r[18].Matches(strTemplate))
            {
                if (IsCodeLine)
                {
                    if (Utils.IsNumeric(m.Groups[3].ToString()))
                        strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "@Utils.ObjectToStr(" + m.Groups[2] + "[" + m.Groups[3] + "])");
                    else
                    {
                        if (m.Groups[3].ToString() == "_id")
                            strTemplate = strTemplate.Replace(m.Groups[0].ToString(), m.Groups[2] + "__loop__id");
                        else
                            strTemplate = strTemplate.Replace(m.Groups[0].ToString(), "@Utils.ObjectToStr(" + m.Groups[2] + "[\"" + m.Groups[3] + "\"])");
                    }
                }
                else
                {
                    if (Utils.IsNumeric(m.Groups[3].ToString()))
                        strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("\" + @Utils.ObjectToStr({0}[{1}]) + \"", m.Groups[2], m.Groups[3]));
                    else
                    {
                        if (m.Groups[3].ToString() == "_id")
                            strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("\" + {0}__loop__id.ToString() + \"", m.Groups[2]));
                        else
                            strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("\" + @Utils.ObjectToStr({0}[\"{1}\"]) + \"", m.Groups[2], m.Groups[3]));
                    }
                }
            }
            #endregion

            #region 解析普通变量标签================================================OK
            foreach (Match m in r[19].Matches(strTemplate))
            {
                if (IsCodeLine)
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("@({0})", m.Groups[2].ToString()));
                else
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(), string.Format("@Utils.ObjectToStr({0}))", m.Groups[2].ToString().Trim()));
            }
            #endregion

            #region 最后处理========================================================OK
            if (IsCodeLine)
            {
                strReturn = strTemplate + "\r\n";
            }
            else
            {
                if (strTemplate.Trim() != "")
                {
                    strReturn = "\"" + strTemplate.Replace("\r\r\r", "\\r\\n") + "\"";
                    strReturn = strReturn.Replace("\\r\\n<?xml", "<?xml");
                    strReturn = strReturn.Replace("\\r\\n<!DOCTYPE", "<!DOCTYPE");
                }
            }
            #endregion

            return strReturn.Replace("<%/csharp%>","}");
        }

    }
}