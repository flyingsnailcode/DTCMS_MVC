using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.IO.Compression;
using System.Text;
using DTcms.Common;

namespace DTcms.Web.Mvc.UI.Controllers
{
    public class ManageController : Controller
    {
        protected internal Model.siteconfig siteConfig;
        public const string EDIT_RESULT_VIEW = "~/Areas/Admin/Views/Shared/EditResult.cshtml";

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            siteConfig = new BLL.siteconfig().loadConfig();
            base.OnAuthorization(filterContext);
            if (!IsAdminLogin())
            {
                filterContext.Result = Redirect("/Home/Login");
                return;
            }
        }

        #region 管理员
        #region 判断管理员是否已经登录(解决Session超时问题)
        /// <summary>
        /// 判断管理员是否已经登录(解决Session超时问题)
        /// </summary>
        public bool IsAdminLogin()
        {
            //如果Session为Null
            if (System.Web.HttpContext.Current.Session[DTKeys.SESSION_ADMIN_INFO] != null)
            {
                return true;
            }
            else
            {
                //检查Cookies
                string adminname = Utils.GetCookie("AdminName", "DTcms");
                string adminpwd = Utils.GetCookie("AdminPwd", "DTcms");
                //cookies密码解密
                try
                {
                    //用户名加密
                    adminname = DESEncrypt.Decrypt(adminname, DTKeys.COOKIE_KEY);
                    //密码绑定IP
                    adminpwd = DESEncrypt.Decrypt(adminpwd, DTRequest.GetIP());
                }
                catch
                {
                    return false;
                }
                if (adminname != "" && adminpwd != "")
                {
                    BLL.manager bll = new BLL.manager();
                    Model.manager model = bll.GetModel(adminname, adminpwd);
                    if (model != null)
                    {
                        System.Web.HttpContext.Current.Session[DTKeys.SESSION_ADMIN_INFO] = model;
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion
        
        #region 取得管理员信息
        /// <summary>
        /// 取得管理员信息
        /// </summary>
        public Model.manager GetAdminInfo()
        {
            if (IsAdminLogin())
            {
                Model.manager model = System.Web.HttpContext.Current.Session[DTKeys.SESSION_ADMIN_INFO] as Model.manager;
                if (model != null)
                {
                    return model;
                }
            }
            return null;
        }
        #endregion

        #region 检查管理员权限
        /// <summary>
        /// 检查管理员权限
        /// </summary>
        /// <returns></returns>
        public Model.manager_role GetAdminRole()
        {
            Model.manager model = GetAdminInfo();
            return new BLL.manager_role().GetModel(model.role_id);
        }
        #endregion
        
        #region 检查管理员权限
        /// <summary>
        /// 检查管理员权限
        /// </summary>
        /// <param name="nav_name">菜单名称</param>
        /// <param name="action_type">操作类型</param>
        public void ChkAdminLevel(string nav_name, string action_type)
        {
            Model.manager model = GetAdminInfo();
            BLL.manager_role bll = new BLL.manager_role();
            bool result = bll.Exists(model.role_id, nav_name, action_type);

            if (!result)
            {
                string msgbox = "parent.jsdialog(\"错误提示\", \"您没有管理该页面的权限，请勿非法进入！\", \"back\")";
                Response.Write("<script type=\"text/javascript\">" + msgbox + "</script>");
                Response.End();
            }
        }
        #endregion

        #region 检查管理员权限
        /// <summary>
        /// 检查管理员权限
        /// </summary>
        /// <param name="nav_name">菜单名称</param>
        /// <param name="action_type">操作类型</param>
        public void ChkAdminLevel(string nav_name, string action_type, out Model.manager_role rolemodel)
        {
            Model.manager model = GetAdminInfo();
            BLL.manager_role bll = new BLL.manager_role();
            bool result = bll.Exists(model.role_id, nav_name, action_type, out rolemodel);
            if (!result)
            {
                string msgbox = "parent.jsdialog(\"错误提示\", \"您没有管理该页面的权限，请勿非法进入！\", \"back\")";
                Response.Write("<script type=\"text/javascript\">" + msgbox + "</script>");
                Response.End();
            }
        }
        #endregion
        
        #region 抛弃（2017-07-15）
        public bool ChkAuthority(string nav_name, string action_type)
        {
            Model.manager model = GetAdminInfo();
            BLL.manager_role bll = new BLL.manager_role();
            bool result = bll.Exists(model.role_id, nav_name, action_type);

            if (result)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 写入管理日志
        /// <summary>
        /// 写入管理日志
        /// </summary>
        /// <param name="action_type"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool AddAdminLog(string action_type, string remark)
        {
            if (siteConfig.logstatus > 0)
            {
                Model.manager model = GetAdminInfo();
                int newId = new BLL.manager_log().Add(model.id, model.user_name, action_type, remark);
                if (newId > 0)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #endregion

        #region 检测是否授权
        public void CheckRoot()
        {
            if (new BLL.manager().RootTotal() != 1)
            {
                base.Response.Redirect("./installer/index", true);
            }
        }
        #endregion

        #region JS提示
        /// <summary>
        /// 添加编辑删除提示
        /// </summary>
        /// <param name="msgtitle">提示文字</param>
        /// <param name="url">返回地址</param>
        public ActionResult ResultMsg(string msgtitle, string url)
        {
            ActionResult result = View(EDIT_RESULT_VIEW);
            string msbox = "parent.jsprint(\"" + msgtitle + "\", \"" + url + "\")";
            ViewBag.ClientScript = msbox;
            return result;
        }
        /// <summary>
        /// 带回传函数的添加编辑删除提示
        /// </summary>
        /// <param name="msgtitle">提示文字</param>
        /// <param name="url">返回地址</param>
        /// <param name="callback">JS回调函数</param>
        public ActionResult ResultMsg(string msgtitle, string url, string callback)
        {
            ActionResult result = View(EDIT_RESULT_VIEW);
            string msbox = "parent.jsprint(\"" + msgtitle + "\", \"" + url + "\", " + callback + ")";
            ViewBag.ClientScript = msbox;
            return result;
        }
        /// <summary>
        /// 添加编辑删除提示
        /// </summary>
        /// <param name="msgtitle">提示文字</param>
        /// <param name="url">返回地址</param>
        public string JscriptMsg(string msgtitle, string url)
        {
            string msbox = "parent.jsprint(\"" + msgtitle + "\", \"" + url + "\")";
            return msbox;
        }
        /// <summary>
        /// 带回传函数的添加编辑删除提示
        /// </summary>
        /// <param name="msgtitle">提示文字</param>
        /// <param name="url">返回地址</param>
        /// <param name="callback">JS回调函数</param>
        public string JscriptMsg(string msgtitle, string url, string callback)
        {
            string msbox = "parent.jsprint(\"" + msgtitle + "\", \"" + url + "\", " + callback + ")";
            return msbox;
        }
        #endregion

        #region 查找匹配的URL
        /// <summary>
        /// 查找匹配的URL
        /// </summary>
        /// <param name="channel_id">频道ID</param>
        /// <param name="call_index">调用名</param>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public string get_url_rewrite(string channel_name, string type, string call_index, int id)
        {
            string querystring = id.ToString();
            if (string.IsNullOrEmpty(channel_name))
            {
                return string.Empty;
            }
            if (!string.IsNullOrEmpty(call_index))
            {
                querystring = call_index;
            }
            BLL.url_rewrite bll = new BLL.url_rewrite();
            Model.url_rewrite model = bll.GetInfo(channel_name, type);
            if (model != null)
            {
                return new BaseController().linkurl(model.name, querystring);
                
            }
            return string.Empty;
        }
        #endregion

        #region 压缩文本(未使用)
        /// <summary>
        /// 压缩文本(未使用)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult CompressText(string text)
        {
            // convert text to bytes  
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            // get a stream  
            MemoryStream ms = new MemoryStream();
            // get ready to zip up our stream  
            using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                // compress the data into our buffer  
                zip.Write(buffer, 0, buffer.Length);
            }
            // reset our position in compressed stream to the start  
            ms.Position = 0;
            // get the compressed data  
            byte[] compressed = ms.ToArray();
            ms.Read(compressed, 0, compressed.Length);
            // prepare final data with header that indicates length  
            byte[] gzBuffer = new byte[compressed.Length + 4];
            //copy compressed data 4 bytes from start of final header  
            System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            // copy header to first 4 bytes  
            System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            // convert back to string and return  
            string result = Convert.ToBase64String(gzBuffer);
            return Content(result);
        }
        #endregion
        
        #region 解压缩文本(未使用)
        /// <summary>
        /// 解压缩文本(未使用)
        /// </summary>
        /// <param name="compressedText"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult UncompressText(string compressedText)
        {
            // get string as bytes  
            byte[] gzBuffer = Convert.FromBase64String(compressedText);
            // prepare stream to do uncompression  
            MemoryStream ms = new MemoryStream();
            // get the length of compressed data  
            int msgLength = BitConverter.ToInt32(gzBuffer, 0);
            // uncompress everything besides the header  
            ms.Write(gzBuffer, 4, gzBuffer.Length - 4);
            // prepare final buffer for just uncompressed data  
            byte[] buffer = new byte[msgLength];
            // reset our position in stream since we're starting over  
            ms.Position = 0;
            // unzip the data through stream  
            GZipStream zip = new GZipStream(ms, CompressionMode.Decompress);
            // do the unzip  
            zip.Read(buffer, 0, buffer.Length);
            // convert back to string and return  
            string result = Encoding.UTF8.GetString(buffer);
            return Content(result);
        }
        #endregion
        
        #region 重写JsonResult
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding)
        {
            return new CustomJsonResult { Data = data, ContentType = contentType, ContentEncoding = contentEncoding };
        }

        public new JsonResult Json(object data, JsonRequestBehavior jsonRequest)
        {
            return new CustomJsonResult { Data = data, JsonRequestBehavior = jsonRequest };
        }

        public new JsonResult Json(object data)
        {
            return new CustomJsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion
    }
}
