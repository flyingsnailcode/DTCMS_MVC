using Dapper;
using System;
using System.Collections.Generic;

namespace DTcms.Model
{
    /// <summary>
    /// 商品规格
    /// </summary>
    [Serializable]
    public partial class article_spec
    {
        public article_spec()
        { }
        #region Model
        private int _id;
        private int _channel_id = 0;
        private int _parent_id = 0;
        private string _title = string.Empty;
        private string _img_url = string.Empty;
        private string _remark = string.Empty;
        private int _sort_id = 99;
        /// <summary>
        /// 自增ID
        /// </summary>
        public int id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 频道ID
        /// </summary>
        public int channel_id
        {
            set { _channel_id = value; }
            get { return _channel_id; }
        }
        /// <summary>
        /// 父节点ID
        /// </summary>
        public int parent_id
        {
            set { _parent_id = value; }
            get { return _parent_id; }
        }
        /// <summary>
        /// 规格标题
        /// </summary>
        public string title
        {
            set { _title = value; }
            get { return _title; }
        }
        /// <summary>
        /// 规格图片
        /// </summary>
        public string img_url
        {
            set { _img_url = value; }
            get { return _img_url; }
        }
        /// <summary>
        /// 备注说明
        /// </summary>
        public string remark
        {
            set { _remark = value; }
            get { return _remark; }
        }
        /// <summary>
        /// 排序数字
        /// </summary>
        public int sort_id
        {
            set { _sort_id = value; }
            get { return _sort_id; }
        }
        #endregion Model

        #region 非数据库字段
        /// <summary>
        /// 规格值
        /// </summary>
        [ResultColumn, Ignore]
        public List<article_spec_value> values { get; set; }
        #endregion
    }

    /// <summary>
    /// 商品规格值
    /// </summary>
    [Serializable]
    public partial class article_spec_value
    {
        public article_spec_value()
        { }
        #region Model
        private int _id;
        private int _parent_id = 0;
        private string _title;
        private string _img_url;
        private int _sort_id = 99;
        /// <summary>
        /// 自增ID
        /// </summary>
        public int id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 父节点ID
        /// </summary>
        public int parent_id
        {
            set { _parent_id = value; }
            get { return _parent_id; }
        }
        /// <summary>
        /// 规格标题
        /// </summary>
        public string title
        {
            set { _title = value; }
            get { return _title; }
        }
        /// <summary>
        /// 规格图片
        /// </summary>
        public string img_url
        {
            set { _img_url = value; }
            get { return _img_url; }
        }
        /// <summary>
        /// 排序数字
        /// </summary>
        public int sort_id
        {
            set { _sort_id = value; }
            get { return _sort_id; }
        }
        #endregion Model
    }

}