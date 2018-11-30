using DTcms.Common;
using DTcms.Search;
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
        /// <summary>
        /// 查询Lucene.NET
        /// </summary>
        /// <param name="top">数量</param>
        /// <param name="site_id">网站ID</param>
        /// <param name="channel_id">频道ID</param>
        /// <param name="category_id">类别ID</param>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        public DataTable get_lucene_list(int top, int site_id, int channel_id, int category_id, string keywords)
        {
            DataTable dt = new DataTable();
            if (site_id > 0 && null != keywords && keywords.Length > 1)
            {
                dt = LuceneHelper.Search(LuceneHelper.INDEX_DIR, top, site_id, channel_id, category_id, keywords);
            }
            return dt;
        }

        /// <summary>
        /// 查询Lucene.NET
        /// </summary>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="pageIndex">当前分页</param>
        /// <param name="site_id">网站ID</param>
        /// <param name="channel_id">频道ID</param>
        /// <param name="category_id">类别ID</param>
        /// <param name="keyword">关键词</param>
        /// <param name="recordCount">结果总数量</param>
        /// <returns></returns>
        public DataTable get_lucene_list(int pageSize, int pageIndex, int site_id, int channel_id, int category_id, string keywords, out int recordCount)
        {
            recordCount = 0;
            DataTable dt = new DataTable();
            if (site_id > 0 && null != keywords && keywords.Length > 0)
            {
                dt = LuceneHelper.Search(LuceneHelper.INDEX_DIR, pageSize, pageIndex, site_id, channel_id, category_id, keywords, out recordCount);
            }
            return dt;
        }
    }
}
