using NetCoreStudy.First.Domain.Entity;
using NetCoreStudy.First.Domain.ValueObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Domain
{
    public interface IUserDomainRepository
    {
        Task<User?> FindOneAsync(PhoneNumber phoneNumber);
        Task<User?> FindOneAsync(Guid userId);

        Task AddNewLoginHistoryAsync(PhoneNumber phoneNumber, string msg);
        Task PublishEventAsync(UserAccessResultEvent eventData);

        Task SavePhoneCodeAsync(PhoneNumber phoneNumber, string code);

        Task<string?> RetrievePhoneCodeAsync(PhoneNumber phoneNumber);
        Task<string?> FindPhoneNumberCodeAsync(PhoneNumber phoneNumber);

    }
}
