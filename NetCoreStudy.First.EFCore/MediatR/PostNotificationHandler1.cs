using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.MediatR
{
    public class PostNotificationHandler1 : NotificationHandler<PostNotification>
    {
        protected override void Handle(PostNotification notification)
        {
            Console.WriteLine($"1111111-{notification.ProductName}" );
        }
    }
}
