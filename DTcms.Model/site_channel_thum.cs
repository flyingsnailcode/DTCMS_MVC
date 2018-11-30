using System;

namespace DTcms.Model
{
    /// <summary>
    /// 频道缩略图实体类
    /// <summary>
    [Serializable]
    public class site_channel_thum
    {
        public site_channel_thum() { }

        private int _id = 0;
        private string _title = string.Empty;
        private int _class_id = 0;
        private int _channel_id = 0;
        private int _width = 0;
        private int _height = 0;
        private int _typeid = 0;
        private int _is_lock = 0;
        private DateTime _add_time = DateTime.Now;

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
        /// 名称
        /// </summary>
        public string title
        {
            set { _title = value; }
            get { return _title; }
        }
        /// <summary>
        /// 类型 0默认，1封面
        /// </summary>
        public int class_id
        {
            set { _class_id = value; }
            get { return _class_id; }
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
        /// 宽度
        /// </summary>
        public int width
        {
           set { _width = value; }
           get { return _width; }
        }
        /// <summary>
        /// 高
        /// </summary>
        public int height
        {
           set { _height = value; }
           get { return _height; }
        }
        /// <summary>
        /// 生成方式
        /// </summary>
        public int typeid
        {
           set { _typeid = value; }
           get { return _typeid; }
        }
        /// <summary>
        /// 是否锁定
        /// </summary>
        public int is_lock
        {
           set { _is_lock = value; }
           get { return _is_lock; }
        }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime add_time
        {
           set { _add_time = value; }
           get { return _add_time; }
        }
        #endregion
    }
}
