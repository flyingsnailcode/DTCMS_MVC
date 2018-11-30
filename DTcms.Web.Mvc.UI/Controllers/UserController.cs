using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTcms.Web.Mvc.UI.Controllers
{
    public class UserController: BaseController
    {
        public Model.users userModel;
        public Model.user_groups groupModel;

        protected override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (!IsUserLogin())
            {
                //跳转URL
                filterContext.Result = Redirect(linkurl("login"));
                return;
            }
            //获得登录用户信息
            userModel = GetUserInfo();
            groupModel = new DTcms.BLL.user_groups().Get(userModel.group_id);
            if (groupModel == null)
            {
                groupModel = new DTcms.Model.user_groups();
            }
            if (userModel.birthday == null)
            {
                userModel.birthday = new DateTime(1900, 1, 1);
            }
            ViewBag.userModel = userModel;
            ViewBag.groupModel = groupModel;
        }
    }
}
