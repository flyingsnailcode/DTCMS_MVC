using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using DTcms.Common;

namespace DTcms.BLL
{
    public class luceneconfig
    {
        /// <summary>
        /// XML文件的物理路径
        /// </summary>
        private string xmlpath;
        /// <summary>
        /// Xml文档
        /// </summary>
        private XmlDocument doc = new XmlDocument();

        public luceneconfig()
        {
            this.xmlpath = Utils.GetXmlMapPath(DTKeys.FILE_LUCENE_XML_CONFING);
            //判断文件是否存在
            try
            {
                doc.Load(this.xmlpath);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <returns></returns>
        public List<Model.luceneconfig> Load()
        {
            List<Model.luceneconfig> list = new List<Model.luceneconfig>();
            XmlNode root = doc.SelectSingleNode("Lucene");
            XmlNodeList xnList = root.ChildNodes;
            if (xnList.Count > 0)
            {
                for (int i = 0; i < xnList.Count; i++)
                {
                    XmlElement xe = (XmlElement)xnList.Item(i);
                    Model.luceneconfig model = new Model.luceneconfig();
                    model.name = xe.Name;
                    model.id = Utils.StrToInt(xe.SelectSingleNode("lastid").InnerText, 0);
                    model.status = Utils.StrToInt(xe.SelectSingleNode("status").InnerText, 0);
                    string time = xe.SelectSingleNode("lasttime").InnerText;
                    if (null != time)
                    {
                        DateTime t;
                        if (DateTime.TryParse(time, out t))
                        {
                            model.update = t;
                        }
                    }
                    list.Add(model);
                }
            }
            return list;
        }

        /// <summary>
        /// 保存配置文件
        /// </summary>
        /// <returns></returns>
        public bool Save(List<Model.luceneconfig> list)
        {
            try
            {
                //移除所有的子节点重新添加
                XmlNode root = doc.SelectSingleNode("Lucene");
                XmlNodeList xnList = root.ChildNodes;
                if (xnList.Count > 0)
                {
                    for (int i = xnList.Count - 1; i >= 0; i--)
                    {
                        root.RemoveChild((XmlElement)xnList.Item(i));
                    }
                }
                //重新写入所有节点
                foreach (Model.luceneconfig model in list)
                {
                    XmlNode node = root.SelectSingleNode(model.name);
                    if (null == node)
                    {
                        node = doc.CreateNode(XmlNodeType.Element, model.name, null);
                        CreateNode(doc, node, "status", model.status.ToString());
                        CreateNode(doc, node, "lastid", model.id.ToString());
                        CreateNode(doc, node, "lasttime", model.update.ToString());
                        root.AppendChild(node);
                    }
                    else
                    {
                        node.SelectSingleNode("status").InnerText = model.status.ToString();
                        node.SelectSingleNode("lastid").InnerText = model.id.ToString();
                        node.SelectSingleNode("lasttime").InnerText = model.update.ToString();
                    }
                }
                doc.Save(this.xmlpath);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error("写入Lucene.Net配置文件失败", ex);
            }
            return false;
        }

        /// <summary>    
        /// 创建节点    
        /// </summary>    
        /// <param name="xmldoc">xml文档</param>  
        /// <param name="parentnode">父节点</param>  
        /// <param name="name">节点名</param>  
        /// <param name="value">节点值</param>
        ///   
        public void CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value)
        {
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
            node.InnerText = value;
            parentNode.AppendChild(node);
        }

        /// <summary>
        /// 获得节点内容
        /// </summary>
        /// <param name="name">节点名</param>
        /// <returns></returns>
        public string GetText(string node)
        {
            return GetText(node, false);
        }

        /// <summary>
        /// 获得节点内容
        /// </summary>
        /// <param name="name">节点名</param>
        /// <param name="is_cdata">是不是CDATA数据</param>
        /// <returns></returns>
        public string GetText(string node, bool isCDATA)
        {
            XmlNode _Node = doc.SelectSingleNode(node);
            if (_Node != null)
            {
                if (isCDATA)
                {
                    return _Node.FirstChild.InnerText;
                }
                else
                {
                    return _Node.InnerText;
                }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 更新节点内容
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="_Content">内容</param>
        public void Update(string node, string content)
        {
            Update(node, content, false);
        }

        /// <summary>
        /// 更新节点内容。
        /// </summary>
        /// <param name="_XmlPathNode"></param>
        /// <param name="_Content">内容</param>
        /// <param name="isCDATA">是不是CDATA数据</param>
        public void Update(string node, string content, bool isCDATA)
        {
            if (isCDATA)
            {
                doc.SelectSingleNode(node).FirstChild.InnerText = content;
            }
            else
            {
                doc.SelectSingleNode(node).InnerText = content;
            }
        }

        /// <summary>
        /// 保存文档。
        /// </summary>
        public void Save()
        {
            try
            {
                doc.Save(this.xmlpath);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 销毁对象。
        /// </summary>
        public void Dispose()
        {
            doc = null;
        }
    }
}