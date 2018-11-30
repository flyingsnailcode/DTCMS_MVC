using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using ThoughtWorks.QRCode.Codec;

namespace DTcms.Common
{
    /// <summary>
    /// 二维码生成类
    /// </summary>
    public static class QRCodeHelper
    {

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="LinkCode">二维码内容</param>
        /// <param name="isTest">是否测试</param>
        /// <param name="merchantLogo">商户Logo  默认木有值</param>
        /// <returns></returns>
        public static string CreateQRCode(string LinkCode, string merchantLogo = null)
        {

            string resultPath = string.Empty;


            if (!string.IsNullOrEmpty(LinkCode))
            {
                //创建二维码编码器
                QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                qrCodeEncoder.QRCodeScale = 4;

                qrCodeEncoder.QRCodeVersion = 8;
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;

                //根据传递的字符串 生成一个二维码
                System.Drawing.Image image = qrCodeEncoder.Encode(LinkCode, Encoding.UTF8);


                //将二维码保存到内存流中
                System.IO.MemoryStream MStream = new System.IO.MemoryStream();

                image.Save(MStream, System.Drawing.Imaging.ImageFormat.Png);


                //内存组合图片
                System.IO.MemoryStream MStream1 = new System.IO.MemoryStream();


                Image imageResult = null;

                imageResult = CombinImage(image);

                
                //保存到内存流
                imageResult.Save(MStream1, System.Drawing.Imaging.ImageFormat.Png);

     
                //声明画布
                using (Bitmap bitmap = new Bitmap(500, 500))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, 500, 500));
                        g.DrawImage(image, new Rectangle(10, 10, 480, 480));

                        //string path = DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "/";

                        string path = "";


                        //没有logo到默认文件文件夹
                        string savePath = string.Empty;

                        savePath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["defaultImage"]);

                        savePath = savePath.Replace("{0}", merchantLogo);



                        //数据库保存路径
                        resultPath = ConfigurationManager.AppSettings["defaultImage"].Trim('~');
                        resultPath = resultPath.Replace("{0}", merchantLogo);

                        resultPath = resultPath + path;


                        //最终保存路径
                        savePath = savePath + path;
                        if (!string.IsNullOrEmpty(savePath))
                        {
                            if (!File.Exists(savePath))
                            {
                                Directory.CreateDirectory(savePath);
                            }
                            //路径加上文件名字
                            string fileName = DateTime.Now.ToString("yyyy-MM-dd") + "-" + Guid.NewGuid().ToString();
                            savePath = savePath + fileName + ".png";

                            //数据库保存文件路径
                            resultPath = resultPath + fileName + ".png";

                            //保存路径修改
                            bitmap.Save(savePath, ImageFormat.Png);
                        }
                    }
                }

                MStream.Dispose();
                MStream1.Dispose();
            }
            //HttpContext.Current.Response.Flush();
            //HttpContext.Current.Response.End();
            return resultPath;
        }

        public static Image CombinImage(Image imgBack)
        {
            Graphics g = Graphics.FromImage(imgBack);
            g.DrawImage(imgBack, 0, 0, imgBack.Width, imgBack.Height);
            GC.Collect();
            return imgBack;
        }
    }
}
