using System;
using DTcms.Cache.Factory;

namespace DTcms.Common
{
    public class PageCache
    {
        /// <summary>
        /// 读取缓存内容
        /// </summary>
        /// <param name="prefix">缓存名称</param>
        /// <returns></returns>
        public static string GetCache(string prefix)
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                return CacheFactory.Cache().GetCache<string>(prefix);
            }
            return null;
        }

        /// <summary>
        /// 写入缓存文件文件
        /// </summary>
        /// <param name="prefix">缓存名称</param>
        public static void WriteCache(string content, string prefix, int time)
        {
            if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(prefix))
            {
                return;
            }
            if (time == 0)
            {
                time = 10;
            }
            CacheFactory.Cache().WriteCache<string>(content, prefix, time);
        }
    }
}
