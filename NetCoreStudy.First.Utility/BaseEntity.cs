using MediatR;
using NetCoreStudy.First.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NetCoreStudy.First.Utility
{
    public abstract class BaseEntity : IDomainEvents
    {
        [NotMapped]
        public IList<INotification> events = new List<INotification>();
        public void AddDomainEvents(INotification notification)
        {
            events.Add(notification);
        }

        public void ClearDomainEvents()
        {
            events.Clear();
        }

        public IEnumerable<INotification> GetDomainEvents()
        {
            return events;
        }
    }
}
