using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web;
using System.Runtime.Remoting.Messaging;
using Dapper;

namespace DTcms.DBUtility
{
  public  class DapperManager
    {
        #region 单列模式 线程安全

        public static DapperManager Instance
        {
            get
            {
                return Nested.DapperManager;
            }
        }

        private DapperManager()
        {

        }

        private class Nested
        {
            static Nested()
            {

            }
            internal static readonly DapperManager DapperManager =
                new DapperManager();
        }
        #endregion

        
        private static object obj = new object();

        private const string SESSION_KEY = "CONTEXT_SESSIONS";

        public Database GetCurrentDataBase(string connectionStringName)
        {
            Database db = (Database)ContextSessions[connectionStringName];
            if (db == null)
            {
                lock (obj)
                {
                    db = new Database(connectionStringName);
                }
                ContextSessions[connectionStringName] = db;
            }
            return db;
        }


        private Hashtable ContextSessions
        {
            get
            {
                if (IsInWebContext())
                {
                    if (HttpContext.Current.Items[SESSION_KEY] == null)
                        HttpContext.Current.Items[SESSION_KEY] = new Hashtable();

                    return (Hashtable)HttpContext.Current.Items[SESSION_KEY];
                }
                else
                {
                    if (CallContext.GetData(SESSION_KEY) == null)
                        CallContext.SetData(SESSION_KEY, new Hashtable());

                    return (Hashtable)CallContext.GetData(SESSION_KEY);
                }
            }
        }


        private bool IsInWebContext()
        {
            return HttpContext.Current != null;
        }

    }
}
