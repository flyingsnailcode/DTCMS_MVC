using System;
using System.Data;
using System.Collections.Generic;
using DTcms.Common;

namespace DTcms.BLL
{
    /// <summary>
    /// 文章内容
    /// </summary>
    public class article : Services<Model.article>
    {
        private DAL.article dal = new DAL.article(siteConfig.sysdatabaseprefix);
        public string sysdatabaseprefix;
        public override void SetCurrentReposiotry()
        {
            sysdatabaseprefix = siteConfig.sysdatabaseprefix;
            CurrentRepository = dal;
        }
        #region 扩展方法================================
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public override int Add(Model.article model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public override bool Update(Model.article model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 根据视图显示前几条数据
        /// </summary>
        public DataSet GetList(string channel_name, int Top, string strWhere, string filedOrder)
        {
            return dal.GetList(channel_name, Top, strWhere, filedOrder);
        }
        
        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int channel_id, int Top, string strWhere, string filedOrder)
        {
            string channelName = new BLL.site_channel().GetChannelName(channel_id);//查询频道名称
            if (string.IsNullOrEmpty(channelName))
            {
                return new DataSet();
            }
            return dal.GetList(channelName, Top, strWhere, filedOrder);
        }

        public DataSet GetList(string channelName, int category_id, int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.GetList(channelName, category_id, pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int channel_id, int article_id)
        {
            string channelName = new BLL.site_channel().GetChannelName(channel_id);//查询频道名称
            if (string.IsNullOrEmpty(channelName))
            {
                return false;
            }
            return dal.Exists(channelName, article_id);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int channel_id, int article_id)
        {
            string channelName = new BLL.site_channel().GetChannelName(channel_id);//查询频道名称
            if (string.IsNullOrEmpty(channelName))
            {
                return false;
            }
            string content = dal.GetContent(channelName, article_id);//获取信息内容
            bool result = dal.Delete(channelName, channel_id, article_id);//删除内容
            if (result && !string.IsNullOrEmpty(content))
            {
                FileHelper.DeleteContentPic(content, siteConfig.webpath + siteConfig.filepath); //删除内容图片
            }
            return result;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.article GetModel(int channel_id, string call_index)
        {
            string channelName = new BLL.site_channel().GetChannelName(channel_id);//查询频道名称
            if (string.IsNullOrEmpty(channelName))
            {
                return null;
            }
            return dal.GetModel(channelName, call_index);
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int channel_id, string call_index)
        {
            if (string.IsNullOrEmpty(call_index))
            {
                return false;
            }
            string channelName = new BLL.site_channel().GetChannelName(channel_id);//查询频道名称
            if (string.IsNullOrEmpty(channelName))
            {
                return false;
            }
            return dal.Exists(channelName, call_index);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.article GetModel(string channelName, int article_id)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                return null;
            }
            return dal.GetModel(channelName, article_id);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.article GetModel(int channel_id, int article_id)
        {
            string channelName = new BLL.site_channel().GetChannelName(channel_id);//查询频道名称
            if (string.IsNullOrEmpty(channelName))
            {
                return null;
            }
            return dal.GetModel(channelName, article_id);
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList(int channel_id, int category_id, int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            string channelName = new BLL.site_channel().GetChannelName(channel_id);//查询频道名称
            if (string.IsNullOrEmpty(channelName))
            {
                recordCount = 0;
                return new DataSet();
            }
            return dal.GetList(channelName, category_id, pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public List<Model.article> GetListPage(int channel_id, int category_id, int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            string channelName = new BLL.site_channel().GetChannelName(channel_id);//查询频道名称
            if (string.IsNullOrEmpty(channelName))
            {
                recordCount = 0;
                return null;
            }
            return dal.GetListPage(channelName, category_id, pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }

        /// <summary>
        /// 根据频道名称获取总记录数
        /// </summary>
        public int ArticleCount(string channel_name, int category_id, string strWhere)
        {
            Dictionary<int, string> dic = new BLL.site_channel().GetListAll();
            if (!dic.ContainsValue(channel_name))
            {
                return 0;
            }
            return dal.ArticleCount(channel_name, category_id, strWhere);
        }

        /// <summary>
        /// 根据频道名称显示前几条数据
        /// </summary>
        public DataSet ArticleList(string channel_name, int category_id, int Top, string strWhere, string filedOrder)
        {
            Dictionary<int, string> dic = new BLL.site_channel().GetListAll();
            if (!dic.ContainsValue(channel_name))
            {
                return new DataSet();
            }
            return dal.ArticleList(channel_name, category_id, Top, strWhere, filedOrder);
        }
        #endregion

        #region 扩展方法================================
        /// <summary>
        /// 是否存在标题
        /// </summary>
        public bool ExistsTitle(int channel_id, string title)
        {
            string channelName = new BLL.site_channel().GetChannelName(channel_id);//查询频道名称
            if (string.IsNullOrEmpty(channelName))
            {
                return false;
            }
            return int.Parse(dal.GetExists(sysdatabaseprefix, channelName, "count(1)", string.Format("title='{0}'", title))) > 0;
        }

        /// <summary>
        /// 是否存在标题
        /// </summary>
        public bool ExistsTitle(int channel_id, int category_id, string title)
        {
            string channelName = new BLL.site_channel().GetChannelName(channel_id);//查询频道名称
            if (string.IsNullOrEmpty(channelName))
            {
                return false;
            }
            return int.Parse(dal.GetExists(sysdatabaseprefix, channelName, "count(1)", string.Format("category_id={0} and title='{1}'", category_id, title))) > 0;
        }

        /// <summary>
        /// 返回信息标题
        /// </summary>
        public string GetTitle(int channel_id, int article_id)
        {
            string channelName = new BLL.site_channel().GetChannelName(channel_id);//查询频道名称
            if (string.IsNullOrEmpty(channelName))
            {
                return string.Empty;
            }
            return dal.GetExists(sysdatabaseprefix, channelName, "top 1 title", "id=" + article_id);
        }

        /// <summary>
        /// 返回关于某列的信息统计
        /// </summary>
        public int GetColumn(string channelName, string column, string value)
        {
            string strWhere = "status=0";
            if (column.Trim() != "" && value.Trim() != "")
            {
                strWhere+=string.Format(" and {0}='{1}'", column, value);
            }
            return int.Parse(dal.GetExists(sysdatabaseprefix, channelName, "count(*) as H", strWhere));
        }

        /// <summary>
        /// 返回信息标题
        /// </summary>
        public string GetTitle(string channel_name, int article_id)
        {
            return dal.GetExists(sysdatabaseprefix, channel_name, "title", "id="+ article_id);
        }

        /// <summary>
        /// 返回信息内容
        /// </summary>
        public string GetContent(string channel_name, int article_id)
        {
            if (string.IsNullOrEmpty(channel_name))
            {
                return string.Empty;
            }
            return dal.GetExists(sysdatabaseprefix, channel_name, "content", "id=" + article_id);
        }

        /// <summary>
        /// 返回信息内容
        /// </summary>
        public string GetContent(string channel_name, string call_index)
        {
            return dal.GetExists(sysdatabaseprefix, channel_name, "content", string.Format("call_index='{0}'", call_index));
        }

        /// <summary>
        /// 返回信息封面图
        /// </summary>
        public string GetImgUrl(int channel_id, int article_id)
        {
            string channelName = new BLL.site_channel().GetChannelName(channel_id);//查询频道名称
            if (string.IsNullOrEmpty(channelName))
            {
                return string.Empty;
            }
            return dal.GetExists(sysdatabaseprefix, channelName, "img_url", "id=" + article_id);
        }

        /// <summary>
        /// 获取阅读次数
        /// </summary>
        public int GetClick(int channel_id, int article_id)
        {
            string channelName = new BLL.site_channel().GetChannelName(channel_id);//查询频道名称
            if (string.IsNullOrEmpty(channelName))
            {
                return 0;
            }
            return int.Parse(dal.GetExists(sysdatabaseprefix, channelName, "click", "id=" + article_id));
        }

        /// <summary>
        /// 获取文章赞次数
        /// </summary>
        public int GetZan(int channel_id, int article_id)
        {
            string channelName = new BLL.site_channel().GetChannelName(channel_id);//查询频道名称
            if (string.IsNullOrEmpty(channelName))
            {
                return 0;
            }
            return int.Parse(dal.GetExists(sysdatabaseprefix, channelName, "zan", "id=" + article_id));
        }

        /// <summary>
        /// 返回数据总数
        /// </summary>
        public int GetCount(int channel_id, string strWhere)
        {
            string channelName = new BLL.site_channel().GetChannelName(channel_id);//查询频道名称
            if (string.IsNullOrEmpty(channelName))
            {
                return 0;
            }
            return int.Parse(dal.GetExists(sysdatabaseprefix, channelName, "count(*) as H", strWhere));
        }

        /// <summary>
        /// 返回数据总数
        /// </summary>
        public new int GetCount(string channelName, string strWhere)
        {
            return int.Parse(dal.GetExists(sysdatabaseprefix, channelName, "count(*) as H", strWhere));
        }

        /// <summary>
        /// 返回商品库存数量
        /// </summary>
        public int GetStockQuantity(int channel_id, int article_id, int goods_id)
        {
            string channelName = new BLL.site_channel().GetChannelName(channel_id);//查询频道名称
            if (string.IsNullOrEmpty(channelName))
            {
                return 0;
            }
            return dal.GetStockQuantity(channelName, channel_id, article_id, goods_id);
        }

        /// <summary>
        /// 修改一列数据
        /// </summary>
        public bool UpdateField(int channel_id, int article_id, string strValue)
        {
            string channelName = new BLL.site_channel().GetChannelName(channel_id);//查询频道名称
            if (string.IsNullOrEmpty(channelName))
            {
                return false;
            }
            return dal.UpdateField(channelName, article_id, strValue);
        }

        /// <summary>
        /// 修改一列数据
        /// </summary>
        public bool UpdateField(string channelName, int article_id, string strValue)
        {
            return dal.UpdateField(channelName, article_id, strValue);
        }

        /// <summary>
        /// 获取微信推送实体
        /// </summary>
        public Model.article GetWXModel(int channel_id, int article_id)
        {
            string channelName = new BLL.site_channel().GetChannelName(channel_id);//查询频道名称
            if (string.IsNullOrEmpty(channelName))
            {
                return null;
            }
            DataTable dt = dal.GetList(channelName, 1, "id=" + article_id, "id desc").Tables[0];
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            Model.article model = new Model.article();
            model.id = int.Parse(dt.Rows[0]["id"].ToString());
            model.title = dt.Rows[0]["title"].ToString();
            model.img_url = dt.Rows[0]["img_url"].ToString();
            model.zhaiyao = dt.Rows[0]["zhaiyao"].ToString();
            model.content = dt.Rows[0]["content"].ToString();
            return model;
        }
        #endregion

        #region 前台模板调用方法========================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool ArticleExists(string channel_name, int article_id)
        {
            Dictionary<int, string> dic = new BLL.site_channel().GetListAll();
            if (!dic.ContainsValue(channel_name))
            {
                return false;
            }
            return int.Parse(dal.GetExists(sysdatabaseprefix, channel_name, "count(1)", "id=" + article_id)) > 0;
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool ArticleExists(string channel_name, string call_index)
        {
            Dictionary<int, string> dic = new BLL.site_channel().GetListAll();
            if (!dic.ContainsValue(channel_name))
            {
                return false;
            }
            return int.Parse(dal.GetExists(sysdatabaseprefix, channel_name, "count(1)", string.Format("call_index='{0}'", call_index)))>0;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.article ArticleModel(string channel_name, int article_id)
        {
            Dictionary<int, string> dic = new BLL.site_channel().GetListAll();
            if (!dic.ContainsValue(channel_name))
            {
                return null;
            }
            return dal.GetModel(channel_name, article_id);
        }
        
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.article ArticleModel(string channel_name, string call_index)
        {
            Dictionary<int, string> dic = new BLL.site_channel().GetListAll();
            if (!dic.ContainsValue(channel_name))
            {
                return null;
            }
            return dal.GetModel(channel_name, call_index);
        }

        /// <summary>
        /// 根据频道名称显示前几条数据
        /// </summary>
        public DataSet ArticleList(string channel_name, int Top, string strWhere, string filedOrder)
        {
            Dictionary<int, string> dic = new BLL.site_channel().GetListAll();
            if (!dic.ContainsValue(channel_name))
            {
                return new DataSet();
            }
            return dal.ArticleList(channel_name, Top, strWhere, filedOrder);
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageSize">分页数量</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="strWhere">条件</param>
        /// <param name="filedOrder">排序</param>
        /// <param name="recordCount">返回数据总数</param>
        /// <returns>DataTable</returns>
        public DataSet ArticleList(string channel_name, int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.ArticleList(channel_name, pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }

        /// <summary>
        /// 根据频道名称获得查询分页数据
        /// </summary>
        public DataSet ArticleList(string channel_name, int category_id, int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            Dictionary<int, string> dic = new BLL.site_channel().GetListAll();
            if (!dic.ContainsValue(channel_name))
            {
                recordCount = 0;
                return new DataSet();
            }
            return dal.ArticleList(channel_name, category_id, pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }

        /// <summary>
        /// 根据频道名称及规格查询分页数据
        /// </summary>
        public DataSet ArticleList(string channel_name, int category_id, Dictionary<string, string> dicSpecIds, int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.ArticleList(channel_name, category_id, dicSpecIds, pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }

        /// <summary>
        /// 获得关健字查询分页数据(搜索用到)
        /// </summary>
        public DataSet ArticleSearch(int site_id, int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.ArticleSearch(site_id, pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }

        /// <summary>
        /// 获得Tags查询分页数据(搜索用到)
        /// </summary>
        public DataSet ArticleSearch(string channel_name, string tags, int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.ArticleSearch(channel_name, tags, pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }

        /// <summary>
        /// 获得Tags查询分页数据(搜索用到)
        /// </summary>
        public DataSet ArticleSearch(int site_id, int tags, int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.ArticleSearch(site_id, tags, pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }
        #endregion
    }
}