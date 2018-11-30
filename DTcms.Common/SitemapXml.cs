using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DTcms.Common
{
    public class SitemapXml
    {
        private const string Xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        private const string XmlnsXsi = "http://www.w3.org/2001/XMLSchema-instance";
        private const string XsiSchemaLocation = "http://www.sitemaps.org/schemas/sitemap/0.9  http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd";

        /// <summary>  
        /// 生成SiteMap地图  
        /// </summary>  
        /// <param name="siteMaps">需要生成的 对象列表</param>  
        /// <param name="saveFileName">设置文件保存名称</param>  
        /// <param name="changefreq">更新周期</param>  
        /// <param name="savePath">xml文件保存路径</param>  
        /// <returns></returns>  
        public static bool CreateSiteMapXml(List<SiteMap> siteMaps, string savePath = "/", string saveFileName = "sitemap", string changefreq = "weekly")
        {
            //保存创建好的XML文档  
            string filename = saveFileName + ".xml";
            string path = System.Web.HttpContext.Current.Server.MapPath(savePath) + filename;

            //先创建XML,返回路径  
            var xmldoc = new XmlDocument();
            //加入XML的声明段落,<?xml version="1.0" encoding="gb2312"?>  
            XmlDeclaration xmldecl = xmldoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmldoc.AppendChild(xmldecl);

            //加入一个根元素  
            XmlNode xmlelem = xmldoc.CreateElement("", "urlset", "");
            //添加属性  
            XmlAttribute attr = xmldoc.CreateAttribute("xmlns");
            attr.Value = Xmlns;
            if (xmlelem.Attributes != null) xmlelem.Attributes.SetNamedItem(attr);

            attr = xmldoc.CreateAttribute("xmlns:xsi");
            attr.Value = XmlnsXsi;
            if (xmlelem.Attributes != null) xmlelem.Attributes.SetNamedItem(attr);

            attr = xmldoc.CreateAttribute("xsi:schemaLocation");
            attr.Value = XsiSchemaLocation;
            if (xmlelem.Attributes != null) xmlelem.Attributes.SetNamedItem(attr);

            xmldoc.AppendChild(xmlelem);
            string lastmod = DateTime.Now.ToString("yyyy-MM-dd");
            for (int i = 0; i < siteMaps.Count; i++)
            {
                XmlNode root = xmldoc.SelectSingleNode("urlset");//查找<urlset>   
                if (root == null)
                {
                    //加入一个根元素  
                    xmlelem = xmldoc.CreateElement("", "urlset", "");
                    //添加属性  
                    attr = xmldoc.CreateAttribute("xmlns");
                    attr.Value = Xmlns;
                    if (xmlelem.Attributes != null) xmlelem.Attributes.SetNamedItem(attr);

                    attr = xmldoc.CreateAttribute("xmlns:xsi");
                    attr.Value = XmlnsXsi;
                    if (xmlelem.Attributes != null) xmlelem.Attributes.SetNamedItem(attr);

                    attr = xmldoc.CreateAttribute("xsi:schemaLocation");
                    attr.Value = XsiSchemaLocation;
                    if (xmlelem.Attributes != null) xmlelem.Attributes.SetNamedItem(attr);

                    xmldoc.AppendChild(xmlelem);
                    i = 0;
                    continue;
                }
                XmlElement xe1 = xmldoc.CreateElement("url");//创建一个<url>节点   

                XmlElement xmlelem1 = xmldoc.CreateElement("", "loc", "");
                XmlText xmltext = xmldoc.CreateTextNode(siteMaps[i].Loc);
                xmlelem1.AppendChild(xmltext);
                xe1.AppendChild(xmlelem1);

                xmlelem1 = xmldoc.CreateElement("", "priority", "");
                xmltext = xmldoc.CreateTextNode(siteMaps[i].Priority);
                xmlelem1.AppendChild(xmltext);
                xe1.AppendChild(xmlelem1);

                xmlelem1 = xmldoc.CreateElement("", "lastmod", "");
                xmltext = xmldoc.CreateTextNode(lastmod);
                xmlelem1.AppendChild(xmltext);
                xe1.AppendChild(xmlelem1);

                xmlelem1 = xmldoc.CreateElement("", "changefreq", "");
                xmltext = xmldoc.CreateTextNode(changefreq);
                xmlelem1.AppendChild(xmltext);
                xe1.AppendChild(xmlelem1);

                root.AppendChild(xe1);//添加到<urlset>节点中   
            }
            try
            {
                //然后在保存到源位置  
                xmldoc.AppendChild(xmlelem);
                xmldoc.Save(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    /// <summary>  
    ///   
    /// <url>  
    /// <loc>http://news.chinahbnet.com/2014/8/19/15352.html</loc>  
    /// <priority>0.5</priority>  
    /// <lastmod>2014-08-19</lastmod>  
    /// <changefreq>weekly</changefreq>  
    /// </url>  
    ///   
    /// </summary>  
    public class SiteMap
    {
        /// <summary>  
        /// 链接地址  
        /// 如：http://news.chinahbnet.com  
        /// </summary>  
        public string Loc { get; set; }
        /// <summary>  
        /// 网页权重  
        /// 0.1 - 1  
        /// </summary>  
        public string Priority { get; set; }
        /// <summary>  
        /// 生成日期  
        /// 2014-08-19  
        /// </summary>  
        public string Lastmod { get; set; }
        /// <summary>  
        /// 更新周期  
        /// always  经常  
        /// hourly  每小时  
        /// daily   每天  
        /// weekly  每周  
        /// monthly 每月  
        /// yearly  每年  
        /// never   从不  
        /// </summary>  
        public string Changefreq { get; set; }
    }
}
