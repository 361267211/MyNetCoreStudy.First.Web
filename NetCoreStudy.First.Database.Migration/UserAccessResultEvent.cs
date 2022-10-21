//using MediatR;
using MediatR;
using NetCoreStudy.First.Domain.ValueObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Domain
{
    public record class UserAccessResultEvent(PhoneNumber PhoneNumber, UserAccessResult Result) : INotification;

}
