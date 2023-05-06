/*********************************************************
 * 名    称：IgnoreMappingAttribute
 * 作    者：张祖琪
 * 联系方式：电话[13883914813],邮件[361267211@qq.com]
 * 创建时间：2021/8/05 16:57:45
 * 描    述：自定义特性 添加了此特性的属性不会被配置到siteglobalconfig上。
 *
 * 更新历史：
 *
 * *******************************************************/

using System;


namespace NetCoreStudy.Core.Apollo
{
    /// <summary>
    /// 自定义特性  作用对象：属性 添加特性后不会通过配置中心取值
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreMappingAttribute : Attribute
    {
      
    }
}
