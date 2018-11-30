using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.IO;
using System.Drawing;
using System.Net;
using System.Configuration;
using DTcms.Common;
using System.Web.Mvc;

namespace DTcms.Web.Mvc.UI
{
    public class UpLoad: Controller
    {
        public Model.siteconfig siteConfig;

        public UpLoad()
        {
            siteConfig = new BLL.siteconfig().loadConfig();
        }

        /// <summary>
        /// 通过文件流上传文件方法
        /// </summary>
        /// <param name="upType">文件上传类型</param>
        /// <param name="byteData">文件字节数组</param>
        /// <param name="fileName">文件名</param>
        /// <param name="isThumbnail">是否生成缩略图</param>
        /// <param name="isWater">是否打水印</param>
        /// <param name="isCompress">是否启用图片压缩</param>
        /// <param name="isCover">是否为封面</param>
        /// <param name="thumbnailWidth">缩略图宽度</param>
        /// <param name="thumbnailHeight">缩略图高度</param>
        /// <param name="thumbnailMode">缩略图生成方式</param>
        /// <returns>上传成功返回JSON字符串</returns>
        public JsonResult FileSaveAs(string upType, byte[] byteData, string fileName, bool isThumbnail, bool isWater, bool isCompress, bool isCover, int thumbnailWidth, int thumbnailHeight, string thumbnailMode)
        {
            try
            {
                int maxSize = 0;
                string[] allowExtensions = null;
                //上传类型
                switch (upType.ToLower())
                {
                    case "file":
                        maxSize = siteConfig.attachsize;
                        allowExtensions = siteConfig.attachextension.Split(',');
                        break;
                    case "video":
                        //视频上传
                        maxSize = siteConfig.videosize;
                        allowExtensions = siteConfig.videoextension.Split(',');
                        break;
                    default:
                        maxSize = siteConfig.imgsize;
                        allowExtensions = siteConfig.imgextension.Split(',');
                        break;
                }
                string fileExt = Path.GetExtension(fileName).Trim('.').ToLower(); //文件扩展名，不含“.”
                string newFileName = Utils.GetRamCode() + "." + fileExt; //随机生成新的文件名
                string newThumbnailFileName = "thumb_" + newFileName; //随机生成缩略图文件名
                string upLoadPath = GetUpLoadPath(); //本地上传目录相对路径
                string fullUpLoadPath = Utils.GetMapPath(upLoadPath); //本地上传目录的物理路径
                string newFilePath = upLoadPath + newFileName; //本地上传后的路径
                string newThumbnailPath = upLoadPath + newThumbnailFileName; //本地上传后的缩略图路径

                byte[] thumbData = null; //缩略图文件流

                //检查文件字节数组是否为NULL
                if (byteData == null)
                {
                    return Json(new { status = 0, msg = "请选择要上传的文件！" });
                }
                //检查文件扩展名是否合法
                if (!CheckFileExt(fileExt, allowExtensions))
                {
                    return Json(new { status = 0, msg = "不允许上传" + fileExt + "类型的文件！" });
                }
                //检查文件大小是否合法
                if (!CheckFileSize(byteData.Length, maxSize))
                {
                    return Json(new { status = 0, msg = "文件超过限制的大小！" });
                }

                //判断是否为图片存储
                if (upType.ToLower() == "img")
                {
                    //是否是生成封面
                    if (isCover)
                    {
                        //封面直接裁剪
                        if (thumbnailWidth > 0 && thumbnailHeight > 0)
                        {
                            byteData = Thumbnail.MakeThumbnailImage(byteData, fileExt, thumbnailWidth, thumbnailHeight, thumbnailMode);
                        }
                    }
                    else
                    {
                        //如果是图片，检查图片是否超出最大尺寸，是则裁剪
                        if (isCompress && this.siteConfig.imgmaxheight > 0 || this.siteConfig.imgmaxwidth > 0)
                        {
                            byteData = Thumbnail.MakeThumbnailImage(byteData, fileExt, this.siteConfig.imgmaxwidth, this.siteConfig.imgmaxheight);
                        }
                    }
                    //如果是图片，检查是否需要生成缩略图，是则生成
                    if (isThumbnail && thumbnailWidth > 0 && thumbnailHeight > 0)
                    {
                        thumbData = Thumbnail.MakeThumbnailImage(byteData, fileExt, thumbnailWidth, thumbnailHeight, thumbnailMode);
                    }
                    //else
                    //{
                    //    newThumbnailPath = newFilePath; //不生成缩略图则返回原图
                    //}
                    //如果是图片，检查是否需要打水印
                    if (isWater && siteConfig.watermarktype > 0 && IsWaterMark(fileExt))
                    {
                        switch (this.siteConfig.watermarktype)
                        {
                            case 1:
                                if (!string.IsNullOrEmpty(this.siteConfig.watermarktext))
                                {
                                    byteData = WaterMark.AddImageSignText(byteData, fileExt, this.siteConfig.watermarktext, this.siteConfig.watermarkposition,
                                    this.siteConfig.watermarkimgquality, this.siteConfig.watermarkfont, this.siteConfig.watermarkfontsize);
                                }
                                break;
                            case 2:
                                if (!string.IsNullOrEmpty(this.siteConfig.watermarkpic))
                                {

                                    byteData = WaterMark.AddImageSignPic(byteData, fileExt, this.siteConfig.watermarkpic, this.siteConfig.watermarkposition,
                                        this.siteConfig.watermarkimgquality, this.siteConfig.watermarktransparency);
                                }
                                break;
                        }
                    }
                    //图片处理插件选择
                    if (siteConfig.imgplugin > 0)
                    {
                        //主图
                        MagickHelper mhr = new MagickHelper();
                        mhr.byteData = byteData;
                        mhr.quality = 0;
                        if (isCompress)
                        {
                            mhr.quality = siteConfig.watermarkimgquality;
                        }
                        //是否需要转换扩展名
                        if (!string.IsNullOrEmpty(siteConfig.imgconvert))
                        {
                            fileExt = siteConfig.imgconvert;
                            mhr.fileExt = siteConfig.imgconvert;
                            //替换扩展名
                            newFileName = newFileName.Remove(newFileName.LastIndexOf(".") + 1) + fileExt;
                            newThumbnailFileName = newThumbnailFileName.Remove(newThumbnailFileName.LastIndexOf(".") + 1) + fileExt;
                            newFilePath = newFilePath.Remove(newFilePath.LastIndexOf(".") + 1) + fileExt;
                            newThumbnailPath = newThumbnailPath.Remove(newThumbnailPath.LastIndexOf(".") + 1) + fileExt;
                        }
                        byteData = mhr.Compress();
                        //缩略图
                        if (thumbData != null)
                        {
                            MagickHelper mhr2 = new MagickHelper();
                            mhr2.byteData = thumbData;
                            mhr2.quality = mhr.quality < 90 ? 90 : mhr.quality;
                            mhr2.fileExt = mhr.fileExt;
                        }
                    }
                }
                else if (upType.ToLower() == "video")
                {

                }
                //分发不同的上传方式处理
                switch (siteConfig.fileserver)
                {
                    case "aliyun": //阿里云OSS对象存储
                        #region 阿里云OSS对象存储
                        //检查配置是否完善
                        if (string.IsNullOrEmpty(siteConfig.accessid) || string.IsNullOrEmpty(siteConfig.accesssecret) || string.IsNullOrEmpty(siteConfig.endpoint))
                        {
                            return Json(new { status = 0, msg = "文件上传配置未完善，无法上传！" });
                        }
                        //初始化阿里云配置
                        DTcms.Cloud.AliyunOss aliyun = new DTcms.Cloud.AliyunOss(siteConfig.endpoint, siteConfig.accessid, siteConfig.accesssecret);
                        string result = string.Empty; //返回信息

                        //保存主文件
                        if (!aliyun.PutObject(byteData, siteConfig.spacename, newFilePath, siteConfig.spaceurl, out result))
                        {
                            return Json(new { status = 0, msg = result });
                        }
                        newFilePath = result; //将地址赋值给新文件地址

                        //保存缩略图文件
                        if (thumbData != null)
                        {
                            aliyun.PutObject(thumbData, siteConfig.spacename, newThumbnailPath, siteConfig.spaceurl, out result);
                            newThumbnailPath = result; //将缩略图地址赋值
                        }
                        else
                        {
                            newThumbnailPath = newFilePath; //没有缩略图将原图返回
                        }
                        #endregion
                        break;
                    case "qcloud": //腾讯云COS对象存储
                        #region 腾讯云COS对象存储
                        #endregion
                        break;
                    case "domain": //本地跨域存储
                        #region 本地跨域存储
                        fullUpLoadPath = siteConfig.domainpath + upLoadPath.Replace("/", "\\");
                        var bindDomain = siteConfig.domainbind;
                        if (bindDomain.EndsWith("/"))
                        {
                            bindDomain = bindDomain.Remove(bindDomain.Length - 1);
                        }
                        newFilePath = bindDomain + newFilePath;
                        //检查本地上传的物理路径是否存在，不存在则创建
                        if (!Directory.Exists(fullUpLoadPath))
                        {
                            Directory.CreateDirectory(fullUpLoadPath);
                        }
                        //保存主文件
                        FileHelper.SaveFile(byteData, fullUpLoadPath + newFileName);
                        //保存缩略图文件
                        if (thumbData != null)
                        {
                            newThumbnailPath = bindDomain + newThumbnailPath;
                            FileHelper.SaveFile(thumbData, fullUpLoadPath + newThumbnailFileName);
                        }
                        else
                        {
                            newThumbnailPath = newFilePath;
                        }
                        #endregion
                        break;
                    default: //本地服务器
                        #region 本地服务器
                        //检查本地上传的物理路径是否存在，不存在则创建
                        if (!Directory.Exists(fullUpLoadPath))
                        {
                            Directory.CreateDirectory(fullUpLoadPath);
                        }
                        //保存主文件
                        FileHelper.SaveFile(byteData, fullUpLoadPath + newFileName);
                        //保存缩略图文件
                        if (thumbData != null)
                        {
                            FileHelper.SaveFile(thumbData, fullUpLoadPath + newThumbnailFileName);
                        }
                        else
                        {
                            newThumbnailPath = upLoadPath;
                        }
                        #endregion
                        break;
                }

                //处理完毕，返回JOSN格式的文件信息
                return Json(new { status = 1, msg = "上传文件成功！", name= fileName, path= newFilePath, thumb= newThumbnailPath, size= byteData.Length, ext= fileExt });
            }
            catch (Exception ex)
            {
                LogHelper.Info(ex.Message);
                return Json(new { status = 0, msg = "上传过程中发生意外错误！" });
            }
        }
        
        /// <summary>
        /// 裁剪图片并保存
        /// </summary>
        public JsonResult CropSaveAs(string fileUri, int maxWidth, int maxHeight, int cropWidth, int cropHeight, int X, int Y)
        {
            string fileExt = Path.GetExtension(fileUri).Trim('.'); //文件扩展名，不含“.”
            if (string.IsNullOrEmpty(fileExt) || !IsImage(fileExt))
            {
                return Json(new { status = 0, msg = "该文件不是图片！" });
            }

            byte[] byteData = null;
            //判断是否远程文件
            if (fileUri.ToLower().StartsWith("http://") || fileUri.ToLower().StartsWith("https://"))
            {
                WebClient client = new WebClient();
                byteData = client.DownloadData(fileUri);
                client.Dispose();
            }
            else //本地源文件
            {
                string fullName = Utils.GetMapPath(fileUri);
                if (System.IO.File.Exists(fullName))
                {
                    FileStream fs = System.IO.File.OpenRead(fullName);
                    BinaryReader br = new BinaryReader(fs);
                    br.BaseStream.Seek(0, SeekOrigin.Begin);
                    byteData = br.ReadBytes((int)br.BaseStream.Length);
                    fs.Close();
                }
            }
            //裁剪后得到文件流
            byteData = Thumbnail.MakeThumbnailImage(byteData, fileExt, maxWidth, maxHeight, cropWidth, cropHeight, X, Y);
            //删除原图
            DeleteFile(fileUri);
            //保存制作好的缩略图
            return FileSaveAs("img", byteData, fileUri, false, false, true, false, 0, 0, string.Empty);
        }

        /// <summary>
        /// 保存远程文件到本地
        /// </summary>
        /// <param name="sourceUri">URI地址</param>
        /// <returns>上传后的路径</returns>
        public JsonResult RemoteSaveAs(string sourceUri)
        {
            if (!IsExternalIPAddress(sourceUri))
            {
                return Json(new { status = 0, msg = "INVALID_URL！" });
            }
            var request = HttpWebRequest.Create(sourceUri) as HttpWebRequest;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return Json(new { status = 0, msg = "Url returns " + response.StatusCode + ", " + response.StatusDescription + "" });
                }
                if (response.ContentType.IndexOf("image") == -1)
                {
                    return Json(new { status = 0, msg = "Url is not an image" });
                }
                try
                {
                    byte[] byteData = FileHelper.ConvertStreamToByteBuffer(response.GetResponseStream());
                    return FileSaveAs("img", byteData, sourceUri, false, false, true, false, 0, 0, string.Empty);
                }
                catch (Exception e)
                {
                    return Json(new { status = 0, msg = "抓取错误："+ e.Message });
                }
            }
        }
        #region 上传
        /// <summary>
        /// 通过文件流上传文件方法
        /// </summary>
        /// <param name="upType">文件上传类型</param>
        /// <param name="byteData">文件字节数组</param>
        /// <param name="fileName">文件名</param>
        /// <param name="isThumbnail">是否生成缩略图</param>
        /// <param name="isWater">是否打水印</param>
        /// <param name="isCompress">是否启用图片压缩</param>
        /// <param name="isCover">是否为封面</param>
        /// <param name="thumbnailWidth">缩略图宽度</param>
        /// <param name="thumbnailHeight">缩略图高度</param>
        /// <param name="thumbnailMode">缩略图生成方式</param>
        /// <returns>上传成功返回JSON字符串</returns>
        public string StrFileSaveAs(string upType, byte[] byteData, string fileName, bool isThumbnail, bool isWater, bool isCompress, bool isCover, int thumbnailWidth, int thumbnailHeight, string thumbnailMode)
        {
            try
            {
                int maxSize = 0;
                string[] allowExtensions = null;
                //上传类型
                switch (upType.ToLower())
                {
                    case "file":
                        maxSize = siteConfig.attachsize;
                        allowExtensions = siteConfig.attachextension.Split(',');
                        break;
                    case "video":
                        //视频上传
                        maxSize = siteConfig.videosize;
                        allowExtensions = siteConfig.videoextension.Split(',');
                        break;
                    default:
                        maxSize = siteConfig.imgsize;
                        allowExtensions = siteConfig.imgextension.Split(',');
                        break;
                }
                string fileExt = Path.GetExtension(fileName).Trim('.').ToLower(); //文件扩展名，不含“.”
                string newFileName = Utils.GetRamCode() + "." + fileExt; //随机生成新的文件名
                string newThumbnailFileName = "thumb_" + newFileName; //随机生成缩略图文件名
                string upLoadPath = GetUpLoadPath(); //本地上传目录相对路径
                string fullUpLoadPath = Utils.GetMapPath(upLoadPath); //本地上传目录的物理路径
                string newFilePath = upLoadPath + newFileName; //本地上传后的路径
                string newThumbnailPath = upLoadPath + newThumbnailFileName; //本地上传后的缩略图路径

                byte[] thumbData = null; //缩略图文件流

                //检查文件字节数组是否为NULL
                if (byteData == null)
                {
                    return "{\"status\": 0, \"msg\": \"请选择要上传的文件！\"}";
                }
                //检查文件扩展名是否合法
                if (!CheckFileExt(fileExt, allowExtensions))
                {
                    return "{\"status\": 0, \"msg\": \"不允许上传" + fileExt + "类型的文件！\"}";
                }
                //检查文件大小是否合法
                if (!CheckFileSize(byteData.Length, maxSize))
                {
                    return "{\"status\": 0, \"msg\": \"文件超过限制的大小！\"}";
                }

                //判断是否为图片存储
                if (upType.ToLower() == "img")
                {
                    //是否是生成封面
                    if (isCover)
                    {
                        //封面直接裁剪
                        if (thumbnailWidth > 0 && thumbnailHeight > 0)
                        {
                            byteData = Thumbnail.MakeThumbnailImage(byteData, fileExt, thumbnailWidth, thumbnailHeight, thumbnailMode);
                        }
                    }
                    else
                    {
                        //如果是图片，检查图片是否超出最大尺寸，是则裁剪
                        if (isCompress && this.siteConfig.imgmaxheight > 0 || this.siteConfig.imgmaxwidth > 0)
                        {
                            byteData = Thumbnail.MakeThumbnailImage(byteData, fileExt, this.siteConfig.imgmaxwidth, this.siteConfig.imgmaxheight);
                        }
                    }
                    //如果是图片，检查是否需要生成缩略图，是则生成
                    if (isThumbnail && thumbnailWidth > 0 && thumbnailHeight > 0)
                    {
                        thumbData = Thumbnail.MakeThumbnailImage(byteData, fileExt, thumbnailWidth, thumbnailHeight, thumbnailMode);
                    }
                    //else
                    //{
                    //    newThumbnailPath = newFilePath; //不生成缩略图则返回原图
                    //}
                    //如果是图片，检查是否需要打水印
                    if (isWater && siteConfig.watermarktype > 0 && IsWaterMark(fileExt))
                    {
                        switch (this.siteConfig.watermarktype)
                        {
                            case 1:
                                if (!string.IsNullOrEmpty(this.siteConfig.watermarktext))
                                {
                                    byteData = WaterMark.AddImageSignText(byteData, fileExt, this.siteConfig.watermarktext, this.siteConfig.watermarkposition,
                                    this.siteConfig.watermarkimgquality, this.siteConfig.watermarkfont, this.siteConfig.watermarkfontsize);
                                }
                                break;
                            case 2:
                                if (!string.IsNullOrEmpty(this.siteConfig.watermarkpic))
                                {

                                    byteData = WaterMark.AddImageSignPic(byteData, fileExt, this.siteConfig.watermarkpic, this.siteConfig.watermarkposition,
                                        this.siteConfig.watermarkimgquality, this.siteConfig.watermarktransparency);
                                }
                                break;
                        }
                    }
                    //图片处理插件选择
                    if (siteConfig.imgplugin > 0)
                    {
                        //主图
                        MagickHelper mhr = new MagickHelper();
                        mhr.byteData = byteData;
                        mhr.quality = 0;
                        if (isCompress)
                        {
                            mhr.quality = siteConfig.watermarkimgquality;
                        }
                        //是否需要转换扩展名
                        if (!string.IsNullOrEmpty(siteConfig.imgconvert))
                        {
                            fileExt = siteConfig.imgconvert;
                            mhr.fileExt = siteConfig.imgconvert;
                            //替换扩展名
                            newFileName = newFileName.Remove(newFileName.LastIndexOf(".") + 1) + fileExt;
                            newThumbnailFileName = newThumbnailFileName.Remove(newThumbnailFileName.LastIndexOf(".") + 1) + fileExt;
                            newFilePath = newFilePath.Remove(newFilePath.LastIndexOf(".") + 1) + fileExt;
                            newThumbnailPath = newThumbnailPath.Remove(newThumbnailPath.LastIndexOf(".") + 1) + fileExt;
                        }
                        byteData = mhr.Compress();
                        //缩略图
                        if (thumbData != null)
                        {
                            MagickHelper mhr2 = new MagickHelper();
                            mhr2.byteData = thumbData;
                            mhr2.quality = mhr.quality < 90 ? 90 : mhr.quality;
                            mhr2.fileExt = mhr.fileExt;
                        }
                    }
                }
                else if (upType.ToLower() == "video")
                {

                }
                //分发不同的上传方式处理
                switch (siteConfig.fileserver)
                {
                    case "aliyun": //阿里云OSS对象存储
                        #region 阿里云OSS对象存储
                        //检查配置是否完善
                        if (string.IsNullOrEmpty(siteConfig.accessid) || string.IsNullOrEmpty(siteConfig.accesssecret) || string.IsNullOrEmpty(siteConfig.endpoint))
                        {
                            return "{\"status\": 0, \"msg\": \"文件上传配置未完善，无法上传\"}";
                        }
                        //初始化阿里云配置
                        DTcms.Cloud.AliyunOss aliyun = new DTcms.Cloud.AliyunOss(siteConfig.endpoint, siteConfig.accessid, siteConfig.accesssecret);
                        string result = string.Empty; //返回信息

                        //保存主文件
                        if (!aliyun.PutObject(byteData, siteConfig.spacename, newFilePath, siteConfig.spaceurl, out result))
                        {
                            return "{\"status\": 0, \"msg\": \"" + result + "\"}";
                        }
                        newFilePath = result; //将地址赋值给新文件地址

                        //保存缩略图文件
                        if (thumbData != null)
                        {
                            aliyun.PutObject(thumbData, siteConfig.spacename, newThumbnailPath, siteConfig.spaceurl, out result);
                            newThumbnailPath = result; //将缩略图地址赋值
                        }
                        else
                        {
                            newThumbnailPath = newFilePath; //没有缩略图将原图返回
                        }
                        #endregion
                        break;
                    case "qcloud": //腾讯云COS对象存储
                        #region 腾讯云COS对象存储
                        #endregion
                        break;
                    case "domain": //本地跨域存储
                        #region 本地跨域存储
                        fullUpLoadPath = siteConfig.domainpath + upLoadPath.Replace("/", "\\");
                        var bindDomain = siteConfig.domainbind;
                        if (bindDomain.EndsWith("/"))
                        {
                            bindDomain = bindDomain.Remove(bindDomain.Length - 1);
                        }
                        newFilePath = bindDomain + newFilePath;
                        //检查本地上传的物理路径是否存在，不存在则创建
                        if (!Directory.Exists(fullUpLoadPath))
                        {
                            Directory.CreateDirectory(fullUpLoadPath);
                        }
                        //保存主文件
                        FileHelper.SaveFile(byteData, fullUpLoadPath + newFileName);
                        //保存缩略图文件
                        if (thumbData != null)
                        {
                            newThumbnailPath = bindDomain + newThumbnailPath;
                            FileHelper.SaveFile(thumbData, fullUpLoadPath + newThumbnailFileName);
                        }
                        else
                        {
                            newThumbnailPath = newFilePath;
                        }
                        #endregion
                        break;
                    default: //本地服务器
                        #region 本地服务器
                        //检查本地上传的物理路径是否存在，不存在则创建
                        if (!Directory.Exists(fullUpLoadPath))
                        {
                            Directory.CreateDirectory(fullUpLoadPath);
                        }
                        //保存主文件
                        FileHelper.SaveFile(byteData, fullUpLoadPath + newFileName);
                        //保存缩略图文件
                        if (thumbData != null)
                        {
                            FileHelper.SaveFile(thumbData, fullUpLoadPath + newThumbnailFileName);
                        }
                        else
                        {
                            newThumbnailPath = upLoadPath;
                        }
                        #endregion
                        break;
                }

                //处理完毕，返回JOSN格式的文件信息
                return "{\"status\": 1, \"msg\": \"上传文件成功！\", \"name\": \""
                    + fileName + "\", \"path\": \"" + newFilePath + "\", \"thumb\": \""
                    + newThumbnailPath + "\", \"size\": " + byteData.Length + ", \"ext\": \"" + fileExt + "\"}";
            }
            catch (Exception ex)
            {
                LogHelper.Info(ex.Message);
                return "{\"status\": 0, \"msg\": \"上传过程中发生意外错误！\"}";
            }
        }

        /// <summary>
        /// 保存远程文件到本地
        /// </summary>
        /// <param name="sourceUri">URI地址</param>
        /// <returns>上传后的路径</returns>
        public string StrRemoteSaveAs(string sourceUri)
        {
            if (!IsExternalIPAddress(sourceUri))
            {
                return "{\"status\": 0, \"msg\": \"INVALID_URL\"}";
            }
            var request = HttpWebRequest.Create(sourceUri) as HttpWebRequest;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return "{\"status\": 0, \"msg\": \"Url returns " + response.StatusCode + ", " + response.StatusDescription + "\"}";
                }
                if (response.ContentType.IndexOf("image") == -1)
                {
                    return "{\"status\": 0, \"msg\": \"Url is not an image\"}";
                }
                try
                {
                    byte[] byteData = FileHelper.ConvertStreamToByteBuffer(response.GetResponseStream());
                    return StrFileSaveAs("img", byteData, sourceUri, false, false, true, false, 0, 0, string.Empty);
                }
                catch (Exception e)
                {
                    return "{\"status\": 0, \"msg\": \"抓取错误：" + e.Message + "\"}";
                }
            }
        }
        #endregion

        /// <summary>
        /// 删除上传文件
        /// </summary>
        /// <param name="fileUri">相对地址或网址</param>
        public void DeleteFile(string fileUri)
        {
            //分发不同的上传方式处理
            switch (siteConfig.fileserver)
            {
                case "aliyun": //阿里云OSS对象存储
                    //检查配置是否完善
                    if (string.IsNullOrEmpty(siteConfig.accessid) || string.IsNullOrEmpty(siteConfig.accesssecret) || string.IsNullOrEmpty(siteConfig.endpoint))
                    {
                        return;
                    }
                    //初始化配置
                    DTcms.Cloud.AliyunOss aliyun = new DTcms.Cloud.AliyunOss(siteConfig.endpoint, siteConfig.accessid, siteConfig.accesssecret);
                    string result = string.Empty; //返回信息
                    aliyun.DeleteObject(siteConfig.spacename, fileUri, siteConfig.spaceurl, out result);
                    break;
                case "qcloud": //腾讯云COS对象存储

                    break;
                default: //本地服务器
                    //文件不应是上传文件，防止跨目录删除
                    if (fileUri.IndexOf("..") == -1 && fileUri.ToLower().StartsWith(siteConfig.webpath.ToLower() + siteConfig.filepath.ToLower()))
                    {
                        FileHelper.DeleteUpFile(fileUri);
                    }
                    break;
            }
        }

        #region 私有方法
        /// <summary>
        /// 返回上传目录相对路径
        /// </summary>
        /// <param name="fileName">上传文件名</param>
        public string GetUpLoadPath()
        {
            string path = siteConfig.webpath + siteConfig.filepath + "/"; //站点目录+上传目录
            switch (this.siteConfig.filesave)
            {
                case 1: //按年月日每天一个文件夹
                    path += DateTime.Now.ToString("yyyyMMdd");
                    break;
                default: //按年月/日存入不同的文件夹
                    path += DateTime.Now.ToString("yyyyMM") + "/" + DateTime.Now.ToString("dd");
                    break;
            }
            return path + "/";
        }

        /// <summary>
        /// 是否需要打水印
        /// </summary>
        /// <param name="_fileExt">文件扩展名，不含“.”</param>
        public bool IsWaterMark(string _fileExt)
        {
            //判断是否开启水印
            if (this.siteConfig.watermarktype > 0)
            {
                //判断是否可以打水印的图片类型
                ArrayList al = new ArrayList();
                al.Add("bmp");
                al.Add("jpeg");
                al.Add("jpg");
                al.Add("png");
                if (al.Contains(_fileExt.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否为图片文件
        /// </summary>
        /// <param name="_fileExt">文件扩展名，不含“.”</param>
        public bool IsImage(string _fileExt)
        {
            ArrayList al = new ArrayList();
            al.Add("bmp");
            al.Add("jpeg");
            al.Add("jpg");
            al.Add("gif");
            al.Add("png");
            if (al.Contains(_fileExt.ToLower()))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查是否为合法的上传文件
        /// </summary>
        /// <param name="fileExtension">扩展名</param>
        /// <param name="allowExtensions">允许上传的扩展名</param>
        /// <returns>True or False</returns>
        private bool CheckFileExt(string fileExtension, string[] allowExtensions)
        {
            //检查危险文件
            string[] excExt = { ".vbs", ".asp", ".aspx", ".ashx", ".asa", ".asmx", ".asax", ".php", ".jsp", ".htm", ".html", ".shtml" };
            if (excExt.Select(x => x.ToLower()).Contains(fileExtension))
            {
                return false;
            }
            //检查合法文件
            if (allowExtensions.Select(x => x.ToLower()).Contains(fileExtension))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查文件大小
        /// </summary>
        /// <param name="size">文件大小(B)</param>
        /// <param name="sizeLimit">允许上传大小</param>
        /// <returns>True or False</returns>
        private bool CheckFileSize(int size, int sizeLimit)
        {
            if (sizeLimit > 0)
            {
                return size < sizeLimit * 1024;
            }
            return true;
        }

        /// <summary>
        /// 检查文件地址是否文件服务器地址
        /// </summary>
        /// <param name="url">文件地址</param>
        private bool IsExternalIPAddress(string url)
        {
            var uri = new Uri(url);
            switch (uri.HostNameType)
            {
                case UriHostNameType.Dns:
                    var ipHostEntry = Dns.GetHostEntry(uri.DnsSafeHost);
                    foreach (IPAddress ipAddress in ipHostEntry.AddressList)
                    {
                        byte[] ipBytes = ipAddress.GetAddressBytes();
                        if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            if (!IsPrivateIP(ipAddress))
                            {
                                return true;
                            }
                        }
                    }
                    break;

                case UriHostNameType.IPv4:
                    return !IsPrivateIP(IPAddress.Parse(uri.DnsSafeHost));
            }
            return false;
        }

        /// <summary>
        /// 检查IP地址是否本地服务器地址
        /// </summary>
        /// <param name="myIPAddress">IP地址</param>
        private bool IsPrivateIP(IPAddress myIPAddress)
        {
            if (IPAddress.IsLoopback(myIPAddress)) return true;
            if (myIPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                byte[] ipBytes = myIPAddress.GetAddressBytes();
                // 10.0.0.0/24 
                if (ipBytes[0] == 10)
                {
                    return true;
                }
                // 172.16.0.0/16
                else if (ipBytes[0] == 172 && ipBytes[1] == 16)
                {
                    return true;
                }
                // 192.168.0.0/16
                else if (ipBytes[0] == 192 && ipBytes[1] == 168)
                {
                    return true;
                }
                // 169.254.0.0/16
                else if (ipBytes[0] == 169 && ipBytes[1] == 254)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

    }
}