using System;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;

namespace DTcms.BLL
{
    /// <summary>
    /// �û���Ϣ
    /// </summary>
    public partial class users : Services<Model.users>
    {
        private DAL.users dal = new DAL.users(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region ��չ����================================
        /// <summary>
        /// �Ƿ���ڸü�¼
        /// </summary>
        public bool Exists(int id)
        {
            return dal.Exists(id);
        }
        /// <summary>
        /// ����û����Ƿ����
        /// </summary>
        public bool Exists(string user_name)
        {
            return dal.Exists(user_name);
        }

        /// <summary>
        /// ɾ��һ������
        /// </summary>
        public bool Delete(int id)
        {
            return dal.Delete(id);
        }

        /// <summary>
        /// ���ͬһIPע����(Сʱ)���Ƿ����
        /// </summary>
        public bool Exists(string reg_ip, int regctrl)
        {
            return dal.Exists(reg_ip, regctrl);
        }

        /// <summary>
        /// �����û�������һ��ʵ��
        /// </summary>
        public Model.users GetModel(string sqlwhere)
        {
            return dal.GetModel(sqlwhere);
        }

        /// <summary>
        /// �����û������뷵��һ��ʵ��
        /// </summary>
        /// <param name="user_name">�û���(����)</param>
        /// <param name="password">����</param>
        /// <param name="emaillogin">�Ƿ�����������Ϊ��¼</param>
        /// <param name="mobilelogin">�Ƿ������ֻ���Ϊ��¼</param>
        /// <param name="is_encrypt">�Ƿ���Ҫ��������</param>
        /// <returns></returns>
        public Model.users GetModel(string user_name, string password, int emaillogin, int mobilelogin, bool is_encrypt)
        {
            //���һ���Ƿ���Ҫ����
            if (is_encrypt)
            {
                //��ȡ�ø��û��������Կ
                string salt = dal.GetSalt(user_name);
                if (string.IsNullOrEmpty(salt))
                {
                    return null;
                }
                //�����Ľ��м������¸�ֵ
                password = DESEncrypt.Encrypt(password, salt);
            }
            return dal.GetModel(user_name, password, emaillogin, mobilelogin);
        }
        /// <summary>
        /// �����û�������һ��ʵ��
        /// </summary>
        public Model.users GetNmaeModel(string user_name)
        {
            return dal.GetNmaeModel(user_name);
        }
        #endregion

        #region ��չ����===================================
        /// <summary>
        /// ���Email�Ƿ����
        /// </summary>
        public bool ExistsEmail(string email)
        {
            return dal.ExistsEmail(email);
        }

        /// <summary>
        /// ����ֻ������Ƿ����
        /// </summary>
        public bool ExistsMobile(string mobile)
        {
            return dal.ExistsMobile(mobile);
        }

        /// <summary>
        /// ����һ������û���
        /// </summary>
        public string GetRandomName(int length)
        {
            string temp = Utils.Number(length, true);
            if (Exists(temp))
            {
                return GetRandomName(length);
            }
            return temp;
        }

        /// <summary>
        /// �����û���ȡ��Salt
        /// </summary>
        public string GetSalt(string user_name)
        {
            return dal.GetSalt(user_name);
        }

        /// <summary>
        /// �޸�һ������
        /// </summary>
        public int UpdateField(int id, string strValue)
        {
            return dal.UpdateField(id, strValue);
        }

        /// <summary>
        /// �û�����
        /// </summary>
        public bool Upgrade(int id)
        {
            if (!Exists(id))
            {
                return false;
            }
            Model.users model = Get(id);
            Model.user_groups groupModel = new user_groups().GetUpgrade(model.group_id, model.exp);
            if (groupModel == null)
            {
                return false;
            }
            int result = UpdateField(id, "group_id=" + groupModel.id);
            if (result > 0)
            {

                //���ӻ���
                if (groupModel.point > 0)
                {
                    new BLL.user_point_log().Add(model.id, model.user_name, groupModel.point, 1, "������û���", true);
                }
                //���ӽ��
                if (groupModel.amount > 0)
                {
                    new BLL.user_amount_log().Add(model.id, model.user_name, groupModel.amount, "�������ͽ��");
                }
            }
            return true;
        }
        /// <summary>
        /// �û���ֵ����
        /// </summary>
        public bool Upgrade(int id, decimal price)
        {
            if (!Exists(id))
            {
                return false;
            }
            Model.users model = Get(id);
            Model.user_groups groupModel = new user_groups().GetUpgradePrice(model.group_id, price);
            if (null == groupModel)
            {
                return false;
            }
            int result = UpdateField(id, "group_id=" + groupModel.id);
            if (result > 0)
            {
                //���ӻ���
                if (groupModel.point > 0)
                {
                    new BLL.user_point_log().Add(model.id, model.user_name, groupModel.point, 1, "������û���", true);
                }
                //���ӽ��
                if (groupModel.amount > 0)
                {
                    new BLL.user_amount_log().Add(model.id, model.user_name, groupModel.amount, "�������ͽ��");
                }
            }
            return true;
        }
        #endregion
    }
}