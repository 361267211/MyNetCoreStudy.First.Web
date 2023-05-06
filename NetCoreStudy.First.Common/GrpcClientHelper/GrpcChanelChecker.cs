/*********************************************************
* 名    称：GrpcChanelChecker.cs
* 作    者：刘孟
* 联系方式：电话[13629774594],邮件[1450873843@qq.com]
* 创建时间：20210827
* 描    述：格式校验
* 更新历史：
*
* *******************************************************/
using System;
using System.Text.RegularExpressions;

namespace NetCoreStudy.Core.GrpcClientHelper
{
    /// <summary>
    /// 数据校验帮助类
    /// </summary>
    public static class GrpcChanelChecker
    {
        /// <summary>
        /// 检查对象是否为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="argument"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public static T CheckNotNull<T>(T argument, string paramName) where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(paramName);
            }
            return argument;
        }

        /// <summary>
        /// 检查字符串不能为空或者""
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public static string CheckNotNullOrEmpty(string argument, string paramName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(paramName);
            }
            if (argument == "")
            {
                throw new ArgumentException("不能为空字符串", paramName);
            }
            return argument;
        }


        /// <summary>
        /// 检测串值是否为合法的网址格式
        /// </summary>
        /// <param name="strValue">要检测的String值</param>
        /// <returns>成功返回true 失败返回false</returns>
        public static bool CheckIsUrlFormat(string strValue)
        {
            return CheckIsFormat(@"(http://)?([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", strValue);
        }

        /// <summary>
        /// 检测串值是否为合法的格式
        /// </summary>
        /// <param name="strRegex">正则表达式</param>
        /// <param name="strValue">要检测的String值</param>
        /// <returns>成功返回true 失败返回false</returns>
        public static bool CheckIsFormat(string strRegex, string strValue)
        {
            if (strValue != null && strValue.Trim() != "")
            {
                Regex re = new Regex(strRegex);
                if (re.IsMatch(strValue))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

    }
}
