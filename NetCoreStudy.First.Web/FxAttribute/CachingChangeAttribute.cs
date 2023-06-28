using System;
using System.ComponentModel.DataAnnotations;

namespace NetCoreStudy.First.Web.FxAttribute
{
    /// <summary>
    /// 注意：关联的资源名称 ResourceName ,必须在特性中传入，否则无法删除对应的缓存
    /// </summary>
    public class CachingChangeAttribute : Attribute
    {
        /// <summary>
        /// 关联的资源名称,必填
        /// </summary>
        public string ResourceName;
        /// <summary>
        /// 缓存变更特性
        /// </summary>
        /// <param name="resourceName">关联的资源名称</param>
        public CachingChangeAttribute()
        {

        }
    }
}
