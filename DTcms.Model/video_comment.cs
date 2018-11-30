using System;

namespace DTcms.Model
{
    /// <summary>
    /// 模型层
    /// <summary>
    [Serializable]
    public class video_comment
    {
        public video_comment() { }

        private int _id = 0;
        private int _channel_id = 0;
        private int _article_id = 0;
        private int _parent_id = 0;
        private string _class_list = string.Empty;
        private int _class_layer = 0;
        private int _user_id = 0;
        private string _user_name = string.Empty;
        private string _user_ip = string.Empty;
        private string _content = string.Empty;
        private int _is_lock = 0;
        private DateTime _add_time = DateTime.Now;
        private int _is_reply = 0;
        private string _reply_content = string.Empty;
        private DateTime? _reply_time;
        private int _star = 0;
        private string _time = string.Empty;
        private string _color = string.Empty;
        private string _type = string.Empty;

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
        /// 父类ID
        /// </summary>
        public int parent_id
        {
           set { _parent_id = value; }
           get { return _parent_id; }
        }
        /// <summary>
        /// 分类列表
        /// </summary>
        public string class_list
        {
           set { _class_list = value; }
           get { return _class_list; }
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
        /// 用户ID
        /// </summary>
        public int user_id
        {
           set { _user_id = value; }
           get { return _user_id; }
        }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string user_name
        {
           set { _user_name = value; }
           get { return _user_name; }
        }
        /// <summary>
        /// 用户IP
        /// </summary>
        public string user_ip
        {
           set { _user_ip = value; }
           get { return _user_ip; }
        }
        /// <summary>
        /// 内容
        /// </summary>
        public string content
        {
           set { _content = value; }
           get { return _content; }
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
        /// <summary>
        /// 是否回复
        /// </summary>
        public int is_reply
        {
           set { _is_reply = value; }
           get { return _is_reply; }
        }
        /// <summary>
        /// 回复内容
        /// </summary>
        public string reply_content
        {
           set { _reply_content = value; }
           get { return _reply_content; }
        }
        /// <summary>
        /// 回复时间
        /// </summary>
        public DateTime? reply_time
        {
           set { _reply_time = value; }
           get { return _reply_time; }
        }
        /// <summary>
        /// 星级
        /// </summary>
        public int star
        {
           set { _star = value; }
           get { return _star; }
        }
        /// <summary>
        /// 视屏评论的时间段
        /// </summary>
        public string time
        {
           set { _time = value; }
           get { return _time; }
        }
        /// <summary>
        /// 颜色
        /// </summary>
        public string color
        {
           set { _color = value; }
           get { return _color; }
        }
        /// <summary>
        /// 类型
        /// </summary>
        public string type
        {
           set { _type = value; }
           get { return _type; }
        }

        #endregion
    }
}
