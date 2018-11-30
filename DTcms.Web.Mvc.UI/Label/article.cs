using DTcms.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DTcms.Web.Mvc.UI.Controllers
{
    public partial class BaseController : Controller
    {
        #region 列表标签======================================
        /// <summary>
        /// 随机文章列表
        /// </summary>
        /// <param name="channel_name">频道名称</param>
        /// <param name="top">显示条数</param>
        /// <param name="strwhere">查询条件</param>
        /// <returns>DataTable</returns>
        //public DataTable get_article_random_list(string channel_name, int category_id, int top, string strwhere)
        //{
        //    DataTable dt = new DataTable();
        //    if (!string.IsNullOrEmpty(channel_name))
        //    {
        //        dt = new BLL.article().GetRandomList(channel_name, category_id, top, strwhere).Tables[0];
        //    }
        //    return dt;
        //}

        /// <summary>
        /// 文章列表
        /// </summary>
        /// <param name="channel_name">频道名称</param>
        /// <param name="top">显示条数</param>
        /// <param name="strwhere">查询条件</param>
        /// <returns>DataTable</returns>
        public DataTable get_article_list(string channel_name, int top, string strwhere)
        {
            DataTable dt = new DataTable();
            if (!string.IsNullOrEmpty(channel_name))
            {
                dt = new BLL.article().ArticleList(channel_name, top, strwhere, "sort_id asc,add_time desc").Tables[0];
            }
            return dt;
        }

        /// <summary>
        /// 获取同类文章
        /// </summary>
        /// <param name="channel_name">频道名</param>
        /// <param name="tonglei">同类值</param>
        /// <param name="top">显示条数</param>
        /// <returns></returns>
        public List<Model.article> get_tonglei_article_list(string channel_name, string tonglei, int top)
        {
            List<Model.article> ls = new List<Model.article>();
            string[] res = tonglei.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < res.Length; i++)
            {
                if (i <= top)
                {
                    Model.article model = new BLL.article().GetModel(channel_name, int.Parse(res[i]));
                    if (model != null)
                    {
                        ls.Add(model);
                    }
                }
            }
            //foreach (string item in res)
            //{
            //    Model.article model = new BLL.article().GetModel(channel_name, int.Parse(item));
            //    if (model != null)
            //    {
            //        ls.Add(model);
            //    }
            //}
            return ls;
        }

        /// <summary>
        /// 文章列表
        /// </summary>
        /// <param name="channel_name">频道名称</param>
        /// <param name="category_id">分类ID</param>
        /// <param name="top">显示条数</param>
        /// <param name="strwhere">查询条件</param>
        /// <returns>DataTable</returns>
        public DataTable get_article_list(string channel_name, int category_id, int top, string strwhere)
        {
            DataTable dt = new DataTable();
            if (!string.IsNullOrEmpty(channel_name))
            {
                dt = new BLL.article().ArticleList(channel_name, category_id, top, strwhere, "sort_id asc,add_time desc").Tables[0];
            }
            return dt;
        }
        /// <summary>
        /// 文章列表
        /// </summary>
        /// <param name="channel_name">频道名称</param>
        /// <param name="category_id">分类ID</param>
        /// <param name="top">显示条数</param>
        /// <param name="strwhere">查询条件</param>
        /// <param name="orderby">排序</param>
        /// <returns>DataTable</returns>
        public DataTable get_article_list(string channel_name, int category_id, int top, string strwhere, string orderby)
        {
            DataTable dt = new DataTable();
            if (!string.IsNullOrEmpty(channel_name))
            {
                dt = new BLL.article().ArticleList(channel_name, category_id, top, strwhere, orderby).Tables[0];
            }
            return dt;
        }
        /// <summary>
        /// 文章分页列表(自定义页面大小)
        /// </summary>
        /// <param name="channel_name">频道名称</param>
        /// <param name="category_id">分类ID</param>
        /// <param name="page_size">页面大小</param>
        /// <param name="page_index">当前页码</param>
        /// <param name="strwhere">查询条件</param>
        /// <param name="orderby">排序</param>
        /// <param name="totalcount">总记录数</param>
        /// <returns>DateTable</returns>
        public DataTable get_article_list(string channel_name, int category_id, int page_size, int page_index, string strwhere, string orderby, out int totalcount)
        {
            DataTable dt = new DataTable();
            if (!string.IsNullOrEmpty(channel_name))
            {
                dt = new BLL.article().ArticleList(channel_name, category_id, page_size, page_index, strwhere, orderby, out totalcount).Tables[0];
            }
            else
            {
                totalcount = 0;
            }
            return dt;
        }

        /// <summary>
        /// 文章分页列表(自动页面大小)
        /// </summary>
        /// <param name="channel_name">频道名称</param>
        /// <param name="category_id">分类ID</param>
        /// <param name="page_size">分页大小</param>
        /// <param name="page_index">当前页码</param>
        /// <param name="strwhere">查询条件</param>
        /// <param name="totalcount">总记录数</param>
        /// <param name="_key">URL配置名称</param>
        /// <param name="_params">传输参数</param>
        /// <returns>DataTable</returns>
        public DataTable get_article_list(string channel_name, int category_id, int page_size, int page_index, string strwhere, out int totalcount, out string pagelist, string _key, params object[] _params)
        {
            DataTable dt = new DataTable();
            if (!string.IsNullOrEmpty(channel_name))
            {
                dt = new BLL.article().ArticleList(channel_name, category_id, page_size, page_index, strwhere, "sort_id asc,add_time desc", out totalcount).Tables[0];
                pagelist = Utils.OutPageList(page_size, page_index, totalcount, linkurl(_key, _params), 8);
            }
            else
            {
                totalcount = 0;
                pagelist = "";
            }
            return dt;
        }

        /// <summary>
        /// 文章分页列表(自动页面大小)
        /// </summary>
        /// <param name="channel_name">频道名称</param>
        /// <param name="category_id">分类ID</param>
        /// <param name="page_size">分页大小</param>
        /// <param name="page_index">当前页码</param>
        /// <param name="strwhere">查询条件</param>
        /// <param name="totalcount">总记录数</param>
        /// <param name="_key">URL配置名称</param>
        /// <param name="_call_index">调用名</param>
        /// <param name="_id">类别ID</param>
        /// <returns>DataTable</returns>
        public DataTable get_article_list_pag(string channel_name, int category_id, int page_size, int page_index, string strwhere, out int totalcount, out string pagelist, string _key, string _call_index, int _id)
        {
            if (!string.IsNullOrEmpty(_call_index))
            {
                return get_article_list(channel_name, category_id, page_size, page_index, strwhere, out totalcount, out pagelist, _key, _call_index, "__id__");
            }
            else
            {
                return get_article_list(channel_name, category_id, page_size, page_index, strwhere, out totalcount, out pagelist, _key, _id, "__id__");
            }
        }

        /// <summary>
        /// 文章分页列表(可排序)
        /// </summary>
        /// <param name="channel_name">频道名称</param>
        /// <param name="category_id">分类ID</param>
        /// <param name="page_size">分页大小</param>
        /// <param name="page_index">当前页码</param>
        /// <param name="strwhere">查询条件</param>
        /// <param name="orderby">排序</param>
        /// <param name="totalcount">总记录数</param>
        /// <param name="_key">URL配置名称</param>
        /// <param name="_params">传输参数</param>
        /// <returns>DataTable</returns>
        public DataTable get_article_list(string channel_name, int category_id, int page_size, int page_index, string strwhere, string orderby, out int totalcount, out string pagelist, string _key, params object[] _params)
        {
            DataTable dt = new DataTable();
            if (!string.IsNullOrEmpty(channel_name))
            {
                dt = new BLL.article().ArticleList(channel_name, category_id, page_size, page_index, strwhere, orderby, out totalcount).Tables[0];
                pagelist = Utils.OutPageList(page_size, page_index, totalcount, linkurl(_key, _params), 8);
            }
            else
            {
                totalcount = 0;
                pagelist = "";
            }
            return dt;
        }

        /// <summary>
        /// 根据频道及规格获得分页列表
        /// </summary>
        /// <param name="channel_name">频道名称</param>
        /// <param name="category_id">分类ID</param>
        /// <param name="spec_ids">规格列表</param>
        /// <param name="page_size">分页大小</param>
        /// <param name="page_index">当前页码</param>
        /// <param name="strwhere">查询条件</param>
        /// <param name="orderby">排序</param>
        /// <param name="totalcount">总记录数</param>
        /// <returns>DataTable</returns>
        public DataTable get_article_list(string channel_name, int category_id, Dictionary<string, string> spec_ids, int page_size, int page_index, string strwhere, string orderby, out int totalcount)
        {
            DataTable dt = new DataTable();
            if (!string.IsNullOrEmpty(channel_name))
            {
                dt = new BLL.article().ArticleList(channel_name, category_id, spec_ids, page_size, page_index, strwhere, orderby, out totalcount).Tables[0];
            }
            else
            {
                totalcount = 0;
            }
            return dt;
        }

        /// <summary>
        /// 获取站点的所有文章  根据关健字查询分页数据
        /// </summary>
        /// <param name="site_id">站点ID</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="strWhere">条件过滤</param>
        /// <param name="filedOrder">排序</param>
        /// <param name="recordCount">文章总数</param>
        /// <returns></returns>
        public DataSet get_article_all(int site_id, int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            DataSet dt = new DataSet();
            if (site_id == 0)
            {
                recordCount = 0;
            }
            else
            {
                dt = new BLL.article().ArticleSearch(site_id, pageSize, pageIndex, strWhere, filedOrder, out recordCount);
            }
            return dt;
        }
        #endregion

        #region 内容标签======================================
        /// <summary>
        /// 根据ID取得内容图片列表
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>String</returns>
        public DataTable get_article_albums(int article_id)
        {
            if (article_id > 0)
            {
                return new BLL.article_albums().GetImagesList(article_id);
            }
            return null;
        }

        //图片列表
        public List<Model.article_albums> get_article_albums(int channel_id, int article_id)
        {
            if (article_id > 0 && channel_id > 0)
            {
                return new BLL.article_albums().GetList(channel_id, article_id);
            }
            return null;
        }

        //附件列表
        public List<Model.article_attach> get_article_attach(int channel_id, int article_id)
        {
            if (article_id > 0 && channel_id > 0)
            {
                return new BLL.article_attach().GetList(channel_id, article_id);
            }
            return null;
        }

        //自定义参数列表
        public List<Model.article_attribute> get_article_attribute(int channel_id, int article_id)
        {
            if (article_id > 0 && channel_id > 0)
            {
                return new BLL.article_attribute().GetList(channel_id, article_id);
            }
            return null;
        }

        /// <summary>
        /// 根据ID取得模型
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>String</returns>
        public Model.article get_article_model(int channel_id, int id)
        {
            if (id > 0)
            {
                BLL.article bll = new BLL.article();
                if (bll.Exists(channel_id, id))
                {
                    return bll.GetModel(channel_id, id);
                }
            }
            return null;
        }

        /// <summary>
        /// 根据channel_name取得模型
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>String</returns>
        public Model.article get_article_model(string channel_name, int id)
        {
            if (id > 0)
            {
                BLL.article bll = new BLL.article();
                if (bll.ArticleExists(channel_name, id))
                {
                    return bll.ArticleModel(channel_name, id);
                }
            }
            return null;
        }

        /// <summary>
        /// 根据调用标识取得内容
        /// </summary>
        /// <param name="call_index">调用别名</param>
        /// <returns>String</returns>
        public string get_article_content(int channel_id, string call_index)
        {
            if (string.IsNullOrEmpty(call_index))
                return string.Empty;
            BLL.article bll = new BLL.article();
            if (bll.Exists(channel_id, call_index))
            {
                return bll.GetModel(channel_id, call_index).content;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取对应的图片路径
        /// </summary>
        /// <param name="article_id">信息ID</param>
        /// <returns>String</returns>
        public string get_article_img_url(int channel_id, int article_id)
        {
            Model.article model = new BLL.article().GetModel(channel_id, article_id);
            if (model != null)
            {
                return model.img_url;
            }
            return "";
        }

        /// <summary>
        /// 获取对应的图片路径
        /// </summary>
        /// <param name="article_id">信息ID</param>
        /// <returns>String</returns>
        public string get_article_img_url(string channel_name, int article_id)
        {
            Model.article model = new BLL.article().GetModel(channel_name, article_id);
            if (model != null)
            {
                return model.img_url;
            }
            return "";
        }

        /// <summary>
        /// 获取对应的标题
        /// </summary>
        /// <param name="article_id">信息ID</param>
        /// <returns>String</returns>
        public string get_article_title(string channel_name, int article_id)
        {
            Model.article model = new BLL.article().GetModel(channel_name, article_id);
            if (model != null)
            {
                return model.title;
            }
            return "";
        }
        /// <summary>
        /// 获取扩展字段的值
        /// </summary>
        /// <param name="article_id">内容ID</param>
        /// <param name="field_name">扩展字段名</param>
        /// <returns>String</returns>
        public string get_article_field(int channel_id, int article_id, string field_name)
        {
            Model.article model = new BLL.article().GetModel(channel_id, article_id);
            if (model != null && model.fields.ContainsKey(field_name))
            {
                return model.fields[field_name];
            }
            return string.Empty;
        }
        public string get_article_field(string channel_name, int article_id, string field_name)
        {
            Model.article model = new BLL.article().GetModel(channel_name, article_id);
            if (model != null && model.fields.ContainsKey(field_name))
            {
                return model.fields[field_name];
            }
            return string.Empty;
        }


        /// <summary>
        /// 获取扩展字段的值
        /// </summary>
        /// <param name="call_index">调用别名</param>
        /// <param name="field_name">扩展字段名</param>
        /// <returns>String</returns>
        public string get_article_field(int channel_id, string call_index, string field_name)
        {
            if (string.IsNullOrEmpty(call_index))
                return string.Empty;
            BLL.article bll = new BLL.article();
            if (!bll.Exists(channel_id, call_index))
            {
                return string.Empty;
            }
            Model.article model = bll.GetModel(channel_id, call_index);
            if (model != null && model.fields.ContainsKey(field_name))
            {
                return model.fields[field_name];
            }
            return string.Empty;
        }
        #endregion

        #region 扩展方法===========================================
        //获取扩展字段对称值
        public Dictionary<string, string> get_field_article_getfields(int channel_id, int article_id, string strWhere)
        {
            return new BLL.article_attribute_field().GetFields(channel_id, article_id, strWhere);
        }
        #endregion

        #region  自增方法
        #region 获取内容第一张图片 不存在返回一个默认图片
       
        #endregion

        /// <summary>
        /// 是否为视频文件
        /// </summary>
        /// <param name="_fileExt">文件扩展名，不含“.”</param>
        public bool IsVideo(string _fileExt)
        {
            ArrayList al = new ArrayList();
            al.Add(".mp3");
            al.Add(".mp4");
            al.Add(".avi");
            al.Add(".flv");
            if (al.Contains(_fileExt.ToLower()))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取对应频道的搜索内容
        /// </summary>
        /// <param name="channel_id"></param>
        /// <returns></returns>
        public List<Model.search_keys> searchList(int channel_id)
        {
            return SearchBuy.GetList(channel_id); //商品列表
        }

        //计算两个日期之间相差的天数
        public int DateDiff(DateTime dateStart, DateTime dateEnd)
        {
            DateTime start = Convert.ToDateTime(dateStart.ToShortDateString());
            DateTime end = Convert.ToDateTime(dateEnd.ToShortDateString());

            TimeSpan sp = end.Subtract(start);

            return sp.Days;
        }

        //统计文章数量
        public int ArticleCount()
        {
            DataTable dt = new BLL.site_channel().FillDataSet(0, "", "").Tables[0];
            int count = 0;
            foreach (DataRow item in dt.Rows)
            {
                count += new BLL.article().GetCount(item["name"].ToString(), "status=0");
            }
            return count;
        }
        #endregion
    }
}
