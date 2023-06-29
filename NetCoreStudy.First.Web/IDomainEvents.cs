using MediatR;
using System.Collections.Generic;

namespace NetCoreStudy.First.Web
{
    public interface IDomainEvents
    {
        IEnumerable<INotification> GetDomainEvents();
        void AddDomainEvents(INotification notification);
        void ClearDomainEvents();

    }
}
