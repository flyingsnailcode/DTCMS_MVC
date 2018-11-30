using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;

namespace DTcms.DAL
{
    /// <summary>
    /// 数据访问类:网站授权
    /// </summary>
    public partial class sn_myaspx : DapperRepository<Model.sn_myaspx>
    {
        private string databaseprefix; //数据库表名前缀
        public sn_myaspx(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
       

        #endregion
    }
}