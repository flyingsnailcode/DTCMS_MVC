using System;

namespace DTcms.Model
{
    /// <summary>
    /// 模型层
    /// <summary>
    [Serializable]
    public class article_attribute
    {
        public article_attribute() { }

        private int _article_id = 0;
        private string _title;
        private string _value;
        private int _channel_id = 0;
        private int _status = 0;
        #region Model
        /// <summary>
        /// 状态
        /// </summary>
        public int status
        {
            set { _status = value; }
            get { return _status; }
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
        /// 新闻ID
        /// </summary>
        public int article_id
        {
            set { _article_id = value; }
            get { return _article_id; }
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
        /// 值
        /// </summary>
        public string value
        {
            set { _value = value; }
            get { return _value; }
        }

        #endregion
    }
}
