using DTcms.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DTcms.Web.Mvc.UI.Controllers
{
    public partial class BaseController : Controller
    {
        /// <summary>
        /// 返回配送列表
        /// </summary>
        /// <param name="top">显示条数</param>
        /// <param name="strwhere">查询条件</param>
        /// <returns>DataTable</returns>
        public List<Model.express> get_express_list(int top, string strwhere)
        {
            List<Model.express> list = new List<Model.express>();
            string _where = "is_lock=0";
            if (!string.IsNullOrEmpty(strwhere))
            {
                _where += " and " + strwhere;
            }
            list = new BLL.express().GetModelList(top, _where, "*", "sort_id asc,id desc");
            return list;
        }

        /// <summary>
        /// 返回配送方式的标题
        /// </summary>
        /// <param name="payment_id">ID</param>
        /// <returns>String</returns>
        public string get_express_title(int express_id)
        {
            return new BLL.express().GetTitle(express_id);
        }
    }
}
