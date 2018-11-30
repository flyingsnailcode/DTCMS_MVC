using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTcms.Model
{
    /// <summary>
    /// app类型
    /// </summary>
    public enum AppTypeEnum
    {
        开卡成功通知 = 1001,
        下单成功通知 = 1002,
        发货通知 = 1003,
        新订单通知 = 1004,
        待处理通知 = 1005,
        会员充值通知 = 1006
    }

    /// <summary>
    /// 消息模板
    /// </summary>
    public enum TemplateTypeEnum
    {
        开卡成功通知 = 8,
        下单成功通知 = 4,
        发货通知 = 9,
        新订单通知 = 7,
        待处理通知 = 5,
        会员充值通知 = 11
    }

    /// <summary>
    /// Ajax请求返回的Json状态
    /// </summary>
    public enum JsonEnum
    {
        提交成功 = 2,
        支付成功 = 3,
        成功 = 1,
        重复 = -2,
        失败 = 0,
        异常 = -1,
        超时 = -3,
    }

    /// <summary>
    /// Admin-Ajax请求返回的Json状态
    /// </summary>
    public enum StatusEnum
    {
        [EnumDescription("星期一")]
        Monday,
        [EnumDescription("星期二")]
        Tuesday
    }

    #region 方法
    /// <summary>
    /// 自定义属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class EnumDescriptionAttribute : Attribute
    {
        private string description;
        public string Description { get { return description; } }

        public EnumDescriptionAttribute(string description) : base()
        {
            this.description = description;
        }
    }

    /// <summary>
    /// 获取枚举字符串
    /// </summary>
    public static class EnumHelper
    {
        public static string GetDescription(Enum value)
        {
            if (value == null)
            {
                throw new ArgumentException("value");
            }
            string description = value.ToString();
            var fieldInfo = value.GetType().GetField(description);
            var attributes =(EnumDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                description = attributes[0].Description;
            }
            return description;
        }
    }
    #endregion
}
