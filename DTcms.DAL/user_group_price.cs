using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DTcms.DBUtility;
using DTcms.Common;
using System.Reflection;
using System.Linq;

namespace DTcms.DAL
{
	/// <summary>
	/// 数据访问类:会员价格
	/// </summary>
    public partial class user_group_price : DapperRepository<Model.user_group_price>
    {
        private string databaseprefix; //数据库表名前缀
        public user_group_price(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.user_group_price GetModel(int goods_id, int group_id)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.user_group_price model = new Model.user_group_price();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                str1.Append(p.Name + ",");//拼接字段
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from " + databaseprefix + "user_group_price");
            strSql.Append(" where goods_id=@0 and group_id=@1");

            return ReadDataBase.Query<Model.user_group_price>(strSql.ToString(), goods_id, group_id).FirstOrDefault();
        }
        #endregion
    }
}

