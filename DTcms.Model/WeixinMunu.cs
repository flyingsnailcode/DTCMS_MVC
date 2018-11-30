
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DTcms.Model
{
    /// <summary>
    /// 微信菜单信息(临时采用的方法,完善代码后应删除)
    /// </summary>
    public class WeixinMunu
    {
        private string _name = string.Empty;
        private string _key = string.Empty;
        private string _url = string.Empty;
        public WeixinMunu()
        {

        }

        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string key
        {
            get { return _key; }
            set { _key = value; }
        }

        public string url
        {
            get { return _url; }
            set { _url = value; }
        }
    }
}