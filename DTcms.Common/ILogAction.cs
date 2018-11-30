using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTcms.Common
{
    public interface ILogAction
    {
        void Debug(object message);
        void Debug(Exception exception, object message);
        void DebugFormat(string format, params object[] args);
        void Info(object message);
        void Info(Exception exception, object message);
        void InfoFormat(string format, params object[] args);
        void Warn(object message);
        void Warn(Exception exception, object message);
        void WarnFormat(string format, params object[] args);
        void Error(object message);
        void Error(Exception exception, object message);
        void ErrorFormat(string format, params object[] args);
        void Fatal(object message);
        void Fatal(Exception exception, object message);
        void FatalFormat(string format, params object[] args);
    }
}
