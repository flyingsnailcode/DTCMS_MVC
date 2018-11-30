using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DTcms.Common
{
    public class Desend
    {
        #region 私有成员

        private string inputString = null;

        private string outString = null;

        private string inputFilePath = null;

        private string outFilePath = null;

        private string encryptKey = null;

        private string decryptKey = null;

        private string noteMessage = null;
        #endregion
        #region 公共属性

        public string InputString
        {
            get { return inputString; }
            set { inputString = value; }
        }

        public string OutString
        {
            get { return outString; }
            set { outString = value; }
        }

        public string InputFilePath
        {
            get { return inputFilePath; }
            set { inputFilePath = value; }
        }

        public string OutFilePath
        {
            get { return outFilePath; }
            set { outFilePath = value; }
        }

        public string EncryptKey
        {
            get { return encryptKey; }
            set { encryptKey = value; }
        }

        public string DecryptKey
        {
            get { return decryptKey; }
            set { decryptKey = value; }
        }

        public string NoteMessage
        {
            get { return noteMessage; }
            set { noteMessage = value; }
        }
        #endregion
        #region 构造函数
        public Desend()
        {

        }
        #endregion
        #region DES加密字符串
        /// <summary>
        /// 加密字符串
        /// 注意:密钥必须为8位
        /// </summary>
        /// <param name="strText">字符串</param>
        /// <param name="encryptKey">密钥</param>
        public void DesEncrypt()
        {
            byte[] byKey = null;
            byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            try
            {
                byKey = System.Text.Encoding.UTF8.GetBytes(this.encryptKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(this.inputString);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                this.outString = Convert.ToBase64String(ms.ToArray());
            }
            catch (System.Exception error)
            {
                this.noteMessage = error.Message;
            }
        }
        #endregion
        #region DES解密字符串
        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="this.inputString">加了密的字符串</param>
        /// <param name="decryptKey">密钥</param>
        public void DesDecrypt()
        {
            byte[] byKey = null;
            byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            byte[] inputByteArray = new Byte[this.inputString.Length];
            try
            {
                byKey = System.Text.Encoding.UTF8.GetBytes(decryptKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(this.inputString);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = new System.Text.UTF8Encoding();
                this.outString = encoding.GetString(ms.ToArray());
            }
            catch (System.Exception error)
            {
                this.noteMessage = error.Message;
            }
        }
        #endregion
    }
}
