using System;
using System.Collections.Generic;

namespace DTcms.Model
{
    /// <summary>
    /// 模板实体类
    /// </summary>
    [Serializable]
    public class template
    {
        private int _apply = 0;
        private string _version = "";
        private string _preview = "";
        private string _name = "";
        private string _demo = "";
        private string _author = "";
        private string _description = "";
        private string _website = "";
        private string _createdate = "";

        /// <summary>
        /// 模板适用范围 0通用、1PC、2移动、3微信
        /// </summary>
        public int apply
        {
            get { return _apply; }
            set { _apply = value; }
        }
        /// <summary>
        /// 模板版本
        /// </summary>
        public string version
        {
            get { return _version; }
            set { _version = value; }
        }
        /// <summary>
        /// 缩略图
        /// </summary>
        public string preview
        {
            get { return _preview; }
            set { _preview = value; }
        }
        /// <summary>
        /// 模板名称
        /// </summary>
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// 演示网址
        /// </summary>
        public string demo
        {
            get { return _demo; }
            set { _demo = value; }
        }
        /// <summary>
        /// 作者
        /// </summary>
        public string author
        {
            get { return _author; }
            set { _author = value; }
        }
        /// <summary>
        /// 网站域名
        /// </summary>
        public string website
        {
            get { return _website; }
            set { _website = value; }
        }
        /// <summary>
        /// 模板适用的版本
        /// </summary>
        public string description
        {
            get { return _description; }
            set { _description = value; }
        }
        /// <summary>
        /// 创建日期
        /// </summary>
        public string createdate
        {
            get { return _createdate; }
            set { _createdate = value; }
        }
    }
}
