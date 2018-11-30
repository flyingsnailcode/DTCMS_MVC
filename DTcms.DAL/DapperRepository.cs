using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using DTcms.Common;
using DTcms.DBUtility;

namespace DTcms.DAL
{
    public abstract class DapperRepository<T> : DapperView where T : class, new()
    {
        #region 表相关.....
        //public static Model.siteconfig siteConfig = new siteconfig().loadConfig(DTKeys.CACHE_SITE_CONFIG); //获得站点配置信息
        /// <summary>
        /// 获取表名称
        /// </summary>
        public string TableName
        {
            get
            {
                Type type = typeof(T);
                object[] primaryKeyObj = type.GetCustomAttributes(typeof(TableNameAttribute), true);
                if (primaryKeyObj.Count() > 0)//取第一个
                {
                    TableNameAttribute primaryKeyAttr = (TableNameAttribute)primaryKeyObj[0];
                    return primaryKeyAttr.Value;
                }
                return "dt_" + type.Name;
                //return siteConfig.sysdatabaseprefix + type.Name;
            }
        }

        /// <summary>
        /// 获取表名称
        /// </summary>
        public string NewTableName
        {
            get
            {
                Type type = typeof(T);
                object[] primaryKeyObj = type.GetCustomAttributes(typeof(TableNameAttribute), true);
                if (primaryKeyObj.Count() > 0)//取第一个
                {
                    TableNameAttribute primaryKeyAttr = (TableNameAttribute)primaryKeyObj[0];
                    return primaryKeyAttr.Value;
                }
                return type.Name;
            }
        }
        /// <summary>
        /// 获取主键名称
        /// </summary>
        public string PrimaryKey
        {
            get
            {
                Type type = typeof(T);
                object[] tableNameObj = type.GetCustomAttributes(typeof(PrimaryKeyAttribute), true);
                if (tableNameObj.Count() > 0)//取第一个
                {
                    PrimaryKeyAttribute tableNameAttr = (PrimaryKeyAttribute)tableNameObj[0];
                    return tableNameAttr.Value;
                }
                return "ID";
            }
        }
        #endregion

        /// <summary>
        ///单独执行一条sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public int Execute(string sql, params object[] args)
        {
            return ReadDataBase.Execute(sql, args);
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        /// <param name="Top">数量</param>
        /// <param name="strWhere">条件</param>
        /// <param name="filedOrder">排序</param>
        /// <returns>DataTable</returns>
        public DataSet FillDataSet(int Top, string sqlwhere, string orderby)
        {
            try
            {
                if (string.IsNullOrEmpty(sqlwhere))
                {
                    sqlwhere = "1=1";
                }
                if (string.IsNullOrEmpty(orderby))
                {
                    orderby = "id desc";
                }
                DataSet ds = new DataSet();
                if (Top > 0)
                {
                    ds = ReadDataBase.QueryFillDataSet(Sql.Builder.Select(string.Format("top {0} *", Top)).From(TableName).Where(sqlwhere).OrderBy(orderby));
                }
                else
                {
                    ds = ReadDataBase.QueryFillDataSet(Sql.Builder.Select("*").From(TableName).Where(sqlwhere).OrderBy(orderby));
                }
                return ds;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                ReadDataBase.CloseSharedConnection();
                return null;
            }
            finally
            {
                ReadDataBase.CloseSharedConnection();
            }
        }

        /// <summary>
        /// Page分页
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="sqlwhere">条件</param>
        /// <param name="orderby">排序</param>
        /// <returns></returns>
        public Page<T> GetQueryPage(int pageIndex, int pageSize, string sqlwhere, string orderby)
        {
            try
            {
                if (string.IsNullOrEmpty(sqlwhere))
                {
                    sqlwhere = "1=1";
                }
                if (string.IsNullOrEmpty(orderby))
                {
                    orderby = "id desc";
                }
                Page<T> pageModel = ReadDataBase.Page<T>(pageIndex, pageSize, Sql.Builder.Select("*").From(TableName).Where(sqlwhere).OrderBy(orderby));
                return pageModel;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                ReadDataBase.CloseSharedConnection();
                return null;
            }
            finally
            {
                ReadDataBase.CloseSharedConnection();
            }
        }

        /// <summary>
        /// 是否存在记录
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns>bool</returns>
        public virtual T GetModel(int id)
        {
            string sqlwhere = string.Empty;
            if (id > 0)
            {
                sqlwhere = "id=" + id;
            }
            try
            {
                T t = WriteDataBase.SingleOrDefault<T>(Sql.Builder.From(TableName).Where(sqlwhere));
                return t;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                WriteDataBase.CloseSharedConnection();
                return null;
            }
            finally
            {
                WriteDataBase.CloseSharedConnection();
            }
        }

        /// <summary>
        /// 是否存在记录
        /// </summary>
        /// <param name="sqlwhere">条件</param>
        /// <param name="files">查询字段</param>
        /// <param name="orderby">排序</param>
        /// <returns>bool</returns>
        public virtual T GetModel(string sqlwhere, string files, string orderby)
        {
            if (string.IsNullOrEmpty(sqlwhere))
            {
                sqlwhere = "1=1";
            }
            if (string.IsNullOrEmpty(files))
            {
                files = "*";
            }
            if (string.IsNullOrEmpty(orderby))
            {
                orderby = "id desc";
            }
            try
            {
                T t = WriteDataBase.SingleOrDefault<T>(Sql.Builder.Select(files).From(TableName).Where(sqlwhere).OrderBy(orderby));
                return t;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                WriteDataBase.CloseSharedConnection();
                return null;
            }
            finally
            {
                WriteDataBase.CloseSharedConnection();
            }
        }

        /// <summary>
        /// 获取一个集合
        /// </summary>
        /// <param name="sqlwhere">条件</param>
        /// <param name="files">查询字段</param>
        /// <param name="orderby">排序</param>
        /// <returns>bool</returns>
        public List<T> GetModelList(int top, string sqlwhere, string files, string orderby)
        {
            if (string.IsNullOrEmpty(sqlwhere))
            {
                sqlwhere = "1=1";
            }
            if (string.IsNullOrEmpty(files))
            {
                files = "*";
            }
            if (string.IsNullOrEmpty(orderby))
            {
                orderby = "id desc";
            }
            try
            {
                List<T> t = null;
                if (top != 0)
                {
                    t = WriteDataBase.Query<T>(Sql.Builder.Select(string.Format("top {0} {1}", top, files)).From(TableName).Where(sqlwhere).OrderBy(orderby)).ToList();
                }
                else
                {
                    t = WriteDataBase.Query<T>(Sql.Builder.Select(files).From(TableName).Where(sqlwhere).OrderBy(orderby)).ToList();
                }
                return t;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                WriteDataBase.CloseSharedConnection();
                return null;
            }
            finally
            {
                WriteDataBase.CloseSharedConnection();
            }
        }

        /// <summary>
        /// 获取一个集合
        /// </summary>
        /// <param name="sqlwhere">条件</param>
        /// <returns>bool</returns>
        public List<T> GetModelList(string sql, params object[] args)
        {
            try
            {
                List<T> t = WriteDataBase.Query<T>(sql, args).ToList();
                return t;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                WriteDataBase.CloseSharedConnection();
                return null;
            }
            finally
            {
                WriteDataBase.CloseSharedConnection();
            }
        }

        /// <summary>
        /// 根据条件修改
        /// </summary>
        /// <param name="sqlwhere">条件</param>
        /// <param name="files">字段</param>
        /// <returns>bool</returns>
        public bool UpdateFile(string sqlwhere, string files)
        {
            bool isSuccess = true;
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("update " + TableName + " set " + files + " ");
                if (sqlwhere.Trim() != "")
                {
                    strSql.Append(" where " + sqlwhere);
                }
                isSuccess = ReadDataBase.Execute(strSql.ToString()) > 0;
            }
            catch (Exception e)
            {
                isSuccess = false;
            }
            finally
            {
                ReadDataBase.CloseSharedConnection();
            }

            return isSuccess;
        }

        /// <summary>
        /// 根据ID获取记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Get(object id)
        {
            try
            {
                T t = ReadDataBase.SingleOrDefault<T>(Sql.Builder.Select("*").From(TableName).Where(PrimaryKey + "=@0", id));
                return t;
            }
            catch (Exception ex)
            {

                LogHelper.Error(ex);
                ReadDataBase.CloseSharedConnection();
                return null;
            }
            finally
            {
                ReadDataBase.CloseSharedConnection();
            }
        }

        /// <summary>
        /// 获取该表的所有实体对象
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetAll()
        {
            try
            {
                IEnumerable<T> listT = ReadDataBase.Query<T>(Sql.Builder.Select("*").From(TableName));
                return listT;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                ReadDataBase.CloseSharedConnection();
                return null;
            }
            finally
            {
                ReadDataBase.CloseSharedConnection();
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="query"></param>
        /// <param name="model"></param>
        /// <param name="context"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public int Add(string query, T model, IDbTransaction transaction = null)
        {
            int id = WriteDataBase.Execute(query, model, transaction);
            return id;
        }

        /// <summary>
        /// 保存实体
        /// </summary>
        /// <param name="t">数据实体</param>
        /// <param name="primaryKey">库表主键</param>
        /// <returns></returns>
        public bool Save(T t, string primaryKey = "id")
        {
            bool isSuccess = true;
            try
            {
                WriteDataBase.BeginTransaction();
                object obj = WriteDataBase.Insert(TableName, primaryKey, t);
                WriteDataBase.CompleteTransaction();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                isSuccess = false;
                WriteDataBase.CloseSharedConnection();
                LogHelper.Error(ex);
            }
            finally
            {
                WriteDataBase.CloseSharedConnection();
            }
            return isSuccess;
        }

        /// <summary>
        /// 保存实体 返回整形ID
        /// </summary>
        /// <param name="t">数据实体</param>
        /// <param name="primaryKey">库表主键</param>
        /// <returns></returns>
        public int Add(T t, string primaryKey = "id")
        {
            int isSuccess = 0;
            try
            {
                WriteDataBase.BeginTransaction();
                object obj = WriteDataBase.Insert(TableName, primaryKey, t);
                isSuccess = Convert.ToInt32(obj);
                WriteDataBase.CompleteTransaction();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                isSuccess = 0;
                WriteDataBase.CloseSharedConnection();
                LogHelper.Error(ex);
            }
            finally
            {
                WriteDataBase.CloseSharedConnection();
            }
            return isSuccess;
        }


        /// <summary>
        /// 根据实体进行更新操作   
        /// </summary>
        /// <param name="t">数据实体</param>
        /// <param name="primaryKey">库表主键</param>
        /// <returns></returns>
        public bool Update(T t, string primaryKey = "id")
        {
            bool isSuccess = true;
            try
            {
                WriteDataBase.BeginTransaction();
                isSuccess = WriteDataBase.Update(TableName, primaryKey, t) > 0;
                WriteDataBase.CompleteTransaction();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                isSuccess = false;
                WriteDataBase.CloseSharedConnection();
            }
            finally
            {
                WriteDataBase.CloseSharedConnection();
            }
            return isSuccess;
        }

        /// <summary>
        /// 根据实体进行删除
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual bool Delete(T t)
        {
            bool isSuccess = true;
            try
            {
                WriteDataBase.BeginTransaction();
                isSuccess = WriteDataBase.Delete(TableName, "id", t) > 0;
                WriteDataBase.CompleteTransaction();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                WriteDataBase.CloseSharedConnection();
                isSuccess = false;
            }
            finally
            {
                WriteDataBase.CloseSharedConnection();
            }
            return isSuccess;
        }

        /// <summary>
        /// 根据主键进行删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Delete(object id)
        {
            try
            {
                T t = Get(id);
                if (t != null)
                    return Delete(t);
                else
                    return false;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                WriteDataBase.CloseSharedConnection();
                return false;
            }
            finally
            {
                WriteDataBase.CloseSharedConnection();
            }
        }

        /// <summary>
        /// 根据条件进行删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Delete(string sqlwhere)
        {
            bool isSuccess = true;
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("delete " + TableName + " ");
                if (sqlwhere.Trim() != "")
                {
                    strSql.Append(" where " + sqlwhere);
                }
                isSuccess = WriteDataBase.Execute(strSql.ToString()) > 0;
            }
            catch (Exception e)
            {
                isSuccess = false;
            }
            return isSuccess;
        }

        /// <summary>
        /// 根据条件进行删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual int Delete_Int(string sqlwhere)
        {
            int isSuccess = 0;
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("delete " + TableName + " ");
                if (sqlwhere.Trim() != "")
                {
                    strSql.Append(" where " + sqlwhere);
                }
                isSuccess = WriteDataBase.Execute(strSql.ToString());
            }
            catch (Exception e)
            {
                isSuccess = 0;
            }
            return isSuccess;
        }

        /// <summary>
        /// 更具条件获取，指定的字段
        /// </summary>
        /// <param name="files">字段（不填写者默认查询全部）</param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public T GeModeltByFile(string files, string where)
        {
            try
            {
                if (string.IsNullOrEmpty(files))
                {
                    files = "*";
                }
                T t = ReadDataBase.SingleOrDefault<T>(Sql.Builder.Select(files).From(TableName).Where(where));
                return t;
            }
            catch (Exception ex)
            {

                LogHelper.Error(ex);
                ReadDataBase.CloseSharedConnection();
                return null;
            }
            finally
            {
                ReadDataBase.CloseSharedConnection();
            }
        }

        /// <summary>
        /// 更新或保存记录
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool SaveOrUpdate(T t)
        {
            try
            {
                Type type = typeof(T);
                object objPrimary = type.GetProperty(PrimaryKey).GetValue(t, null);
                if (objPrimary == null)
                    return Save(t);
                else
                    return Update(t);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                WriteDataBase.CloseSharedConnection();
                return false;
            }
            finally
            {

                WriteDataBase.CloseSharedConnection();
            }

        }

        /// <summary>
        /// 多表关联保存
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Merge(T t)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Page分页
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页容量</param>
        /// <returns></returns>
        public Page<T> GetQueryPage(int pageIndex, int pageSize)
        {
            try
            {
                Page<T> pageModel = ReadDataBase.Page<T>(pageIndex, pageSize, Sql.Builder.Select("*").From(TableName));
                return pageModel;
            }
            catch (Exception ex)
            {


                LogHelper.Error(ex);
                ReadDataBase.CloseSharedConnection();
                return null;
            }
            finally
            {
                ReadDataBase.CloseSharedConnection();
            }
        }

        /// <summary>
        /// Page分页（手机版）todo：（弃用）
        /// </summary>
        /// <param name="search">search</param>
        /// <returns></returns>
        public Page<T> Page(Search search)
        {
            try
            {
                Page<T> pageModel;
                object[] args;
                string strWhere = search.GetConditonByDapper(out args);

                pageModel = ReadDataBase.Page<T>(search.CurrentPageIndex, search.PageSize, Sql.Builder.Select(search.SelectedColums).From(search.TableName).Where(strWhere, args).OrderBy(search.SortField));

                return pageModel;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                ReadDataBase.CloseSharedConnection();
                return null;
            }
            finally
            {
                ReadDataBase.CloseSharedConnection();
            }
        }

        /// <summary>
        /// Page分页（DataTable.js）
        /// </summary>
        /// <param name="search">search</param>
        /// <returns></returns>
        public TablePage<T> GetQueryPage(Search search)
        {
            try
            {
                Page<T> pageModel;
                object[] args;
                string strWhere = search.GetConditonByDapper(out args);

                pageModel = ReadDataBase.Page<T>(search.CurrentPageIndex, search.PageSize, Sql.Builder.Select(search.SelectedColums).From(search.TableName).Where(strWhere, args).OrderBy(search.SortField));


                TablePage<T> tablePage = new TablePage<T>();
                if (pageModel != null)
                {
                    tablePage.draw = search.sEcho;
                    tablePage.recordsTotal = pageModel.TotalItems;
                    tablePage.recordsFiltered = pageModel.TotalItems;
                    tablePage.aaData = pageModel.Items;
                }

                return tablePage;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                ReadDataBase.CloseSharedConnection();
                return null;
            }
            finally
            {
                ReadDataBase.CloseSharedConnection();
            }
        }


        /// <summary>
        /// Page分页（DataTable.js）
        /// </summary>
        /// <param name="search">search</param>
        /// <returns></returns>
        public TablePage<T> jsonDataTable(List<T> jsonlist)
        {
            try
            {
                TablePage<T> tablePage = new TablePage<T>();
                if (jsonlist != null)
                {
                    tablePage.draw = 0;
                    tablePage.recordsTotal = jsonlist.Count;
                    tablePage.recordsFiltered = jsonlist.Count;
                    tablePage.aaData = jsonlist;
                }
                return tablePage;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                ReadDataBase.CloseSharedConnection();
                return null;
            }
            finally
            {
                ReadDataBase.CloseSharedConnection();
            }
        }

        /// <summary>
        /// SkipTake分页
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页容量</param>
        /// <returns></returns>
        public List<T> GetQueryPageSkipTake(int pageIndex, int pageSize)
        {
            try
            {
                List<T> pageModel = ReadDataBase.SkipTake<T>((pageIndex - 1) * pageSize, pageSize, Sql.Builder.Select("*").From(TableName));
                return pageModel;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                ReadDataBase.CloseSharedConnection();
                return null;
            }
            finally
            {
                ReadDataBase.CloseSharedConnection();
            }
        }
        /// <summary>
        /// SkipTake分页
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="sqlwhere">条件</param>
        /// <param name="orderby">排序</param>
        /// <returns></returns>
        public List<T> GetQueryPageSkipTake(int pageIndex, int pageSize, string sqlwhere, string orderby)
        {
            try
            {
                if (string.IsNullOrEmpty(sqlwhere))
                {
                    sqlwhere = "1=1";
                }
                if (string.IsNullOrEmpty(orderby))
                {
                    orderby = "id desc";
                }
                List<T> pageModel = ReadDataBase.SkipTake<T>((pageIndex - 1) * pageSize, pageSize, Sql.Builder.Select("*").From(TableName).Where(sqlwhere).OrderBy(orderby));
                return pageModel;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                ReadDataBase.CloseSharedConnection();
                return null;
            }
            finally
            {
                ReadDataBase.CloseSharedConnection();
            }
        }

        /// <summary>
        /// Fetch分页
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页容量</param>
        /// <returns></returns>
        public List<T> GetQueryPageFetch(int pageIndex, int pageSize)
        {
            try
            {
                List<T> pageModel = ReadDataBase.Fetch<T>((pageIndex - 1) * pageSize, pageSize, Sql.Builder.Select("*").From(TableName));
                return pageModel;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                ReadDataBase.CloseSharedConnection();
                return null;
            }
            finally
            {
                ReadDataBase.CloseSharedConnection();
            }
        }

        /// <summary>
        /// 获取分页查询集合,返回json
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns></returns>
        public string QueryPageToJson(Search search)
        {

            try
            {
                int total = 0;
                DataSet ds = QueryPageList(search, out total);
                string jsonResult = JsonHelper.DataTableToJsonGrid(ds.Tables[0], total, search.sEcho);

                return jsonResult;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                ReadDataBase.CloseSharedConnection();
                return null;
            }
            finally
            {
                ReadDataBase.CloseSharedConnection();
            }
        }

        /// <summary>
        /// 获取分页查询集合,返回 DataSet
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns></returns>
        public DataSet QueryPageToJson(Search search, out int total)
        {
            total = 0;
            try
            {
                DataSet ds = QueryPageList(search, out total);
                return ds;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                ReadDataBase.CloseSharedConnection();
                return null;
            }
            finally
            {
                ReadDataBase.CloseSharedConnection();
            }
        }

        /// <summary>
        /// 查询分页数据
        /// </summary>
        /// <param name="search"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public DataSet QueryPageList(Search search, out int total)
        {
            total = 0;
            SqlParameter[] pms =
            {
                new SqlParameter("@FieldKey",search.KeyFiled),
                new SqlParameter("@FieldShow",search.SelectedColums),
                new SqlParameter("@tbname",search.TableName),
                new SqlParameter("@Where",search.GetConditon()),
                new SqlParameter("@FieldOrder",search.SortField),
                new SqlParameter("@PageCurrent",search.CurrentPageIndex),
                new SqlParameter("@PageSize",search.PageSize)
            };
            DataSet ds = ReadDataBase.QueryFillDataSet("SP_PageList", CommandType.StoredProcedure, pms);
            if (ds.Tables.Count != 2 || ds.Tables[1].Rows.Count == 0)
            {
                return null;
            }
            total = Convert.ToInt32(ds.Tables[1].Rows[0]["TotalCount"]);
            return ds;
        }


        /*
        /// <summary>
        /// 获取分页查询集合,返回json
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public string QueryPageToJson(Search search)
        {
            int total = 0;
            DataSet ds = QueryPageList(search, out total);
            string jsonResult = JsonHelper.DataTableToJsonGrid(ds.Tables[0], total, search.sEcho);
            return jsonResult;
        }

        /// <summary>
        /// 查询分页数据
        /// </summary>
        /// <param name="search"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public DataSet QueryPageList(Search search, out int total)
        {
            total = 0;
            SqlParameter[] pms = 
            {
                new SqlParameter("@FieldKey",search.KeyFiled),
                new SqlParameter("@FieldShow",search.SelectedColums),
                new SqlParameter("@tbname",search.TableName),
                new SqlParameter("@Where",search.GetConditon()),
                new SqlParameter("@FieldOrder",search.SortField),
                new SqlParameter("@PageCurrent",search.CurrentPageIndex),
                new SqlParameter("@PageSize",search.PageSize)
            };
            DataSet ds =QueryFillDataSet("SP_PageList", CommandType.StoredProcedure, pms);
            if (ds.Tables.Count != 2 || ds.Tables[1].Rows.Count == 0)
            {
                return null;
            }
            total = Convert.ToInt32(ds.Tables[1].Rows[0]["TotalCount"]);
            return ds;
        }
         */
        /// <summary>
        /// 获取该表的所有实体对象
        /// </summary>
        /// <param name="companyId">公司ID</param>
        /// <returns></returns>
        public IEnumerable<T> GetAll(string companyId)
        {
            try
            {
                //IEnumerable<T> listT = ReadDataBase.Query<T>(Sql.Builder.Select("*").From(TableName).Where("CompanyId=@0 and Status!=-1", companyId));
                IEnumerable<T> listT = ReadDataBase.Query<T>(Sql.Builder.Select("*").From(TableName));
                return listT;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                ReadDataBase.CloseSharedConnection();
                return null;
            }
            finally
            {
                ReadDataBase.CloseSharedConnection();
            }
        }

        /// <summary>
        /// 统计数据（目前只中用于Article）
        /// </summary>
        /// <param name="field"></param>
        /// <param name="sqlwhere"></param>
        /// <returns></returns>
        public string GetExists(string databaseprefix, string TableName, string field, string sqlwhere)
        {
            string result = string.Empty;
            try
            {
                StringBuilder strSql = new StringBuilder();
                if (string.IsNullOrEmpty(field))
                {
                    field = "*";
                }
                strSql.Append("select " + field + " from " + databaseprefix + DTKeys.TABLE_CHANNEL_ARTICLE + TableName + "");
                if (sqlwhere.Trim() != "")
                {
                    strSql.Append(" where " + sqlwhere);
                }
                result = ReadDataBase.ExecuteScalar<string>(strSql.ToString());
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                ReadDataBase.CloseSharedConnection();
                return null;
            }
            finally
            {
                ReadDataBase.CloseSharedConnection();
            }
        }

        /// <summary>
        /// 统计数据
        /// </summary>
        /// <param name="field"></param>
        /// <param name="sqlwhere"></param>
        /// <returns></returns>
        public string GetCount(string field, string sqlwhere)
        {
            string result = string.Empty;
            try
            {
                StringBuilder strSql = new StringBuilder();
                if (string.IsNullOrEmpty(field))
                {
                    field = "*";
                }
                strSql.Append("select " + field + " from " + TableName + "");
                if (sqlwhere.Trim() != "")
                {
                    strSql.Append(" where " + sqlwhere);
                }
                result = ReadDataBase.ExecuteScalar<string>(strSql.ToString());
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                ReadDataBase.CloseSharedConnection();
                return null;
            }
            finally
            {
                ReadDataBase.CloseSharedConnection();
            }
        }
    }
}
