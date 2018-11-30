using DTcms.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DTcms.Web.Mvc.UI.Controllers
{
     /// <summary>
     /// 搜索帮助类
     /// </summary>
    public partial class SearchBuyController
    {
        #region 基本增删改方法====================================
        /// <summary>
        /// 获得搜索列表
        /// </summary>
        public static List<Model.search_keys> GetList(int channel_id)
        {
            List<Model.search_keys> ls = GetCart(); //获取搜索商品
            return ToList(ls, channel_id);
        }

        /// <summary>
        /// 添加到搜索
        /// </summary>
        public static bool Add(int channel_id, string keyword)
        {
            List<Model.search_keys> ls = GetCart();
            if (ls != null)
            {
                Model.search_keys modelt = ls.Find(p => p.channel_id == channel_id && p.keyword == keyword);
                if (modelt != null)
                {
                    int i = ls.FindIndex(p => p.channel_id == channel_id && p.keyword == keyword);
                    ls[i] = modelt;
                    string jsonStr = JsonHelper.ObjectToJSON(ls); //转换为JSON字符串
                    AddCookies(jsonStr); //重新加入Cookies
                    return true;
                }
            }
            else
            {
                ls = new List<Model.search_keys>();
            }
            //不存在的则新增
            ls.Add(new Model.search_keys() { channel_id = channel_id, keyword = keyword });
            AddCookies(JsonHelper.ObjectToJSON(ls)); //添加至Cookies
            return true;
        }

        /// <summary>
        /// 清空搜索
        /// </summary>
        public static void Clear()
        {
            Utils.WriteCookie(DTKeys.COOKIE_SEARCH_BUY, "", -43200);
        }

        /// <summary>
        /// 移除搜索指定项
        /// </summary>
        public static void Clear(int channel_id, string keyword)
        {
            if (channel_id > 0 && keyword != "")
            {
                List<Model.search_keys> cartList = GetCart();
                if (cartList == null)
                {
                    return;
                }
                Model.search_keys modelt = cartList.Find(p => p.channel_id == channel_id && p.keyword == keyword);
                if (modelt != null)
                {
                    cartList.Remove(modelt); //移除指定的项
                    string jsonStr = JsonHelper.ObjectToJSON(cartList);
                    AddCookies(jsonStr);
                }
            }
        }

        /// <summary>
        /// 移除搜索指定项
        /// </summary>
        public static void Clear(List<Model.search_keys> ls)
        {
            if (ls != null)
            {
                List<Model.search_keys> cartList = GetCart();
                if (cartList == null)
                {
                    return;
                }
                foreach (Model.search_keys modelt in ls)
                {
                    Model.search_keys model = cartList.Find(p => p.channel_id == modelt.channel_id && p.keyword == modelt.keyword);
                    if (model != null)
                    {
                        cartList.Remove(model);
                    }
                }
                string jsonStr = JsonHelper.ObjectToJSON(cartList);
                AddCookies(jsonStr);
            }
        }
        #endregion

        #region 扩展方法==========================================
        /// <summary>
        /// 转换成List
        /// </summary>
        public static List<Model.search_keys> ToList(List<Model.search_keys> ls, int channel_id)
        {
            if (ls != null)
            {
                List<Model.search_keys> iList = new List<Model.search_keys>();
                ls = ls.FindAll(p => p.channel_id == channel_id);
                foreach (Model.search_keys item in ls)
                {
                    //开始赋值
                    Model.search_keys modelt = new Model.search_keys();
                    modelt.channel_id = item.channel_id;
                    modelt.keyword = item.keyword;
                    //添加入列表
                    iList.Add(modelt);
                }
                return iList;
            }
            return null;
        }
        #endregion

        #region 私有方法==========================================
        /// <summary>
        /// 获取cookies值
        /// </summary>
        private static List<Model.search_keys> GetCart()
        {
            List<Model.search_keys> ls = new List<Model.search_keys>();
            string jsonStr = GetCookies(); //获取Cookies值
            if (!string.IsNullOrEmpty(jsonStr))
            {
                ls = (List<Model.search_keys>)JsonHelper.JSONToObject<List<Model.search_keys>>(jsonStr);
                return ls;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 添加对象到cookies
        /// </summary>
        /// <param name="strValue"></param>
        private static void AddCookies(string strValue)
        {
            Utils.WriteCookie(DTKeys.COOKIE_SEARCH_BUY, strValue, 43200); //存储一个月
        }

        /// <summary>
        /// 获取cookies
        /// </summary>
        /// <returns></returns>
        private static string GetCookies()
        {
            return Utils.GetCookie(DTKeys.COOKIE_SEARCH_BUY);
        }
        #endregion
    }
}