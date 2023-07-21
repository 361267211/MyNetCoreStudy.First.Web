using NetCoreStudy.First.Domain.AggregateRoot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Domain.Entity.Fond
{
    public class FxFond:IAggregatrRoot
    {

        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// 支出方
        /// </summary>
        public string ExContactId { get; set; }

        /// <summary>
        /// 收入方
        /// </summary>
        public string InContactId { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 活动金额
        /// </summary>
        public decimal Amount { get; set; }

        [Required]
        public string FxFondEventId { get; set; }
        public FxFondEvent FondEvent { get; set; } = null!;
    }
}
