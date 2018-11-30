using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DTcms.Common
{
    public class CustomJsonResult: JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            HttpResponseBase response = context.HttpContext.Response;

            response.ContentType = "application/json";

            if (Data != null)
            {
                var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };//这里使用自定义日期格式，默认是ISO8601格式
                response.Write(JsonConvert.SerializeObject(Data, Formatting.Indented, timeConverter));
            }
        }
    }
}
