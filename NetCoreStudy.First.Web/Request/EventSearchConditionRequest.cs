using System;
using System.Collections.Generic;

namespace NetCoreStudy.First.Web.Request
{
    public class EventSearchConditionRequest
    {
        /// <summary>
        /// 名称前缀
        /// </summary>
        public string NamePrefix { get; set; }
        /// <summary>
        /// 关键词
        /// </summary>
        public string KeyWord { get; set; }
        /// <summary>
        /// 事件类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 开始事件
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 结束事件
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 事件发起人id
        /// </summary>
        public List<string> Initiators { get; set; }
    }
}
