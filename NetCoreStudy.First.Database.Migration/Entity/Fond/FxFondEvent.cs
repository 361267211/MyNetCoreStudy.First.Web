using NetCoreStudy.First.Domain.AggregateRoot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Domain.Entity.Fond
{
    public class FxFondEvent : IAggregatrRoot
    {

        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// 事件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 事件类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// FK_FxFondEvent_FxContact:事件发起者
        /// </summary>
        public string EventInitiator { get; set; }

        /// <summary>
        /// 关键词
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// 详情描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 事件开始时间
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 事件结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 关键词
        /// </summary>
        public List<FxFond> Fonds { get; set; }
    }
}
