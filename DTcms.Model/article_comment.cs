using System;
namespace DTcms.Model
{
    /// <summary>
    /// 评论表
    /// </summary>
    [Serializable]
    public partial class article_comment
    {
        public article_comment()
        { }
        #region Model
        private int _id;
        protected int _site_id;
        private int _channel_id = 0;
        private int _article_id = 0;
        private int _parent_id = 0;
        private int _user_id = 0;
        private string _user_name = string.Empty;
        private string _user_ip;
        private string _content;
        private int _is_lock = 0;
        private DateTime _add_time = DateTime.Now;
        private int _is_reply = 0;
        private string _reply_content;
        private DateTime? _reply_time;
        private string _class_list = string.Empty;
        private int _class_layer = 0;

        private string _city = string.Empty;
        private int _img_url = 0;
        private string _email = string.Empty;
        private string _web = string.Empty;
        private string _system = string.Empty;
        private int _is_email = 0;
        /// <summary>
        /// 系统类型
        /// </summary>
        public string system
        {
            set { _system = value; }
            get { return _system; }
        }
        /// <summary>
        /// 留言者邮箱
        /// </summary>
        public string email
        {
            set { _email = value; }
            get { return _email; }
        }
        /// <summary>
        /// 留言者网站
        /// </summary>
        public string web
        {
            set { _web = value; }
            get { return _web; }
        }
        /// <summary>
        /// 评论者随机头像
        /// </summary>
        public int img_url
        {
            set { _img_url = value; }
            get { return _img_url; }
        }
        /// <summary>
        /// 城市
        /// </summary>
        public string city
        {
            set { _city = value; }
            get { return _city; }
        }

        /// <summary>
        /// 站点ID
        /// </summary>
        public int site_id
        {
            set { _site_id = value; }
            get { return _site_id; }
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
        /// 主表ID
        /// </summary>
        public int article_id
        {
            set { _article_id = value; }
            get { return _article_id; }
        }
        /// <summary>
        /// 父评论ID
        /// </summary>
        public int parent_id
        {
            set { _parent_id = value; }
            get { return _parent_id; }
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
        /// 用户名
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
        /// 评论内容
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
        /// 发表时间
        /// </summary>
        public DateTime add_time
        {
            set { _add_time = value; }
            get { return _add_time; }
        }
        /// <summary>
        /// 是否已答复
        /// </summary>
        public int is_reply
        {
            set { _is_reply = value; }
            get { return _is_reply; }
        }
        /// <summary>
        /// 答复内容
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
        /// 接收回复邮件通知
        /// </summary>
        public int is_email
        {
            set { _is_email = value; }
            get { return _is_email; }
        }
        #endregion Model

    }
}