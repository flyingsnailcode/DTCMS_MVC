using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using Quartz;
using Quartz.Impl;
using DTcms.Common;

namespace DTcms.Jobs
{
    /// <summary>
    /// 定时任务操作类
    /// </summary>
    public class JobsHelper
    {
        /// <summary>
        /// 任务详情
        /// </summary>
        private class jobinfo
        {
            /// <summary>
            /// 任务名称
            /// </summary>
            public string name { set; get; }
            /// <summary>
            /// 是否启用
            /// </summary>
            public bool enabled { set; get; }
            /// <summary>
            /// 任务操作类
            /// </summary>
            public string type { set; get; }
            /// <summary>
            /// cron表达式
            /// </summary>
            public string CronExpression { set; get; }
        }

        /// <summary>
        /// 配置文件路径
        /// </summary>
        private static string ConfigFile = "";
        /// <summary>
        /// 定义调度
        /// </summary>
        private static IScheduler sched = null;

        /// <summary>
        /// 启动服务
        /// </summary>
        public static void start(out int recordCount)
        {
            JobsHelper.ConfigFile = Utils.GetXmlMapPath(DTKeys.FILE_JOBS_XML_CONFING);
            List<JobsHelper.jobinfo> list = new List<JobsHelper.jobinfo>();
            try
            {
                if (JobsHelper.sched != null)
                {
                    JobsHelper.stop();
                    JobsHelper.sched = null;
                }
                JobsHelper.sched = new StdSchedulerFactory().GetScheduler();
                XmlDocument document = new XmlDocument();
                document.Load(JobsHelper.ConfigFile);
                XmlNode node = document.SelectSingleNode("Jobs");
                if (node.ChildNodes.Count > 0)
                {
                    foreach (XmlNode node2 in node.ChildNodes)
                    {
                        JobsHelper.jobinfo item = new JobsHelper.jobinfo
                        {
                            name = node2.Attributes["name"].Value,
                            type = node2.Attributes["type"].Value,
                            CronExpression = node2.Attributes["CronExpression"].Value,
                            enabled = bool.Parse(node2.Attributes["enabled"].Value)
                        };
                        if (item.enabled)
                        {
                            list.Add(item);
                            IJobDetail jobDetail = JobBuilder.Create(Type.GetType(item.type)).WithIdentity(item.name, item.name + "Group").Build();
                            ITrigger trigger = TriggerBuilder.Create().WithIdentity(item.name, item.name + "Group").WithCronSchedule(item.CronExpression).Build();
                            JobsHelper.sched.ScheduleJob(jobDetail, trigger);
                        }
                    }
                    if (list.Count > 0)
                    {
                        JobsHelper.sched.Start();
                        recordCount = list.Count;
                        LogHelper.Error(string.Format("成功启动{0}条任务！", list.Count.ToString()));
                    }
                    else
                    {
                        recordCount = 0;
                        LogHelper.Error("暂未有计划任务开启");
                    }
                }
                else
                {
                    recordCount = 0;
                    LogHelper.Error("暂未有计划任务开启");
                }
            }
            catch (Exception ex)
            {
                recordCount = 0;
                LogHelper.Error(JsonHelper.ObjectToJSON(list));
                LogHelper.Error("启动计划任务失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 停止任务
        /// </summary>
        public static bool stop()
        {
            bool IsTop = false;
            try
            {
                if (JobsHelper.sched != null)
                {
                    JobsHelper.sched.Shutdown(false);
                    JobsHelper.sched.Clear();
                }
                IsTop = true;
            }
            catch (Exception ex)
            {
                LogHelper.Error("关闭计划任务失败：" + ex.Message);
            }
            return IsTop;
        }
    }
}
