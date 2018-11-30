using System;
using System.Collections.Generic;
using System.Data;

namespace DTcms.BLL
{
    /// <summary>
    /// 业务逻辑层
    /// </summary>
    public class video_comment : Services<Model.video_comment>
    {
        private DAL.video_comment dal = new DAL.video_comment(siteConfig.sysdatabaseprefix);
        public override void SetCurrentReposiotry()
        {
            CurrentRepository = dal;
        }

        #region 基本方法
        
        #endregion

        #region 增加一条数据
        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="model">Model.video_comment</param>
        /// <returns>ID</returns>
        public override int Add(Model.video_comment model)
        {
              return dal.Add(model);
        }
        #endregion
        
        #region 更新一条数据
        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <param name="model">Model.video_comment</param>
        /// <returns>True Or False</returns>
        public override bool Update(Model.video_comment model)
        {
              return dal.Update(model);
        }
        #endregion
        
        #region 扩展方法
        /// <summary>
        /// 取得所有类别列表
        /// </summary>
        /// <param name="Top">数量</param>
        /// <param name="parent_id">父类ID</param>
        /// <param name="strWhere">条件</param>
        /// <param name="filedOrder">排序</param>
        /// <returns>DataTable</returns>
        public DataTable GetList(int Top, int parent_id, string strWhere, string filedOrder)
        {
              return dal.GetList(Top, parent_id, strWhere, filedOrder);
        }
        #endregion
    }
}
