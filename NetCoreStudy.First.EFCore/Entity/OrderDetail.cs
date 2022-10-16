using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreStudy.First.EFCore.Entity
{
   public class OrderDetail
    {
        public int Id { get; set; }

        public Order Order { get; set; }
        public Merchan Marchan { get; set; }
        public int Count { get; set; }
    }
}
