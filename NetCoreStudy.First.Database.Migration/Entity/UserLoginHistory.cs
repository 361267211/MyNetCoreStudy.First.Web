using NetCoreStudy.First.Domain.AggregateRoot;
using NetCoreStudy.First.Domain.ValueObj;
//using NetCoreStudy.First.EFCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Domain.Entity
{
    public class UserLoginHistory: IAggregatrRoot
    {
        public long Id { get; init; }
        public Guid? UserId { get; init; }
        public PhoneNumber PhoneNumber { get; init; }
        public string Message { get; init; }
        public DateTime CreatedDateTime { get; init; }
        private UserLoginHistory()
        {}
        public UserLoginHistory(Guid? userId, PhoneNumber phoneNumber, string message )
        {
            UserId = userId;
            PhoneNumber = phoneNumber;
            Message = message;
            CreatedDateTime = DateTime.Now;
        }


    }
}
