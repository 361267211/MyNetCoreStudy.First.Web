/*********************************************************
* 名    称：SmartCapEvent.cs
* 作    者：刘孟
* 联系方式：电话[13629774594],邮件[1450873843@qq.com]
* 创建时间：20210827
* 描    述：Cap事件描述基类
* 更新历史：
*
* *******************************************************/
using System.ComponentModel;

namespace NetCoreStudy.Core.Cap
{
    /// <summary>
    /// Cap事件基类
    /// </summary>
    public abstract class SmartCapEventBase
    {
    }
    /// <summary>
    /// Cap发布事件基类
    /// </summary>
    public abstract class SmartCapPublishEventBase : SmartCapEventBase
    {

    }
    /// <summary>
    /// Cap订阅事件基类
    /// </summary>
    public abstract class SmartCapSubscribeEventBase : SmartCapEventBase
    {

    }
    /// <summary>
    /// Cap事件消息基类
    /// </summary>
    public abstract class SmartCapEventMsgBase
    {
        /// <summary>
        /// 租户名称
        /// </summary>
        [Description("租户名称")]
        public string TenantName { get; set; }
    }
}
