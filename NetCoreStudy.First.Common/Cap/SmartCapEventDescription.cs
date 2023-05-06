/*********************************************************
* 名    称：SmartCapEventDescription.cs
* 作    者：刘孟
* 联系方式：电话[13629774594],邮件[1450873843@qq.com]
* 创建时间：20210827
* 描    述：Cap时间描述相关类，用于生成Cap事件以及消息体自描述信息
* 更新历史：
*
* *******************************************************/
using System.Collections.Generic;

namespace NetCoreStudy.Core.Cap
{
    /// <summary>
    /// Cap事件描述信息
    /// </summary>
    public class SmartCapEventField
    {
        /// <summary>
        /// 字段类型
        /// </summary>
        public string FieldType { get; set; }
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// 字段说明
        /// </summary>
        public string FieldDesc { get; set; }
    }
    /// <summary>
    /// Cap事件消息描述信息
    /// </summary>
    public class SmartCapEventMsg
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public SmartCapEventMsg()
        {
            Fields = new List<SmartCapEventField>();
        }
        /// <summary>
        /// 消息类型名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 类型字段信息
        /// </summary>
        public List<SmartCapEventField> Fields { get; set; }
    }
    /// <summary>
    /// Cap事件描述信息
    /// </summary>
    public class SmartCapEventDes
    {
        /// <summary>
        /// 事件名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 事件值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 事件说明
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 事件对应消息描述信息
        /// </summary>
        public SmartCapEventMsg BindMsg { get; set; }
    }

    /// <summary>
    /// 事件描述信息，用于描述事件以及关联消息类型
    /// </summary>
    public class SmartCapEventIntroduction
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public SmartCapEventIntroduction()
        {
            EventList = new List<SmartCapEventDes>();
        }
        /// <summary>
        /// 事件定义类型名称
        /// </summary>
        public string ServiceEventName { get; set; }
        /// <summary>
        /// 包含的事件信息
        /// </summary>
        public List<SmartCapEventDes> EventList { get; set; }
    }
}
