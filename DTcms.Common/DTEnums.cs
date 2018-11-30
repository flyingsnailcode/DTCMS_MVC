using System;
using System.Collections.Generic;
using System.Text;

namespace DTcms.Common
{
    public class DTEnums
    {
        /// <summary>
        /// 统一管理操作枚举
        /// </summary>
        public enum ActionEnum
        {
            /// <summary>
            /// 所有
            /// </summary>
            All,
            /// <summary>
            /// 显示
            /// </summary>
            Show,
            /// <summary>
            /// 查看
            /// </summary>
            View,
            /// <summary>
            /// 添加
            /// </summary>
            Add,
            /// <summary>
            /// 修改
            /// </summary>
            Edit,
            /// <summary>
            /// 删除
            /// </summary>
            Delete,
            /// <summary>
            /// 清空
            /// </summary>
            DelAll,
            /// <summary>
            /// 审核
            /// </summary>
            Audit,
            /// <summary>
            /// 回复
            /// </summary>
            Reply,
            /// <summary>
            /// 确认
            /// </summary>
            Confirm,
            /// <summary>
            /// 取消
            /// </summary>
            Cancel,
            /// <summary>
            /// 作废
            /// </summary>
            Invalid,
            /// <summary>
            /// 生成
            /// </summary>
            Build,
            /// <summary>
            /// 安装
            /// </summary>
            Instal,
            /// <summary>
            /// 卸载
            /// </summary>
            UnLoad,
            /// <summary>
            /// 登录
            /// </summary>
            Login,
            /// <summary>
            /// 备份
            /// </summary>
            Back,
            /// <summary>
            /// 还原
            /// </summary>
            Restore,
            /// <summary>
            /// 替换
            /// </summary>
            Replace,
            /// <summary>
            /// 复制
            /// </summary>
            Copy
        }

        /// <summary>
        /// 系统导航菜单类别枚举
        /// </summary>
        public enum NavigationEnum
        {
            /// <summary>
            /// 系统后台菜单
            /// </summary>
            System,
            /// <summary>
            /// 会员中心导航
            /// </summary>
            Users,
            /// <summary>
            /// 网站主导航
            /// </summary>
            WebSite
        }

        /// <summary>
        /// 用户生成码枚举
        /// </summary>
        public enum CodeEnum
        {
            /// <summary>
            /// 注册验证
            /// </summary>
            RegVerify,
            /// <summary>
            /// 邀请注册
            /// </summary>
            Register,
            /// <summary>
            /// 取回密码
            /// </summary>
            Password
        }

        /// <summary>
        /// 金额类型枚举
        /// </summary>
        public enum AmountTypeEnum
        {
            /// <summary>
            /// 系统赠送
            /// </summary>
            SysGive,
            /// <summary>
            /// 在线充值
            /// </summary>
            Recharge,
            /// <summary>
            /// 用户消费
            /// </summary>
            Consumption,
            /// <summary>
            /// 购买商品
            /// </summary>
            BuyGoods,
            /// <summary>
            /// 积分兑换
            /// </summary>
            Convert
        }

        /// <summary>
        /// 编辑器状态
        /// </summary>
        public enum ResultState
        {
            /// <summary>
            /// 成功
            /// </summary>
            Success,
            /// <summary>
            /// 参数不正确
            /// </summary>
            InvalidParam,
            /// <summary>
            /// 路径不存在
            /// </summary>
            AuthorizError,
            /// <summary>
            /// 文件系统权限不足
            /// </summary>
            IOError,
            /// <summary>
            /// 文件系统读取错误
            /// </summary>
            PathNotFound,
            /// <summary>
            /// 文件访问出错，请检查写入权限
            /// </summary>
            FileAccessError,
            /// <summary>
            /// 文件大小超出服务器限制
            /// </summary>
            SizeLimitExceed,
            /// <summary>
            /// 不允许的文件格式
            /// </summary>
            TypeNotAllow,
            /// <summary>
            /// 网络错误
            /// </summary>
            NetworkError
        }

        /// <summary>
        /// 用户生成码枚举
        /// </summary>
        public enum BaiduSitemap
        {
            /// <summary>
            /// 推送数据
            /// </summary>
            urls,
            /// <summary>
            /// 更新数据
            /// </summary>
            update,
            /// <summary>
            /// 删除数据
            /// </summary>
            del
        }
    }
}
