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
        /// 返回支付列表
        /// </summary>
        /// <param name="top">显示条数</param>
        /// <param name="strwhere">查询条件</param>
        /// <returns>DataTable</returns>
        public List<Model.payment> get_payment_list(int top, string strwhere)
        {
            List<Model.payment> list = new List<Model.payment>();
            string _where = "is_lock=0";
            if (!string.IsNullOrEmpty(strwhere))
            {
                _where += " and " + strwhere;
            }
            list = new BLL.payment().GetModelList(top, _where, "*", "sort_id asc,id desc");
            return list;
        }

        /// <summary>
        /// 返回支付类型的标题
        /// </summary>
        /// <param name="payment_id">ID</param>
        /// <returns>String</returns>
        public string get_payment_title(int payment_id)
        {
            return new BLL.payment().GetTitle(payment_id);
        }

        /// <summary>
        /// 返回支付费用金额
        /// </summary>
        /// <param name="payment_id">支付ID</param>
        /// <param name="total_amount">总金额</param>
        /// <returns>decimal</returns>
        public decimal get_payment_poundage_amount(int payment_id, decimal total_amount)
        {
            Model.payment payModel = new BLL.payment().Get(payment_id);
            if (payModel == null)
            {
                return 0;
            }
            decimal poundage_amount = payModel.poundage_amount;
            if (payModel.poundage_type == 1)
            {
                poundage_amount = (poundage_amount * total_amount) / 100;
            }
            return poundage_amount;
        }
    }
}
