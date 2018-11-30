using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
	/// <summary>
	/// 微信平台回复信息
	/// </summary>
	public partial class weixin_response_content : Services<Model.weixin_response_content>
    {
        private DAL.weixin_response_content dal = new DAL.weixin_response_content();
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        /// <summary>
        /// 增加一条记录
        /// </summary>
        /// <param name="account_id">公众平台账户ID</param>
        /// <param name="openid">请求的用户openid</param>
        /// <param name="request_type">用户请求的类型：文本消息：text 图片消息:image 地理位置消息:location 链接消息:link 事件:event</param>
        /// <param name="request_content">用户请求的数据内容</param>
        /// <param name="response_type"> 系统回复的类型：文本消息：text ,图文消息:txtpic ,语音music, 地理位置消息:location 链接消息:link,未取到数据none</param>
        /// <param name="reponse_content">系统回复的内容</param>
        /// <param name="ToUserName">由于取不到xml内容，我们将toUserName存入</param>
        public int Add(int account_id, string openid, string request_type, string request_content, string response_type, string reponse_content, string xml_content)
        {
            int result = 0;
            try
            {
                Model.weixin_response_content model = new Model.weixin_response_content();
                model.account_id = account_id;
                model.openid = openid;
                model.request_type = request_type;
                model.request_content = request_content;
                model.response_type = response_type;
                model.reponse_content = reponse_content;
                model.xml_content = xml_content;
                model.add_time = DateTime.Now;
                result = dal.Add(model);
            }
            catch
            {
                return 0;
            }
            return result;
        }
        #endregion
    }
}

