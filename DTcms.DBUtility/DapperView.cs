using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Dapper;
using System.Data.SqlClient;
using DTcms.Common;

namespace DTcms.DBUtility
{
    public class DapperView
    {
        public static string connectionString = null;
        static DapperView()
        {
            string en = ConfigurationManager.AppSettings["Encrypt"].ToString();
            connectionString = ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["WriteConnectionStringName"]].ConnectionString;
            if (string.Equals(en, "true"))
            {
                connectionString = DESEncrypt.Decrypt(connectionString);
            }
        }
        private static DapperView _instance;
        public static DapperView GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DapperView();
            }
            return _instance;
        }

        public enum DatabaseType
        {
            SqlServer,
            MySql,
            Oracle,
            DB2
        }

        #region Context
        public SqlConnection Context()
        {
            string connectionString = ConfigurationManager.ConnectionStrings[WriteConnectionString].ToString();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
        #endregion
        
        #region MSSQL
        private string ReadConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["ReadConnectionStringName"];
            }
        }

        private string WriteConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["WriteConnectionStringName"];
            }
        }

        private string MysqlConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["MysqlConnectionStringName"];
            }
        }

        /// <summary>
        /// 写数据库(主)
        /// </summary>
        public Database WriteDataBase
        {
            get
            {
                DatabaseType dbType = DatabaseType.SqlServer;
                string ConnectionStringName = string.Empty;
                switch (dbType)
                {
                    case DatabaseType.SqlServer:
                        ConnectionStringName = WriteConnectionString;
                        break;
                    case DatabaseType.MySql:
                        ConnectionStringName = MysqlConnectionString;
                        break;
                    case DatabaseType.Oracle:
                        ConnectionStringName = WriteConnectionString;
                        break;
                    case DatabaseType.DB2:
                        ConnectionStringName = WriteConnectionString;
                        break;
                }

                return DapperManager.Instance.GetCurrentDataBase(ConnectionStringName);
            }
        }

        /// <summary>
        /// 读数据库(从)
        /// </summary>
        public Database ReadDataBase
        {
            get
            {
                return WriteDataBase;
                //MSSQLSERVER订阅发布可撤销注释代码
                //return DapperManager.Instance.GetCurrentDataBase(ReadConnectionString);
            }
        }
        #endregion
    }
}

