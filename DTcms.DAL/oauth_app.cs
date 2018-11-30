using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using DTcms.DBUtility;
using DTcms.Common;
using System.Linq;

namespace DTcms.DAL
{
    /// <summary>
    /// 数据访问类:OAuth应用
    /// </summary>
    public partial class oauth_app : DapperRepository<Model.oauth_app>
    {
        private string databaseprefix; //数据库表名前缀
        public oauth_app(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }

        #region 基本方法================================
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            //删除站点OAtuh应用
            StringBuilder strSql1 = new StringBuilder();
            strSql1.Append("delete from " + databaseprefix + "site_oauth");
            strSql1.Append(" where oauth_id=@0");
            WriteDataBase.Execute(strSql1.ToString(), id);

            //删除主表
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from  " + databaseprefix + "oauth_app ");
            strSql.Append(" where id=@0");
            int rows = WriteDataBase.Execute(strSql.ToString(), id);

            return rows > 0;
        }
        
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.oauth_app GetModel(string api_path)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.oauth_app model = new Model.oauth_app();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                str1.Append(p.Name + ",");//拼接字段
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from " + databaseprefix + "oauth_app");
            strSql.Append(" where api_path=@0");
            return ReadDataBase.Query<Model.oauth_app>(strSql.ToString(), api_path).FirstOrDefault();
        }
        #endregion

        #region 扩展方法================================
        /// <summary>
        /// 获取站点未添加数据
        /// </summary>
        public DataSet GetList(int site_id, int oauth_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM  " + databaseprefix + "oauth_app");
            strSql.Append(" where is_lock=0 and id not in(");
            strSql.Append("select oauth_id from " + databaseprefix + "site_oauth where site_id=" + site_id);
            if (oauth_id > 0)
            {
                strSql.Append(" and oauth_id<>" + oauth_id);
            }
            strSql.Append(")");
            strSql.Append(" order by sort_id asc,id desc");
            return WriteDataBase.QueryFillDataSet(strSql.ToString());
        }
        #endregion
    }
}