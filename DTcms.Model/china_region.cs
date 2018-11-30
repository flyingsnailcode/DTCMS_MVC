using System;

namespace DTcms.Model
{
    /// <summary>
    /// 实体类
    /// <summary>
    [Serializable]
    public class china_region
    {
        public china_region() { }

        private int _id = 0;
        private int _parent_id = 0;
        private string _title = string.Empty;
        private string _short_title = string.Empty;
        private int _class_layer = 0;
        private string _pinyin = string.Empty;
        private string _jianpin = string.Empty;
        private string _fchar = string.Empty;
        private int _sort_id = 99;
        private int _is_lock = 0;

        #region Model
        /// <summary>
        /// ID号
        /// </summary>
        public int id
        {
           set { _id = value; }
           get { return _id; }
        }
        /// <summary>
        /// 父类ID
        /// </summary>
        public int parent_id
        {
           set { _parent_id = value; }
           get { return _parent_id; }
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string title
        {
           set { _title = value; }
           get { return _title; }
        }
        /// <summary>
        /// 简称
        /// </summary>
        public string short_title
        {
           set { _short_title = value; }
           get { return _short_title; }
        }
        /// <summary>
        /// 深度
        /// </summary>
        public int class_layer
        {
           set { _class_layer = value; }
           get { return _class_layer; }
        }
        /// <summary>
        /// 拼音
        /// </summary>
        public string pinyin
        {
           set { _pinyin = value; }
           get { return _pinyin; }
        }
        /// <summary>
        /// 简拼
        /// </summary>
        public string jianpin
        {
           set { _jianpin = value; }
           get { return _jianpin; }
        }
        /// <summary>
        /// 首字母
        /// </summary>
        public string fchar
        {
           set { _fchar = value; }
           get { return _fchar; }
        }
        /// <summary>
        /// 排序
        /// </summary>
        public int sort_id
        {
           set { _sort_id = value; }
           get { return _sort_id; }
        }
        /// <summary>
        /// 是否锁定
        /// </summary>
        public int is_lock
        {
           set { _is_lock = value; }
           get { return _is_lock; }
        }
        #endregion
    }
}
