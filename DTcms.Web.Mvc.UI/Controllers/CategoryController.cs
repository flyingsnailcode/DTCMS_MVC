using DTcms.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DTcms.Web.Mvc.UI.Controllers
{
    public class CategoryController : BaseController
    {
        public int category_id; //类别ID
        public string call_index;  //调用别名
        public string action;

        public Model.article_category model = new Model.article_category(); //分类的实体
        public Model.site_channel channelModel = new Model.site_channel(); //分类的实体

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            category_id = DTRequest.GetQueryInt("category_id");
            call_index = Utils.SafeXXS(DTRequest.GetQueryString("call_index"));
            action = DTRequest.GetQueryString("action");
            
            BLL.article_category bll = new BLL.article_category();
            model.title = "所有类别";
            if (category_id > 0) //如果ID获取到，将使用ID
            {
                if (!bll.Exists(category_id))
                {
                    Response.Redirect(linkurl("error"));
                    return;
                }
                model = bll.GetModel(category_id);
            }
            else if (!string.IsNullOrEmpty(call_index)) //否则检查设置的别名
            {
                if (!bll.Exists(call_index))
                {
                    Response.Redirect(linkurl("error"));
                    return;
                }
                model = bll.GetModel(call_index);
                //赋值类别ID
                category_id = model.id;
            }
            //判断SEO标题
            if (string.IsNullOrEmpty(model.seo_title) || "" == model.seo_title)
            {
                model.seo_title = model.title;
            }
            //获取频道内容
            channelModel = new BLL.site_channel().GetModel(model.channel_id);

            ViewBag.ChannelModel = channelModel;
            ViewBag.model = model;
        }

        /// <summary>
        /// 验证频道列表数据
        /// </summary>
        /// <param name="channel_name"></param>
        protected void validate_channel_data(string channel_name)
        {
            if (category_id > 0 && null != channelModel && !channelModel.name.Equals(channel_name))
            {
                Response.Redirect(linkurl("error"));
                return;
            }
        }
    }
}