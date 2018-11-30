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
        #region
        /// <summary>
        /// 返回当前类别名称
        /// </summary>
        /// <param name="category_id">类别ID</param>
        /// <returns>String</returns>
        public string get_category_title(int category_id)
        {
            return get_category_title(category_id, string.Empty);
        }

        /// <summary>
        /// 返回当前类别名称
        /// </summary>
        /// <param name="category_id">类别ID</param>
        /// <param name="default_value">默认名称</param>
        /// <returns></returns>
        public string get_category_title(int category_id, string default_value)
        {
            BLL.article_category bll = new BLL.article_category();
            if (bll.Exists(category_id))
            {
                return bll.GetTitle(category_id);
            }
            return default_value;
        }

        /// <summary>
        /// 返回类别一个实体类
        /// </summary>
        /// <param name="category_id">类别ID</param>
        /// <returns>Model.category</returns>
        public Model.article_category get_category_model(int category_id)
        {
            return new BLL.article_category().GetModel(category_id);
        }

        /// <summary>
        /// 返回类别导航面包屑
        /// </summary>
        /// <param name="urlKey">URL重写Name</param>
        /// <param name="category_id">类别ID</param>
        /// <param name="mark">标记</param>
        /// <returns>String</returns>
        public string get_category_menu(string urlKey, int category_id, string mark)
        {
            StringBuilder strTxt = new StringBuilder();
            if (category_id > 0)
            {
                LoopChannelMenu(strTxt, urlKey, mark, category_id);
            }
            return strTxt.ToString();
        }

        /// <summary>
        /// 返回类别列表
        /// </summary>
        /// <param name="channel_name">频道名称</param>
        /// <param name="parent_id">父类别ID</param>
        /// <returns>DataTable</returns>
        public DataTable get_category_list(string channel_name, int parent_id)
        {
            return new BLL.article_category().GetList(parent_id, channel_name);
        }

        /// <summary>
        /// 返回指定类别下列表(一层)
        /// </summary>
        /// <param name="channel_name">频道名称</param>
        /// <param name="parent_id">父类别ID</param>
        /// <returns>DataTable</returns>
        public DataTable get_category_child_list(string channel_name, int parent_id)
        {
            return new BLL.article_category().GetChildList(parent_id, channel_name);
        }
        public List<Model.article_category> get_category_child_list(string channel_name, int parent_id, int top)
        {
            int channel_id = new BLL.site_channel().GetChannelId(channel_name);
            return new BLL.article_category().GetModelList(top, "channel_id=" + channel_id + " and parent_id=" + parent_id + "", "*", "sort_id asc,id desc");
        }

        public string get_category_count(string channel_name, int category_id)
        {
            return new BLL.article_category().GetCount(channel_name, category_id).ToString();
        }

        /// <summary>
        /// 返回指定类别同级列表
        /// </summary>
        /// <param name="channel_name">频道名称</param>
        /// <param name="category_id">当前类别ID</param>
        /// <returns>DataTable</returns>
        public DataTable get_category_current_list(string channel_name, int category_id)
        {
            BLL.article_category bll = new BLL.article_category();
            Model.article_category model = new Model.article_category();
            model = bll.GetModel(category_id);
            return new BLL.article_category().GetChildList(model.parent_id, channel_name);
        }

        /// <summary>
        /// 返回指定类别的上一级ID
        /// </summary>
        /// <param name="category_id">类别ID</param>
        /// <returns>DataTable</returns>
        public int get_category_parent_id(int category_id)
        {
            return new BLL.article_category().GetParentId(category_id);
        }
        /// <summary>
        /// 返回指定类别的父节点ID
        /// </summary>
        /// <param name="category_id">类别ID</param>
        /// <returns>DataTable</returns>
        public int get_category_parent_first_id(int category_id)
        {
            BLL.article_category bll = new BLL.article_category();
            Model.article_category model = bll.GetModel(category_id);
            string[] res = model.class_list.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (model.parent_id == 0)
            {
                return model.id;
            }
            else if (model.parent_id == int.Parse(res[0]))
            {
                return model.parent_id;
            }
            else
            {
                return int.Parse(res[0]);
            }
        }

        /// <summary>
        /// 返回指定类别的上一级title
        /// </summary>
        /// <param name="category_id">类别ID</param>
        /// <returns>DataTable</returns>
        public string get_category_parent_title(int category_id)
        {
            int ParentId = new BLL.article_category().GetParentId(category_id);
            return get_category_title(ParentId, string.Empty);
        }
        private int i = 1;
        #endregion

        #region 私有方法===========================================
        /// <summary>
        /// 递归找到父节点
        /// </summary>
        private void LoopChannelMenu(StringBuilder strTxt, string urlKey, string mark, int category_id)
        {
            BLL.article_category bll = new BLL.article_category();
            Model.article_category model = bll.Get(category_id);
            int parentId = model.parent_id;
            if (parentId > 0)
            {
                i++;
                this.LoopChannelMenu(strTxt, urlKey, mark, parentId);
            }
            string url = null;
            if (model.call_index != "")
            {
                url = linkurl(urlKey, model.call_index);
            }
            else
            {
                url = linkurl(urlKey, category_id);
            }
            strTxt.Append("<a href=\"" + url + "\">" + bll.GetTitle(category_id) + "</a>" + mark);
        }
        #endregion
    }
}
