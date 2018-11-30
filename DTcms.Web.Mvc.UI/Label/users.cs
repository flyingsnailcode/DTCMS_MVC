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
        /// OAuth平台列表
        /// </summary>
        /// <param name="top">显示条数</param>
        /// <param name="strwhere">查询条件</param>
        /// <returns>DataTable</returns>
        public DataTable get_site_oauth_list(int top, string strwhere)
        {
            string _where = "is_lock=0";
            if (!string.IsNullOrEmpty(strwhere))
            {
                _where += " and " + strwhere;
            }
            return new BLL.site_oauth().GetList(top, _where).Tables[0];
        }
        /// <summary>
        /// 返回用户头像图片地址
        /// </summary>
        /// <param name="user_name">用户名</param>
        /// <returns>String</returns>
        public string get_user_avatar(string user_name)
        {
            string avatar = string.Empty;
            BLL.users bll = new BLL.users();
            if (!bll.Exists(user_name))
            {
                return "";
            }
            avatar = bll.GetModel("user_name='"+ user_name + "' and status<3", "top 1 *", "").avatar;
            if (avatar == "")
            {
                avatar = "/images/noavatar_middle.gif";
            }
            return avatar;
        }

        /// <summary>
        /// 返回用户头像昵称
        /// </summary>
        /// <param name="user_name">用户名</param>
        /// <returns>String</returns>
        public string get_user_nick_name(string user_name)
        {
            BLL.users bll = new BLL.users();
            if (!bll.Exists(user_name))
            {
                return "";
            }
            return bll.GetModel("user_name='" + user_name + "' and status<3", "top 1 *", "").nick_name;
        }

        /// <summary>
        /// 返回用户头像图片地址
        /// </summary>
        /// <param name="user_name">用户ID</param>
        /// <returns>String</returns>
        public string get_user_avatar(int user_id)
        {
            BLL.users bll = new BLL.users();
            if (!bll.Exists(user_id))
            {
                return "";
            }
            return bll.Get(user_id).avatar;
        }

        /// <summary>
        /// 统计短信息数量
        /// </summary>
        /// <param name="strwhere">查询条件</param>
        /// <returns>Int</returns>
        public int get_user_message_count(string strwhere)
        {
            return int.Parse(new BLL.user_message().GetCount("count(*) as H", strwhere));
        }

        /// <summary>
        /// 统计发布供求数量
        /// </summary>
        /// <param name="strwhere">查询条件</param>
        /// <returns>Int</returns>
        public int get_user_live_count(string channelName, string strwhere)
        {
            return new BLL.article().GetCount(channelName, strwhere);
        }

        /// <summary>
        /// 短信息列表
        /// </summary>
        /// <param name="top">显示条数</param>
        /// <param name="strwhere">查询条件</param>
        /// <returns>DataTable</returns>
        public List<Model.user_message> get_user_message_list(int top, string strwhere)
        {
            return new BLL.user_message().GetModelList(top, strwhere, "*", "id asc,post_time desc");
        }

        /// <summary>
        /// 短信息分页列表
        /// </summary>
        /// <param name="page_size">页面大小</param>
        /// <param name="page_index">当前页码</param>
        /// <param name="strwhere">查询条件</param>
        /// <param name="totalcount">总记录数</param>
        /// <returns>DateTable</returns>
        public List<Model.user_message> get_user_message_list(int page_size, int page_index, string strwhere, out int totalcount)
        {
            return new BLL.user_message().GetList(page_size, page_index, strwhere, "is_read asc,post_time desc", out totalcount);
        }

        /// <summary>
        /// 积分明细分页列表
        /// </summary>
        /// <param name="page_size">页面大小</param>
        /// <param name="page_index">当前页码</param>
        /// <param name="strwhere">查询条件</param>
        /// <param name="totalcount">总记录数</param>
        /// <returns>DateTable</returns>
        public List<Model.user_point_log> get_user_point_list(int page_size, int page_index, string strwhere, out int totalcount)
        {
            return new BLL.user_point_log().GetList(page_size, page_index, strwhere, "add_time desc,id desc", out totalcount);
        }

        /// <summary>
        /// 余额明细分页列表
        /// </summary>
        /// <param name="page_size">页面大小</param>
        /// <param name="page_index">当前页码</param>
        /// <param name="strwhere">查询条件</param>
        /// <param name="totalcount">总记录数</param>
        /// <returns>DateTable</returns>
        public List<Model.user_amount_log> get_user_amount_list(int page_size, int page_index, string strwhere, out int totalcount)
        {
            return new BLL.user_amount_log().GetList(page_size, page_index, strwhere, "add_time desc,id desc", out totalcount);
        }

        /// <summary>
        /// 充值记录分页列表
        /// </summary>
        /// <param name="page_size">页面大小</param>
        /// <param name="page_index">当前页码</param>
        /// <param name="strwhere">查询条件</param>
        /// <param name="totalcount">总记录数</param>
        /// <returns>DateTable</returns>
        public List<Model.user_recharge> get_user_recharge_list(int page_size, int page_index, string strwhere, out int totalcount)
        {
            return new BLL.user_recharge().GetList(page_size, page_index, strwhere, "add_time desc,id desc", out totalcount);
        }

        /// <summary>
        /// 用户邀请码列表
        /// </summary>
        /// <param name="top">显示条数</param>
        /// <param name="strwhere">查询条件</param>
        /// <returns></returns>
        public List<Model.user_code> get_user_invite_list(int top, string strwhere)
        {
            string _where = "type='" + DTEnums.CodeEnum.Register.ToString() + "'";
            if (!string.IsNullOrEmpty(strwhere))
            {
                _where += " and " + strwhere;
            }
            return new BLL.user_code().GetModelList(top, _where, "*", "add_time desc,id desc");
        }
        /// <summary>
        /// 返回邀请码状态
        /// </summary>
        /// <param name="str_code">邀请码</param>
        /// <returns>bool</returns>
        public bool get_invite_status(string str_code)
        {
            Model.user_code model = new BLL.user_code().GetModel("status=0 and datediff(d,eff_time,getdate())<=0 and str_code='" + str_code + "'", "top 1 *", "");
            if (model != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 收货地址列表
        /// </summary>
        /// <param name="top">显示条数</param>
        /// <param name="strwhere">查询条件</param>
        /// <returns>DataTable</returns>
        public List<Model.user_addr_book> get_user_addr_book_list(int top, string strwhere)
        {
            return new BLL.user_addr_book().GetModelList(top, strwhere, "*", "is_default desc,id asc");
        }

        /// <summary>
        /// 收货地址分页列表
        /// </summary>
        /// <param name="page_size">页面大小</param>
        /// <param name="page_index">当前页码</param>
        /// <param name="strwhere">查询条件</param>
        /// <param name="totalcount">总记录数</param>
        /// <returns>DateTable</returns>
        public List<Model.user_addr_book> get_user_addr_book_list(int page_size, int page_index, string strwhere, out int totalcount)
        {
            return new BLL.user_addr_book().GetList(page_size, page_index, strwhere, "is_default desc,id asc", out totalcount);
        }

        public DataTable get_user_add_article_list(string channel_name, int page_size, int page_index, string strwhere, out int totalcount)
        {
            return new BLL.article().GetList(channel_name, 0, page_size, page_index, strwhere, "add_time desc,id asc", out totalcount).Tables[0];
        }

        #region  评论
        /// <summary>
        /// 用户评论分页列表
        /// </summary>
        /// <param name="page_size">页面大小</param>
        /// <param name="page_index">当前页码</param>
        /// <param name="strwhere">查询条件</param>
        /// <param name="totalcount">总记录数</param>
        /// <returns>DateTable</returns>
        public List<Model.article_comment> get_user_comment(string channel_name, int user_id, bool relation, int page_size, int page_index, string strwhere, out int totalcount)
        {
            return new BLL.article_comment().GetList(channel_name, user_id, relation, page_size, page_index, strwhere, "add_time desc,id asc", out totalcount);
        }

        #endregion
    }
}
