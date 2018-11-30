using DTcms.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DTcms.Web.Mvc.UI.Controllers
{
    public class Article_ListController : BaseController
    {
        public string channel = string.Empty; //频道名称
        public int page;         //当前页码
        public int category_id;  //类别ID
        public int totalcount;   //OUT数据总数
        public string pagelist;  //分页页码
        public string call_index;  //调用别名
        public string paramString = string.Empty;

        public DTcms.Model.article_category model = new DTcms.Model.article_category(); //分类的实体
        public Model.site_channel channelModel = new Model.site_channel();//频道实体对像

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            //接收参数
            page = DTRequest.GetQueryInt("page", 1);
            category_id = DTRequest.GetQueryInt("category_id");
            call_index = Utils.SafeXXS(DTRequest.GetQueryString("call_index"));

            DTcms.BLL.article_category bll = new DTcms.BLL.article_category();
            model.title = "所有类别";
            if (category_id > 0) //如果ID获取到，将使用ID
            {
                if (bll.Exists(category_id))
                    model = bll.GetModel(category_id);
            }
            ViewBag.model = model;
        }

        /// <summary>
        /// 验证频道列表数据
        /// </summary>
        /// <param name="channel_name"></param>
        public void validate_channel_data(string channel_name)
        {
            BLL.article_category bll = new BLL.article_category();
            channelModel = new BLL.site_channel().GetModel(channel_name);
            if (model == null)
            {
                Response.Redirect(linkurl("error"));
                return;
            }
            if (category_id > 0)
            {
                model = bll.GetModel(channelModel.id, category_id);
                if (model == null)
                {
                    Response.Redirect(linkurl("error"));
                    return;
                }
            }
            else if (!string.IsNullOrEmpty(call_index)) //否则检查设置的别名
            {
                model = bll.GetModel(channelModel.id, call_index);
                if (model == null)
                {
                    Response.Redirect(linkurl("error"));
                    return;
                }
                category_id = model.id;
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
            //分页参数
            if (!string.IsNullOrEmpty(model.call_index))
            {
                paramString = model.call_index;
            }
            else if (category_id > 0)
            {
                paramString = category_id.ToString();
            }
        }
    }
}
