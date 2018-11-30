using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageMagick;

namespace DTcms.Common
{
    public class MagickHelper
    {
        public MagickHelper() { }

        /// <summary>
        /// 文件字节数组
        /// </summary>
        public byte[] byteData { set; get; }
        /// <summary>
        /// 文件扩展名
        /// </summary>
        public int quality { set; get; }
        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string fileExt { set; get; }
        ///// <summary>
        ///// 是否添加水印 0不添加，1文字，2图片
        ///// </summary>
        //public int isWatermark { set; get; }
        ///// <summary>
        ///// 文字字号
        ///// </summary>
        //public int fontSize { set; get; }
        ///// <summary>
        ///// 字体
        ///// </summary>
        //public int fontType { set; get; }
        ///// <summary>
        ///// 水印文字
        ///// </summary>
        //public int words { set; get; }
        ///// <summary>
        ///// 水印位置
        ///// </summary>
        //public int position { set; get; }
        ///// <summary>
        ///// 图片水印
        ///// </summary>
        //public string imgSrc { set; get; }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        public byte[] Compress()
        {
            using (MagickImage image = new MagickImage(byteData))
            {
                //类型
                if (!string.IsNullOrEmpty(fileExt))
                {
                    switch (fileExt.ToLower())
                    {
                        case "png":
                            image.Format = MagickFormat.Png;
                            break;
                        case "gif":
                            image.Format = MagickFormat.Gif;
                            break;
                        case "webp":
                            image.Format = MagickFormat.WebP;
                            break;
                        default:
                            image.Format = MagickFormat.Jpg;
                            break;
                    }
                }
                //图片质量
                if (quality > 0)
                {
                    image.Quality = quality;
                }
                return image.ToByteArray();
            }
        }
    }
}