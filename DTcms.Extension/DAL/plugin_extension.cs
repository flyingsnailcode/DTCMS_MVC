using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using DTcms.Common;
using DTcms.DBUtility;
using System.Reflection;

namespace DTcms.DAL
{
    /// <summary>
    /// 插件管理(修改MarkTemplet方法,支持mvc插件,不能支持dtcms原有插件)
    /// </summary>
    public partial class plugin_extension : DapperRepository<Model.plugin>
    {
        private string databaseprefix; //数据库表名前缀
        public plugin_extension(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }

        /// <summary>
        /// 返回插件列表
        /// </summary>
        public List<Model.plugin> GetList(string dirPath)
        {
            List<Model.plugin> lt = new List<Model.plugin>();
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            foreach (DirectoryInfo dir in dirInfo.GetDirectories())
            {
                Model.plugin aboutInfo = GetInfo(dir.FullName + @"\");
                lt.Add(new Model.plugin
                {
                    directory = aboutInfo.directory,
                    name = aboutInfo.name,
                    author = aboutInfo.author,
                    version = aboutInfo.version,
                    description = aboutInfo.description,
                    isload = aboutInfo.isload
                });
            }
            return lt;
        }

        /// <summary>
        /// 返回插件说明信息
        /// </summary>
        public Model.plugin GetInfo(string dirPath)
        {
            Model.plugin aboutInfo = new Model.plugin();
            ///存放关于信息的文件 plugin.config是否存在,不存在返回空串
            if (!File.Exists(dirPath + DTKeys.FILE_PLUGIN_XML_CONFING))
                return aboutInfo;

            XmlDocument xml = new XmlDocument();
            xml.Load(dirPath + DTKeys.FILE_PLUGIN_XML_CONFING);
            try
            {
                XmlNode root = xml.SelectSingleNode("plugin");
                foreach (XmlNode n in root.ChildNodes)
                {
                    switch (n.Name)
                    {
                        case "directory":
                            aboutInfo.directory = n.InnerText;
                            break;
                        case "name":
                            aboutInfo.name = n.InnerText;
                            break;
                        case "author":
                            aboutInfo.author = n.InnerText;
                            break;
                        case "version":
                            aboutInfo.version = n.InnerText;
                            break;
                        case "description":
                            aboutInfo.description = n.InnerText;
                            break;
                        case "isload":
                            aboutInfo.isload = int.Parse(n.InnerText);
                            break;
                    }
                }
            }
            catch
            {
                aboutInfo = new Model.plugin();
            }
            return aboutInfo;
        }

        /// <summary>
        /// 生成模板文件
        /// </summary>
        public void MarkTemplet(string sitePath, string tempPath, string skinName, string dirPath, string xPath)
        {
            XmlNodeList xnList = XmlHelper.ReadNodes(dirPath + DTKeys.FILE_PLUGIN_XML_CONFING, xPath);
            foreach (XmlElement xe in xnList)
            {
                if (xe.NodeType != XmlNodeType.Comment && xe.Name.ToLower() == "rewrite")
                {
                    if (xe.Attributes["page"] != null && !string.IsNullOrEmpty(xe.Attributes["page"].InnerText)
                        && !string.IsNullOrEmpty(xe.Attributes["templet"].InnerText) && !string.IsNullOrEmpty(xe.Attributes["inherit"].InnerText))
                    {
                        //检查是否带有分页数量，如果有则传过去
                        string pagesizeStr = string.Empty;
                        if (xe.Attributes["pagesize"] != null)
                        {
                            pagesizeStr = xe.Attributes["pagesize"].InnerText;
                        }
                        //生成模板文件
                        GetTemplate(sitePath, tempPath, skinName, xe.Attributes["templet"].InnerText, xe.Attributes["page"].InnerText, xe.Attributes["inherit"].InnerText, skinName, string.Empty, pagesizeStr, 1, xe.Attributes["name"].InnerText);
                    }
                }
            }
        }

        /// <summary>
        /// 修改插件节点数据
        /// </summary>
        public bool UpdateNodeValue(string dirPath, string xPath, string value)
        {
            return XmlHelper.UpdateNodeInnerText(dirPath + DTKeys.FILE_PLUGIN_XML_CONFING, xPath, value);
        }

        /// <summary>
        /// 执行插件SQL语句
        /// </summary>
        public bool ExeSqlStrs(string dirPath, string xPath)
        {
            bool result = true;
            List<string> ls = ReadChildNodesValue(dirPath + DTKeys.FILE_PLUGIN_XML_CONFING, xPath);
            if (ls != null)
            {
                if (WriteDataBase.ExecuteSqlTran(ls) == 0)
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 执行插件SQL语句
        /// </summary>
        public bool ExeSqlStr(string dirPath, string xPath)
        {
            bool result = true;
            int count = 0;
            List<string> ls = ReadChildNodesValue(dirPath + DTKeys.FILE_PLUGIN_XML_CONFING, xPath);
            if (ls != null)
            {
                using (IDbConnection conn = new DapperView().Context())
                {
                    using (IDbTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            count = WriteDataBase.ExecuteSqlTran(conn, trans, ls);
                            trans.Commit();
                            return count > 0;
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            return false;
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 添加URL映射节点
        /// </summary>
        public bool AppendNodes(string dirPath, string xPath)
        {
            XmlNodeList xnList = XmlHelper.ReadNodes(dirPath + DTKeys.FILE_PLUGIN_XML_CONFING, xPath);
            return new url_rewrite().Import(xnList);
        }

        /// <summary>
        /// 删除URL映射节点
        /// </summary>
        public bool RemoveNodes(string dirPath, string xPath)
        {
            XmlNodeList xnList = XmlHelper.ReadNodes(dirPath + DTKeys.FILE_PLUGIN_XML_CONFING, xPath);
            return new url_rewrite().Remove(xnList);
        }

        /// <summary>
        /// 添加后台管理导航
        /// </summary>
        public bool AppendMenuNodes(string navPath, string dirPath, string xPath, string parentName)
        {
            XmlNodeList xnList = XmlHelper.ReadNodes(dirPath + DTKeys.FILE_PLUGIN_XML_CONFING, xPath);
            if (xnList.Count > 0)
            {
                foreach (XmlElement xe in xnList)
                {
                    if (xe.NodeType != XmlNodeType.Comment && xe.Name.ToLower() == "nav")
                    {
                        //插入导航记录
                        int sort_id = 99;
                        string nav_name = string.Empty;
                        string nav_title = string.Empty;
                        string link_url = string.Empty;
                        string action_type = string.Empty;
                        if (xe.Attributes["name"] != null)
                        {
                            nav_name = xe.Attributes["name"].Value;
                        }
                        if (xe.Attributes["title"] != null)
                        {
                            nav_title = xe.Attributes["title"].Value;
                        }
                        if (xe.Attributes["url"] != null)
                        {
                            link_url = navPath + xe.Attributes["url"].Value;
                        }
                        if (xe.Attributes["action"] != null)
                        {
                            action_type = xe.Attributes["action"].Value;
                        }
                        if (xe.Attributes["sort"] != null)
                        {
                            sort_id = Utils.StrToInt(xe.Attributes["sort"].Value, 99);
                        }
                        int nav_id = new navigation(databaseprefix).Add(parentName, nav_name, nav_title, link_url, sort_id, 0, action_type); //写入数据库
                        if (nav_id < 1)
                        {
                            return false;
                        }
                        //调用自身迭代
                        AppendMenuNodes(navPath, dirPath, xPath + "/nav", nav_name);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 删除后台管理导航
        /// </summary>
        public void RemoveMenuNodes(string dirPath, string xPath)
        {
            XmlNodeList xnList = XmlHelper.ReadNodes(dirPath + DTKeys.FILE_PLUGIN_XML_CONFING, xPath);
            if (xnList.Count > 0)
            {
                DAL.navigation dal = new navigation(databaseprefix);
                foreach (XmlElement xe in xnList)
                {
                    if (xe.NodeType != XmlNodeType.Comment && xe.Name.ToLower() == "nav")
                    {
                        //删除导航记录
                        if (xe.Attributes["name"] != null)
                        {
                            int nav_id = dal.GetNavId(xe.Attributes["name"].Value);
                            if (nav_id > 0)
                            {
                                dal.Delete(nav_id);
                            }
                        }
                    }
                }
            }
        }

        #region 私有方法==================================================
        /// <summary>
        /// 读取所有SQL语句子节点的的值
        /// </summary>
        private List<string> ReadChildNodesValue(string filePath, string xPath)
        {
            try
            {
                List<string> ls = new List<string>();
                XmlNodeList xnList = XmlHelper.ReadNodes(filePath, xPath);
                if (xnList.Count > 0)
                {
                    foreach (XmlElement xe in xnList)
                    {
                        if (xe.NodeType != XmlNodeType.Comment && xe.Name.ToLower() == "sql" && !string.IsNullOrEmpty(xe.InnerText))
                        {
                            ls.Add(xe.InnerText.Replace("{databaseprefix}", databaseprefix)); //替换数据库表前缀
                        }
                    }
                }
                return ls;
            }
            catch
            {
                return null;
            }
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
        /// <param name="name">插件目录</param>
        /// <returns>string值,如果失败则为"",成功则为模板内容的string</returns>
        private static string GetTemplate(string sitePath, string tempPath, string skinName, string templet, string fromPage, string inherit, string buildPath, string channelName, string pageSize, int nest, string name)
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

                //开始查找替换标签
                string[] strlist = Utils.SplitString(textOutput.ToString(), "\r");
                for (int i = 0; i < strlist.Length; i++)
                {
                    if (strlist[i] == "")
                        continue;

                    strReturn.Append(strlist[i]);//不进行标签替换 直接发挥模板的内容
                }
                //如果是第一层则写入文件
                if (nest == 1)
                {
                    //定义页面常量
                    string channelStr = string.Empty; //频道名称
                    string constStr = string.Empty; //分页大小
                    string validate_channel = string.Empty; //验证频道列表数据
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
                    string inherit_controller = string.Empty;
                    string[] strinherit = inherit.Split('.');
                    if (strinherit.Length >= 4)
                    {
                        inherit_controller = inherit;
                        inherit = string.Empty;
                        for (int i = 0; i < strinherit.Length; i++)
                        {
                            if (i <= 3)
                            {
                                inherit += strinherit[i] + ".";
                            }
                        }
                        inherit = inherit.TrimEnd('.');
                    }
                    else
                    {
                        inherit_controller = inherit + "." + name;
                    }
                    //..\packages\Microsoft.CodeDom.Providers.DotNetCompilerPlatform.1.0.5\build\net45\Microsoft.CodeDom.Providers.DotNetCompilerPlatform.props。
                    sb.Append(string.Format("<!--本页面代码由DTcms非官方Mvc版模板引擎生成于 {0} -->\r\n", DateTime.Now));
                    sb.Append(string.Format(
                      "@using System.Collections.Generic;\r\n" +
                      "@using System.Text;\r\n" +
                      "@using System.Data;\r\n" +
                      "@using DTcms.Common;\r\n" +
                      "@using DTcms.Web.Mvc.UI.Controllers;{0}\r\n", extNamespace.ToString()));
                    sb.Append(string.Format(
                      "@{{\r\n" +
                      "\tDTcms.Model.channel_site site = ViewData[\"site\"] as DTcms.Model.channel_site;\r\n" +
                      "\tDTcms.Model.siteconfig config = ViewData[\"config\"] as DTcms.Model.siteconfig;\r\n" +
                      "\tDTcms.Model.userconfig uconfig = ViewData[\"uconfig\"] as DTcms.Model.userconfig;\r\n" +
                      "\t{0}Controller c = ViewData[\"controller\"] as {0}Controller;\r\n", inherit_controller));
                    Assembly assembly = Assembly.Load(inherit);
                    Type type = assembly.GetType(inherit_controller + "Controller", true);
                    if (type != null)
                    {
                        System.Reflection.BindingFlags flag = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
                        System.Reflection.FieldInfo[] fields = type.GetFields(flag);
                        foreach (System.Reflection.FieldInfo fi in fields)
                        {
                            if (fi.Name != "channel")
                            {
                                string _str = string.Format("\t{0} {1} = c.{1};\r\n", fi.FieldType.ToString(), fi.Name);
                                _str = _str.Replace("`1[", "<").Replace("]", "]");
                                sb.Append(_str);
                            }
                        }
                    }
                    sb.Append(string.Format("\tViewBag.Title = site.seo_title;\r\n" + "}}\r\n{2}", channelStr, constStr, strReturn.ToString()));
                    template = sb.ToString();

                    string pageDir = Utils.GetMapPath(string.Format("{0}Areas/Web/Views/{1}/", sitePath, buildPath)); //生成文件的目录路径
                    string outputPath = pageDir + fromPage; //生成文件的物理路径

                    if (!Directory.Exists(pageDir)) //如果物理路径不存在则创建
                    {
                        Directory.CreateDirectory(pageDir);
                    }
                    //保存写入文件
                    File.WriteAllText(outputPath, template, Encoding.UTF8);

                }
            }

            return strReturn.ToString();
        }
        #endregion

    }
}
