using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTcms.Common
{
    /// <summary>
    /// 查询类
    /// </summary>
    public class Search
    {
        public List<string> conditions = new List<string>();

        private int _pageSize = 1;


        private int _sEcho = 0;
        /// <summary>
        /// 必须传
        /// </summary>
        public int sEcho { get { return _sEcho; } set { _sEcho = value; } }

        /// <summary>
        /// 查询每页显示记录条数
        /// </summary>
        public int PageSize { set { _pageSize = value; } get { return _pageSize; } }

        private int _currentPageIndex = 1;
        /// <summary>
        /// 当前页面索引
        /// </summary>
        public int CurrentPageIndex { set { _currentPageIndex = value; } get { return _currentPageIndex; } }

        private string _selectedColums = "*";
        /// <summary>
        /// 选择的列
        /// </summary>
        public string SelectedColums { set { _selectedColums = value; } get { return _selectedColums; } }

        private string _keyFiled = string.Empty;
        /// <summary>
        /// 关键字
        /// </summary>
        public string KeyFiled { set { _keyFiled = value; } get { return _keyFiled; } }

        private string _tableName = string.Empty;
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { set { _tableName = value; } get { return _tableName; } }

        private string _sortField = "";
        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortField { set { _sortField = value; } get { return _sortField; } }

        /// <summary>
        /// 搜索关键字
        /// </summary>
        private string _searchKeyWord = string.Empty;
        public string SearchKeyWord
        {
            get { return _searchKeyWord; }
            set { _searchKeyWord = value; }
        }

        //private string _statusDefaultCondition = "STATUS!=-1";
        private string _statusDefaultCondition = "";
        /// <summary>
        /// 状态默认查询条件：STATUS!=-1
        /// </summary>
        public string StatusDefaultCondition
        {
            set { _statusDefaultCondition = value; }
            get { return _statusDefaultCondition; }
        }


        public void AddCondition(string conditionField, string condition, string conditionValue)
        {
            if (condition != "in")
            {
                conditionValue = conditionValue.Replace("'", "''");
            }
            switch (condition)
            {
                case "like":
                    conditions.Add(string.Format("{0} like '%{1}%'", conditionField, conditionValue));
                    break;
                case "=":
                    conditions.Add(string.Format("{0} = '{1}'", conditionField, conditionValue));
                    break;
                case "!=":
                    conditions.Add(string.Format("{0} != '{1}'", conditionField, conditionValue));
                    break;
                case "<>":
                    conditions.Add(string.Format("{0} <> '{1}'", conditionField, conditionValue));
                    break;
                case ">":
                    conditions.Add(string.Format("{0} > '{1}'", conditionField, conditionValue));
                    break;
                case ">=":
                    conditions.Add(string.Format("{0} >= '{1}'", conditionField, conditionValue));
                    break;
                case "<":
                    conditions.Add(string.Format("{0} < '{1}'", conditionField, conditionValue));
                    break;
                case "<=":
                    conditions.Add(string.Format("{0} <= '{1}'", conditionField, conditionValue));
                    break;
                case "in":
                    conditions.Add(string.Format("{0} in ({1})", conditionField, conditionValue));
                    break;
                case "":
                    conditions.Add(conditionField);
                    break;
            }
        }

        public void AddCondition(string condition)
        {
            conditions.Add(condition);
        }

        public string GetConditon()
        {
            StringBuilder sb = new StringBuilder();
            int index = 1;
            foreach (var item in conditions)
            {
                sb.Append((index++) == 1 ? item : string.Format(" AND {0} ", item));
            }

            if (sb.Length > 0)
            {
                if (_statusDefaultCondition.Length > 0) sb.Append(string.Format(" AND {0} ", _statusDefaultCondition));
            }
            else
            {
                if (_statusDefaultCondition.Length > 0) sb.Append(string.Format(" {0} ", _statusDefaultCondition));
            }
            return sb.ToString();
        }


        public void AddConditionByDapper(string conditionField, string condition, string conditionValue)
        {
            string strWhere = string.Empty;
            conditionValue = conditionValue.Replace("'", "''");
            switch (condition)
            {
                case "like":
                    strWhere = string.Format("{0} like ", conditionField);
                    break;
                case "=":
                    strWhere = string.Format("{0} =", conditionField);
                    break;
                case "<>":
                    strWhere = string.Format("{0} <>", conditionField);
                    break;
                case ">":
                    strWhere = string.Format("{0} >", conditionField);
                    break;
                case ">=":
                    strWhere = string.Format("{0} >=", conditionField);
                    break;
                case "<":
                    strWhere = string.Format("{0} <", conditionField);
                    break;
                case "<=":
                    strWhere = string.Format("{0} <=", conditionField);
                    break;

            }
            if (!string.IsNullOrEmpty(strWhere))
            {
                ConditionsByDappers.Add(new conditionsByDapper() { conditionField = strWhere, condition = condition, conditionValue = conditionValue });
            }

        }

        public List<conditionsByDapper> ConditionsByDappers = new List<conditionsByDapper>();

        public class conditionsByDapper
        {
            public string conditionField { get; set; }
            public string condition { get; set; }
            public string conditionValue { get; set; }
        }

        //public object[] args;
        public string GetConditonByDapper(out object[] args)
        {
            StringBuilder sb = new StringBuilder();

            args = new object[ConditionsByDappers.Count];
            foreach (var item in ConditionsByDappers)
            {

                if (ConditionsByDappers.IndexOf(item) == 0)
                {
                    sb.Append(string.Format("{0} @{1} ", item.conditionField, ConditionsByDappers.IndexOf(item)));
                }
                else
                {
                    sb.Append(string.Format(" AND {0}@{1} ", item.conditionField, ConditionsByDappers.IndexOf(item)));
                }

                if (item.condition == "like")
                {
                    args[ConditionsByDappers.IndexOf(item)] = string.Format("%{0}%", item.conditionValue);
                }
                else
                {
                    args[ConditionsByDappers.IndexOf(item)] = item.conditionValue;
                }

            }

            if (sb.Length > 0)
            {
                if (_statusDefaultCondition.Length > 0) sb.Append(string.Format(" AND {0} ", _statusDefaultCondition));
            }
            else
            {
                if (_statusDefaultCondition.Length > 0) sb.Append(string.Format(" {0} ", _statusDefaultCondition));
            }
            return sb.ToString();
        }

        public string SqlString
        {
            //get
            //{
            //    var sqlString = "SELECT {0} FROM {1} WHERE {2}";
            //    sqlString = string.Format(sqlString, _selectedColums, _tableName, GetConditon());
            //    return sqlString;
            //}
            get;
            set;
        }

        public void RemoveAt(int Index)
        {
            if (conditions != null && conditions.Count > 0 && Index < conditions.Count)
            {
                conditions.RemoveAt(Index);
            }
        }
    }
}
