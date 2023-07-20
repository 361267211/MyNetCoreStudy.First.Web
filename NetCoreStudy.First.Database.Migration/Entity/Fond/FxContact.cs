using NetCoreStudy.First.Domain.AggregateRoot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Domain.Entity.Fond
{
    public class FxContact: IAggregatrRoot
    {

        /// <summary>
        /// 联系人Id
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 联系人名称
        /// </summary>
        public string Name { get; set; }


    }
}
