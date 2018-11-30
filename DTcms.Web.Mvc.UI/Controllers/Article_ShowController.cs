using DTcms.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DTcms.Web.Mvc.UI.Controllers
{
    public class Article_ShowController : BaseController
    {
        public string channel = string.Empty; //频道名称
        public string call_index; //调用名
        public int id;
        public DTcms.Model.article model = new DTcms.Model.article();
        public Model.site_channel channelModel = new Model.site_channel(); //频道的实体
        public Model.article_category categoryModel = new Model.article_category(); //分类的实体

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            id = DTRequest.GetQueryInt("id");
            call_index = Utils.SafeXXS(DTRequest.GetQueryString("call_index"));
        }
        
        /// <summary>
        /// 获取上一条下一条的链接
        /// </summary>
        /// <param name="urlkey">urlkey</param>
        /// <param name="type">-1代表上一条，1代表下一条</param>
        /// <param name="defaultvalue">默认文本</param>
        /// <param name="callIndex">是否使用别名，0使用ID，1使用别名</param>
        /// <returns>A链接</returns>
        public string get_prevandnext_article(string urlkey, int type, string defaultvalue, int callIndex)
        {
            string symbol = (type == -1 ? "<" : ">");
            DTcms.BLL.article bll = new DTcms.BLL.article();
            string str = string.Empty;
            str = " and category_id=" + model.category_id;

            string orderby = type == -1 ? "id desc" : "id asc";
            DataSet ds = bll.ArticleList(channel, 1, "channel_id=" + model.channel_id + " " + str + " and status=0 and Id" + symbol + id, orderby);
            if (ds == null || ds.Tables[0].Rows.Count <= 0)
            {
                return defaultvalue;
            }
            if (callIndex == 1 && !string.IsNullOrEmpty(ds.Tables[0].Rows[0]["call_index"].ToString()))
            {
                return "<a href=\"" + linkurl(urlkey, ds.Tables[0].Rows[0]["call_index"].ToString()) + "\">" + ds.Tables[0].Rows[0]["title"] + "</a>";
            }
            return "<a href=\"" + linkurl(urlkey, ds.Tables[0].Rows[0]["id"].ToString()) + "\">" + ds.Tables[0].Rows[0]["title"] + "</a>";
        }

        /// <summary>
        /// 验证频道列表数据
        /// </summary>
        /// <param name="channel_name"></param>
        public void validate_channel_data(string channel_name)
        {
            this.channel = channel_name;
            channelModel = new BLL.site_channel().GetModel(channel_name);
            if (null != channelModel && !channelModel.name.Equals(channel_name))
            {
                Response.Redirect(linkurl("error"));
                return;
            }

            BLL.article bll = new BLL.article();
            if (id > 0) //如果ID获取到，将使用ID
            {
                if (!bll.ArticleExists(channel, id))
                {
                    Response.Redirect(linkurl("error"));
                    return;
                }
                model = bll.ArticleModel(channel, id);
            }
            else if (!string.IsNullOrEmpty(call_index)) //否则检查设置的别名
            {
                if (!bll.ArticleExists(channel, call_index))
                {
                    Response.Redirect(linkurl("error"));
                    return;
                }
                model = bll.ArticleModel(channel, call_index);
                //赋值文章ID
                id = model.id;
            }
            else
            {
                Response.Redirect(linkurl("error"));
                return;
            }
            if (model.status == 1)
            {
                Response.Redirect(linkurl("error", "?msg=" + Utils.UrlEncode("浏览得该信息审核还未通过！")));
                return;
            }
            //判断是否是链接
            if (!string.IsNullOrEmpty(model.link_url))
            {
                Response.Redirect(model.link_url);
                return;
            }
            //判断SEO标题
            if (string.IsNullOrEmpty(model.seo_title))
            {
                model.seo_title = site.seo_title;
            }
            if (string.IsNullOrEmpty(model.seo_keywords))
            {
                model.seo_keywords = site.seo_keywords;
            }
            if (string.IsNullOrEmpty(model.seo_description))
            {
                model.seo_description = site.seo_description;
            }
            //获取频道内容
            channelModel = new BLL.site_channel().GetModel(model.channel_id);

            //获取类别内容
            categoryModel = new BLL.article_category().GetModel(model.category_id);

            ViewBag.model = model;
        }
    }
}
