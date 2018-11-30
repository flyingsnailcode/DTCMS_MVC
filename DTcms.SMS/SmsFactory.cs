using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTcms.SMS
{
    public class SmsFactory
    {
        public static ISms Sms()
        {
            Model.siteconfig sysConfig = new BLL.siteconfig().loadConfig();
            switch (sysConfig.smsplatform)
            {
                case "ums86":
                    return new Ums.Sms(sysConfig.smsusername, sysConfig.smspassword, sysConfig.smsapicode, sysConfig.smssendcount, sysConfig.smsipcount, sysConfig.smssafecount);
                case "aliyun":
                    return new Ums.Sms(sysConfig.smsusername, sysConfig.smspassword, sysConfig.smsapicode, sysConfig.smssendcount, sysConfig.smsipcount, sysConfig.smssafecount);
                default:
                    return new Sms(sysConfig.smsusername, sysConfig.smspassword, sysConfig.smssendcount, sysConfig.smsipcount, sysConfig.smssafecount);
            }
        }
    }
}
