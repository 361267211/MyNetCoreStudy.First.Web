/*********************************************************
* 名    称：SmartCapSubscribeAttribute.cs
* 作    者：刘孟
* 联系方式：电话[13629774594],邮件[1450873843@qq.com]
* 创建时间：20210827
* 描    述：复写TopicAattribute，用于区分不同租户事件
* 更新历史：
*
* *******************************************************/
using DotNetCore.CAP;

namespace NetCoreStudy.Core.Cap
{
    /// <summary>
    /// 智图事件订阅属性，给订阅事件自动添加租户作为前缀
    /// </summary>
    public class SmartCapSubscribeAttribute : CapSubscribeAttribute
    {
        public SmartCapSubscribeAttribute(string name, bool isPartial = false) : base(name.ReplaceTenant(SmartCapConfig.TenantName), isPartial)
        {

        }
    }
}
