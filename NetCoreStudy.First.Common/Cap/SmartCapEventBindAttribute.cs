/*********************************************************
* 名    称：SmartCapEventBindAttribute.cs
* 作    者：刘孟
* 联系方式：电话[13629774594],邮件[1450873843@qq.com]
* 创建时间：20210827
* 描    述：事件类型绑定属性，用于绑定传输消息体类型
* 更新历史：
*
* *******************************************************/
using System;

namespace NetCoreStudy.Core.Cap
{
    /// <summary>
    /// Cap事件与对应消息类型绑定，用于建立事件与消息关联
    /// </summary>
    public class SmartCapEventBindAttribute : Attribute
    {
        /// <summary>
        /// 事件对应消息类型
        /// </summary>
        public Type MsgType { get; set; }
        /// <summary>
        /// 初始化消息类型
        /// </summary>
        /// <param name="msgType"></param>
        public SmartCapEventBindAttribute(Type msgType)
        {
            MsgType = msgType;
        }
    }
}
