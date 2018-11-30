using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;
using System.Linq;

namespace DTcms.DAL
{
    /// <summary>
    ///  数据访问类:用户组别
    /// </summary>
    public partial class user_groups : DapperRepository<Model.user_groups>
    {
        private string databaseprefix; //数据库表名前缀
        public user_groups(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 获取会员组折扣
        /// </summary>
        public int GetDiscount(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 discount from " + databaseprefix + "user_groups");
            strSql.Append(" where id=" + id);
            return WriteDataBase.Execute(strSql.ToString());
        }

        /// <summary>
        /// 取得默认组别实体
        /// </summary>
        public Model.user_groups GetDefault()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 *");
            strSql.Append(" from " + databaseprefix + "user_groups");
            strSql.Append(" where is_lock=0 order by is_default desc,id asc");

            return ReadDataBase.Query<Model.user_groups>(strSql.ToString()).FirstOrDefault();
        }
        /// <summary>
        /// 返回标题名称
        /// </summary>
        public string GetTitle(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 title from " + databaseprefix + "user_groups");
            strSql.Append(" where id=@0");
            string title = ReadDataBase.ExecuteScalar<string>(strSql.ToString(), id);
            if (!string.IsNullOrEmpty(title))
            {
                return title;
            }
            return string.Empty;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            //删除会员组价格
            StringBuilder strSql1 = new StringBuilder();
            strSql1.Append("delete from " + databaseprefix + "user_group_price ");
            strSql1.Append(" where group_id=@0 ");
            WriteDataBase.Execute(strSql1.ToString(), id);

            //删除主表
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + databaseprefix + "user_groups ");
            strSql.Append(" where id=@0");

            int rowsAffected = WriteDataBase.Execute(strSql.ToString(), id);
            return rowsAffected > 0;
        }
        #endregion

        /// <summary>
        /// 根据经验值返回升级的组别实体
        /// </summary>
        public Model.user_groups GetUpgrade(int group_id, int exp)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 *");
            strSql.Append(" from " + databaseprefix + "user_groups");
            strSql.Append(" where is_lock=0 and is_upgrade=1 and grade>(select grade from " + databaseprefix + "user_groups where id=" + group_id + ") and upgrade_exp<=" + exp);
            strSql.Append(" order by grade asc");

            return ReadDataBase.Query<Model.user_groups>(strSql.ToString()).FirstOrDefault();
        }
        /// <summary>
        /// 根据充值金额返回升级的组别实体
        /// </summary>
        public Model.user_groups GetUpgradePrice(int group_id, decimal price)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 *");
            strSql.Append(" from " + databaseprefix + "user_groups");
            strSql.Append(" where is_lock=0 and upgrade_price>0 and is_upgrade=1 and grade>(select grade from " + databaseprefix + "user_groups where id=" + group_id + ") and upgrade_price<=" + price);
            strSql.Append(" order by upgrade_price desc,grade asc");
            return ReadDataBase.Query<Model.user_groups>(strSql.ToString()).FirstOrDefault();
        }
    }
}