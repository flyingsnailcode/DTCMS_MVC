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
    /// 数据访问类:用户充值记录
    /// </summary>
    public partial class user_recharge : DapperRepository<Model.user_recharge>
    {
        private string databaseprefix; //数据库表名前缀
        public user_recharge(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }
        #region 扩展方法================================
        /// <summary>
        /// 直接充值订单
        /// </summary>
        /// <summary>
        /// 直接充值订单
        /// </summary>
        public bool Recharge(Model.user_recharge model)
        {
            int i = 0;
            using (IDbConnection conn = new DapperView().Context())
            {
                using (IDbTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        #region 增加一条账户余额记录===============
                        Model.user_amount_log amountModel = new Model.user_amount_log();
                        amountModel.user_id = model.user_id;
                        amountModel.user_name = model.user_name;
                        amountModel.value = model.amount;
                        amountModel.remark = "在线充值，单号：" + model.recharge_no;
                        amountModel.add_time = DateTime.Now;
                        new DAL.user_amount_log(databaseprefix).Add(conn, trans, amountModel);
                        #endregion

                        #region 添加充值表=========================
                        StringBuilder strSql = new StringBuilder();
                        StringBuilder str1 = new StringBuilder();//数据字段
                        StringBuilder str2 = new StringBuilder();//数据参数
                        //利用反射获得属性的所有公共属性
                        PropertyInfo[] pros = model.GetType().GetProperties();
                        List<object> paras = new List<object>();
                        strSql.Append("insert into " + databaseprefix + "user_recharge(");
                        foreach (PropertyInfo pi in pros)
                        {
                            //如果不是主键则追加sql字符串
                            if (!pi.Name.Equals("id"))
                            {
                                //判断属性值是否为空
                                if (pi.GetValue(model, null) != null && !pi.GetValue(model, null).ToString().Equals(""))
                                {
                                    str1.Append(pi.Name + ",");//拼接字段
                                    str2.Append("@" + i + ",");//声明参数
                                    i++;
                                    paras.Add(pi.GetValue(model, null));//对参数赋值
                                }
                            }
                        }
                        strSql.Append(str1.ToString().Trim(','));
                        strSql.Append(") values (");
                        strSql.Append(str2.ToString().Trim(','));
                        strSql.Append(") ");
                        strSql.Append(";SELECT @@@IDENTITY;");
                        object obj = WriteDataBase.ExecuteScalar<object>(conn, trans, strSql.ToString(), paras.ToArray());
                        model.id = Convert.ToInt32(obj);
                        #endregion

                        trans.Commit();//提交事务
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();//回滚事务
                        return false;
                    }
                }
            }
            return model.id > 0;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id, string user_name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + databaseprefix + "user_recharge ");
            strSql.Append(" where id=@0 and user_name=@1");
            int rows = WriteDataBase.Execute(strSql.ToString(), id, user_name);
            return rows > 0;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.user_recharge GetModel(string recharge_no)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.user_recharge model = new Model.user_recharge();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                str1.Append(p.Name + ",");//拼接字段
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(',') + " from " + databaseprefix + "user_recharge");
            strSql.Append(" where recharge_no=@0");

           return ReadDataBase.Query<Model.user_recharge>(strSql.ToString(), recharge_no).FirstOrDefault();
        }

        /// <summary>
        /// 根据充值单号获取支付方式ID
        /// </summary>
        public int GetPaymentId(string recharge_no)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 payment_id from " + databaseprefix + "user_recharge");
            strSql.Append(" where recharge_no=@recharge_no");
            object obj = ReadDataBase.ExecuteScalar<int>(strSql.ToString(), recharge_no);
            if (obj != null)
            {
                return Convert.ToInt32(obj);
            }
            return 0;
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public List<Model.user_recharge> GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM " + databaseprefix + "user_recharge");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            recordCount = ReadDataBase.ExecuteScalar<int>(PagingHelper.CreateCountingSql(strSql.ToString()));
            return GetModelList(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }


        /// <summary>
        /// 确认充值订单
        /// </summary>
        public bool Confirm(string recharge_no)
        {
            Model.user_recharge model = GetModel("recharge_no='"+ recharge_no + "'","","");//根据充值单号得到实体
            if (model == null)
            {
                return false;
            }
            using (IDbConnection conn = new DapperView().Context())
            {
                using (IDbTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        #region 增加一条账户余额记录===============
                        Model.user_amount_log amountModel = new Model.user_amount_log();
                        amountModel.user_id = model.user_id;
                        amountModel.user_name = model.user_name;
                        amountModel.value = model.amount;
                        amountModel.remark = "在线充值，单号：" + recharge_no;
                        amountModel.add_time = DateTime.Now;
                        new DAL.user_amount_log(databaseprefix).Add(conn, trans, amountModel);
                        #endregion

                        #region 更新充值表=========================
                        StringBuilder strSql = new StringBuilder();
                        strSql.Append("update " + databaseprefix + "user_recharge set ");
                        strSql.Append("status=@0,");
                        strSql.Append("complete_time=@1");
                        strSql.Append(" where recharge_no=@2");
                        WriteDataBase.Execute(conn, trans, strSql.ToString(), 1, DateTime.Now, recharge_no);
                        #endregion
                        trans.Commit();//提交事务
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();//回滚事务
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion
    }
}

