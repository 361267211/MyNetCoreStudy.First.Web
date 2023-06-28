using System;

namespace NetCoreStudy.First.Web.FxAttribute
{
    public class CachingAttribute : Attribute
    {
        public string Resource;
        /// <summary>
        /// 缓存绝对过期时间（分钟）
        /// </summary>
        public int AbsoluteExpiration { get; set; } = 30;
        public CachingAttribute(string resource=null)
        {
            Resource = resource;
        }

    }
}
