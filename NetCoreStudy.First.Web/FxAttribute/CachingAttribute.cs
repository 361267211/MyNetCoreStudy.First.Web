﻿using System;

namespace NetCoreStudy.First.Web.FxAttribute
{
    /// <summary>
    /// 注意：关联的资源名称 ResourceName ,必须在特性中传入，否则无法删除对应的缓存
    /// </summary>
    public class CachingAttribute : Attribute
    {
        /// <summary>
        /// 关联的资源名称,必填
        /// </summary>
        public string ResourceName;
        /// <summary>
        /// 缓存绝对过期时间（分钟）
        /// </summary>
        public int AbsoluteExpiration { get; set; } = 30;
        public CachingAttribute(string resourceName)
        {
            ResourceName = resourceName;
        }

    }
}
