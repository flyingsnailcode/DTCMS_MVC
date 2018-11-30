using DTcms.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DTcms.Web.Mvc.UI.Controllers
{
    public class ArticleController : BaseController
    {
        public int page;         //当前页码
        public int totalcount;   //OUT数据总数
        public string pagelist;  //分页页码
        public string channel = string.Empty;//频道名称
        public Model.site_channel model = new Model.site_channel();//频道实体对像
        public int channel_id; //当前频道ID

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            channel = DTRequest.GetFormString("channel");
            page = DTRequest.GetQueryInt("page", 1);
        }

        /// <summary>
        /// 验证频道列表数据
        /// </summary>
        /// <param name="channel_name"></param>
        public void validate_channel_data(string channel_name)
        {
            BLL.site_channel bll = new BLL.site_channel();
            model = new BLL.site_channel().GetModel(channel_name);
            if (model == null)
            {
                Response.Redirect(linkurl("error"));
                return;
            }
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
            channel_id = model.id;
            ViewBag.model = model;
        }
    }
}