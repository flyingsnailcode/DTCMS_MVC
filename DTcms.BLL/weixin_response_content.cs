using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
	/// <summary>
	/// ΢��ƽ̨�ظ���Ϣ
	/// </summary>
	public partial class weixin_response_content : Services<Model.weixin_response_content>
    {
        private DAL.weixin_response_content dal = new DAL.weixin_response_content();
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region ��չ����================================
        /// <summary>
        /// ����һ����¼
        /// </summary>
        /// <param name="account_id">����ƽ̨�˻�ID</param>
        /// <param name="openid">������û�openid</param>
        /// <param name="request_type">�û���������ͣ��ı���Ϣ��text ͼƬ��Ϣ:image ����λ����Ϣ:location ������Ϣ:link �¼�:event</param>
        /// <param name="request_content">�û��������������</param>
        /// <param name="response_type"> ϵͳ�ظ������ͣ��ı���Ϣ��text ,ͼ����Ϣ:txtpic ,����music, ����λ����Ϣ:location ������Ϣ:link,δȡ������none</param>
        /// <param name="reponse_content">ϵͳ�ظ�������</param>
        /// <param name="ToUserName">����ȡ����xml���ݣ����ǽ�toUserName����</param>
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

