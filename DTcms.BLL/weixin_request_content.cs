using System;
using System.Data;
using System.Collections.Generic;

namespace DTcms.BLL
{
	/// <summary>
	/// 微信请求回复的内容
	/// </summary>
	public partial class weixin_request_content : Services<Model.weixin_request_content>
    {
        private DAL.weixin_request_content dal = new DAL.weixin_request_content(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }
        #region 基本方法================================
        #endregion

        #region 扩展方法================================
        /// <summary>
        /// 返回规则的第一条标题
        /// </summary>
        public string GetTitle(int rule_id)
        {
            return dal.GetTitle(rule_id);
        }

        /// <summary>
        /// 返回规则的第一条内容
        /// </summary>
        public string GetContent(int rule_id)
        {
            return dal.GetContent(rule_id);
        }

        /// <summary>
        /// 返回规则下面的内容数量
        /// </summary>
        public int GetCount(int rule_id)
        {
            return dal.GetCount(rule_id);
        }
        #endregion
    }
}

