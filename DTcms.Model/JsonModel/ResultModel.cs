using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DTcms.Model
{
    public class ResultModel
    {
        private string _Msg = "操作成功";
        /// <summary>
        /// 返回状态
        /// </summary>
        public string msg { get { return _Msg; } set { _Msg = value; } }

        private object _Status = string.Empty;
        /// <summary>
        /// 返回信息
        /// </summary>
        public object status { get { return _Status; } set { _Status = value; } }
        
        private string _Url = "";
        /// <summary>
        /// 返回链接
        /// </summary>
        public string url { get { return _Url; } set { _Url = value; } }
    }

    public class SaveModel
    {
        private string _Msg = "操作成功";
        private int _Status = 0;
        private int _ID = 0;

        /// <summary>
        /// 返回状态
        /// </summary>
        public string msg { get { return _Msg; } set { _Msg = value; } }

        /// <summary>
        /// 返回信息
        /// </summary>
        public int status { get { return _Status; } set { _Status = value; } }

        /// <summary>
        /// ID
        /// </summary>
        public int id { get { return _ID; } set { _ID = value; } }
        
        private string _Url = "";
        /// <summary>
        /// 返回链接
        /// </summary>
        public string url { get { return _Url; } set { _Url = value; } }
    }
}