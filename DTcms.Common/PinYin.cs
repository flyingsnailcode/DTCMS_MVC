using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.International.Converters.PinYinConverter;

namespace DTcms.Common
{
    /// <summary>
    /// 汉字转拼音类
    /// </summary>
    public class PinYin
    {
        /// <summary>
        /// 汉字转换成全拼的拼音
        /// </summary>
        /// <param name="Chstr">汉字字符串</param>
        /// <param name="is_short">简写</param>
        /// <returns>转换后的拼音字符串</returns>
        public static string convertCh(string Chstr, bool is_short = false)
        {
            string PYstr = "";
            foreach (char item in Chstr.ToCharArray())
            {
                if (item == '区')
                {
                    if (is_short)
                    {
                        PYstr += "Q";
                    }
                    else
                    {
                        PYstr += "Qu";
                    }
                }
                else if (item == '藏')
                {
                    if (is_short)
                    {
                        PYstr += "Z";
                    }
                    else
                    {
                        PYstr += "Zang";
                    }
                }
                else if (item == '南')
                {
                    if (is_short)
                    {
                        PYstr += "N";
                    }
                    else
                    {
                        PYstr += "Nan";
                    }
                }
                else if (item == '辖')
                {
                    if (is_short)
                    {
                        PYstr += "X";
                    }
                    else
                    {
                        PYstr += "Xia";
                    }
                }
                else if (item == '地')
                {
                    if (is_short)
                    {
                        PYstr += "D";
                    }
                    else
                    {
                        PYstr += "Di";
                    }
                }
                else if (item == '广')
                {
                    if (is_short)
                    {
                        PYstr += "G";
                    }
                    else
                    {
                        PYstr += "Guang";
                    }
                }
                else if (ChineseChar.IsValidChar(item))
                {
                    ChineseChar cc = new ChineseChar(item);
                    if (is_short)
                    {
                        PYstr += cc.Pinyins[0].Substring(0, 1);
                    }
                    else
                    {
                        PYstr += cc.Pinyins[0].Substring(0, cc.Pinyins[0].Length - 1);
                    }
                }
                else
                {
                    PYstr += item.ToString();
                }
            }
            return PYstr;
        }
    }
}
