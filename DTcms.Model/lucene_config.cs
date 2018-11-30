using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTcms.Model
{
    public class luceneconfig
    {
        private int _id = 0;
        private int _status = 0;
        private string _name = string.Empty;
        private DateTime? _update;

        /// <summary>
        /// ID
        /// </summary>
        public int id
        {
            set { _id = value; }
            get { return _id; }
        }

        /// <summary>
        /// 状态
        /// </summary>
        public int status
        {
            set { _status = value; }
            get { return _status; }
        }

        /// <summary>
        /// 频道名称
        /// </summary>
        public string name
        {
            set { _name = value; }
            get { return _name; }
        }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? update
        {
            set { _update = value; }
            get { return _update; }
        }
    }
}