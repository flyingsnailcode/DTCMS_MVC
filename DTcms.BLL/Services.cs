using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using DTcms.Common;
using DTcms.DAL;

namespace DTcms.BLL
{
    public abstract class Services<T> where T : class, new()
    {
        public static Model.siteconfig siteConfig = new BLL.siteconfig().loadConfig(); //获得站点配置信息

        protected DapperRepository<T> CurrentRepository { get; set; }

        public Services()
        {
            SetCurrentReposiotry();
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
            Page<T> pageModel = CurrentRepository.GetQueryPage(pageIndex, pageSize, sqlwhere, orderby);
            return pageModel;
        }

        /// <summary>
        ///单独执行一条sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public int ExecuteSql(string sql, params object[] args)
        {
            return CurrentRepository.Execute(sql, args);
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        /// <param name="Top">数量</param>
        /// <param name="strWhere">条件</param>
        /// <param name="filedOrder">排序</param>
        /// <returns>DataTable</returns>
        public DataSet FillDataSet(int Top, string strWhere, string filedOrder)
        {
            return CurrentRepository.FillDataSet(Top, strWhere, filedOrder);
        }

        /// <summary>
        /// 获取一个实体
        /// </summary>
        /// <param name="sqlwhere">条件</param>
        /// <param name="files">查询字段</param>
        /// <param name="orderby">排序</param>
        /// <returns>bool</returns>
        public virtual T GetModel(string sqlwhere, string files, string orderby)
        {
            return CurrentRepository.GetModel(sqlwhere, files, orderby);
        }
        /// <summary>
        /// 获取一个实体
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns>bool</returns>
        public virtual T GetModel(int id)
        {
            //return CurrentRepository.GetModel(id);
            return CurrentRepository.Get(id);
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
            return CurrentRepository.GetModelList(top, sqlwhere, files, orderby);
        }

        /// <summary>
        /// 根据条件修改
        /// </summary>
        /// <param name="sqlwhere">条件</param>
        /// <param name="files">查询字段</param>
        /// <param name="orderby">排序</param>
        /// <returns>bool</returns>
        public virtual bool UpdateFile(string sqlwhere, string files)
        {
            return CurrentRepository.UpdateFile(sqlwhere, files);
        }
        
        /// <summary>
        /// 根据ID获取记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T Get(object id)
        {
            return CurrentRepository.Get(id);
        }

        public abstract void SetCurrentReposiotry();
        /// <summary>
        /// 获取该表的所有实体对象
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAll()
        {
            return CurrentRepository.GetAll();
        }

        /// <summary>
        /// 保存该实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual bool Save(T t)
        {
            return CurrentRepository.Save(t);
        }

        /// <summary>
        /// 保存该实体 返回整形id
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual int Add(T t)
        {
            return CurrentRepository.Add(t);
        }


        /// <summary>
        /// 保存该实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual bool Save(T t, string file)
        {
            return CurrentRepository.Save(t, file);
        }

        /// <summary>
        /// 根据实体进行更新操作   
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual bool Update(T t)
        {
            return CurrentRepository.Update(t);
        }

        /// <summary>
        /// 根据实体进行删除
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual bool Delete(T t)
        {
            return CurrentRepository.Delete(t);
        }

        /// <summary>
        /// 根据主键进行删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Delete(object id)
        {
            return CurrentRepository.Delete(id);
        }

        /// <summary>
        /// 根据条件进行删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Delete(string sqlwhere)
        {
            return CurrentRepository.Delete(sqlwhere);
        }

        /// <summary>
        /// 根据条件进行删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual int Delete_Int(string sqlwhere)
        {
            return CurrentRepository.Delete_Int(sqlwhere);
        }

        /// <summary>
        /// 更新或保存记录
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual bool SaveOrUpdate(T t)
        {
            return CurrentRepository.SaveOrUpdate(t);
        }
        /// <summary>
        /// 多表关联保存
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual bool Merge(T t)
        {
            return CurrentRepository.Merge(t);
        }

        /// <summary>
        /// Page分页
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页容量</param>
        /// <returns></returns>
        public Page<T> GetQueryPage(int pageIndex, int pageSize)
        {
            Page<T> pageModel = CurrentRepository.GetQueryPage(pageIndex, pageSize);
            return pageModel;
        }

        /// <summary>
        /// Page分页（手机专版）
        /// </summary>
        /// <param name="search">search</param>
        /// <returns></returns>
        public Page<T> Page(Search search)
        {
            Page<T> pageModel = CurrentRepository.Page(search);
            return pageModel;
        }

        /// <summary>
        /// Page分页
        /// </summary>
        /// <param name="search">search</param>
        /// <returns></returns>
        public TablePage<T> GetQueryPage(Search search)
        {
            TablePage<T> pageModel = CurrentRepository.GetQueryPage(search);
            return pageModel;
        }

        /// <summary>
        /// Page分页
        /// </summary>
        /// <param name="search">search</param>
        /// <returns></returns>
        public TablePage<T> jsonDataTable(List<T> jsonlist)
        {
            TablePage<T> pageModel = CurrentRepository.jsonDataTable(jsonlist);
            return pageModel;
        }


        /// <summary>
        /// SkipTake分页
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页容量</param>
        /// <returns></returns>
        public List<T> GetQueryPageSkipTake(int pageIndex, int pageSize)
        {
            List<T> pageModel = CurrentRepository.GetQueryPageSkipTake(pageIndex, pageSize);
            return pageModel;
        }
        /// <summary>
        /// SkipTake分页
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页容量</param>
        /// <returns></returns>
        public List<T> GetQueryPageSkipTake(int pageIndex, int pageSize, string sqlwhere, string orderby)
        {
            List<T> pageModel = CurrentRepository.GetQueryPageSkipTake(pageIndex, pageSize, sqlwhere, orderby);
            return pageModel;
        }


        /// <summary>
        /// Fetch分页
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页容量</param>
        /// <returns></returns>
        public List<T> GetQueryPageFetch(int pageIndex, int pageSize)
        {
            List<T> pageModel = CurrentRepository.GetQueryPageFetch(pageIndex, pageSize);
            return pageModel;
        }

        /// <summary>
        /// 获取分页查询集合,返回DataSet
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns></returns>
        public DataSet QueryPageToJson(Search search, out int total)
        {
            return CurrentRepository.QueryPageToJson(search, out total);
        }

        /// <summary>
        /// 获取分页查询集合,返回json
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns></returns>
        public string QueryPageToJson(Search search)
        {
            return CurrentRepository.QueryPageToJson(search);
        }

        /// <summary>
        /// 获取该表的所有实体对象
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAll(string companyId)
        {
            return CurrentRepository.GetAll(companyId);
        }

        /// <summary>
        /// 获取该表的所有实体对象
        /// </summary>
        /// <returns></returns>
        public string GetCount(string field, string sqlwhere)
        {
            return CurrentRepository.GetCount(field, sqlwhere);
        }
    }
}
