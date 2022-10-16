using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreStudy.First.Web
{
   public interface IDomainEvents
    {
        IEnumerable<INotification> GetDomainEvents();
        void AddDomainEvents(INotification notification);
        void ClearDomainEvents();

    }
}
