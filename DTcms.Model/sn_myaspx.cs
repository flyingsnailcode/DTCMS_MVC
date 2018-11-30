using System;

namespace DTcms.Model
{
    /// <summary>
    /// 网站授权
    /// <summary>
    [Serializable]
    public class sn_myaspx
    {
        public sn_myaspx() { }

        private int _id = 0;
        private string _action = string.Empty;
        private string _sn = string.Empty;
        private string _idn = string.Empty;
        private string _ip = string.Empty;
        private string _sid = string.Empty;
        private int _is_lock = 0;
        private string _remark = string.Empty;
        private DateTime? _star_time;
        private DateTime? _end_time;
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
        /// 操作类型
        /// </summary>
        public string action
        {
            set { _action = value; }
            get { return _action; }
        }
        /// <summary>
        /// 序列号
        /// </summary>
        public string sn
        {
            set { _sn = value; }
            get { return _sn; }
        }
        /// <summary>
        /// 授权域名
        /// </summary>
        public string idn
        {
            set { _idn = value; }
            get { return _idn; }
        }
        /// <summary>
        /// 授权IP
        /// </summary>
        public string ip
        {
            set { _ip = value; }
            get { return _ip; }
        }
        /// <summary>
        /// 安全码加密
        /// </summary>
        public string sid
        {
            set { _sid = value; }
            get { return _sid; }
        }
        /// <summary>
        /// 是否已经使用  0：未启用  1：启用
        /// </summary>
        public int is_lock
        {
            set { _is_lock = value; }
            get { return _is_lock; }
        }
        
        /// <summary>
        /// 授权开始时间
        /// </summary>
        public DateTime? star_time
        {
            set { _star_time = value; }
            get { return _star_time; }
        }

        /// <summary>
        /// 授权结束时间
        /// </summary>
        public DateTime? end_time
        {
            set { _end_time = value; }
            get { return _end_time; }
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
        /// 发送时间
        /// </summary>
        public DateTime add_time
        {
            set { _add_time = value; }
            get { return _add_time; }
        }
        #endregion
    }
}