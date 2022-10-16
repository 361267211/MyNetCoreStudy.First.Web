using NetCoreStudy.First.Utility;
using NetCoreStudy.First.Web.MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetCoreStudy.First.EFCore.Entity
{
   public class Order: BaseEntity, IAggregateRoot
    {
        public int Id { get; set; }
        public DateTime  CreateDateTime { get; set; }
        public double TotalAmount { get; set; }
        public List<OrderDetail> Details { get; set; } = new List<OrderDetail>();

        public void AddDetail(Merchan merchan,int count)
        {
            var detail = Details.SingleOrDefault(e => e.Marchan == merchan);
            if (detail==null)
            {
                Details.Add(new OrderDetail {Marchan=merchan,Count=count });
            }
            else
            {
                detail.Count += count;
            }

            this.AddDomainEvents(new PostNotification { ProductName = $"{merchan.Name}", Price = merchan.Price });
        }
    }
}
