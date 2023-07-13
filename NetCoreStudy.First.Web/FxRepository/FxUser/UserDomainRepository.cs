using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NetCoreStudy.First.Domain;
using NetCoreStudy.First.Domain.Entity;
using NetCoreStudy.First.Domain.ValueObj;
using NetCoreStudy.First.EFCore;
using System;
using System.Threading.Tasks;
using Zack.Infrastructure.EFCore;

namespace NetCoreStudy.First.Web.FxRepository.FxUser
{
    public class UserDomainRepository : IUserDomainRepository
    {
        private UserDbContext _dbContext;
        private readonly IDistributedCache _distributedCache;
        private readonly IMediator _mediator;
        public UserDomainRepository(UserDbContext dbContext, IDistributedCache distributedCache, IMediator mediator)
        {
            _dbContext = dbContext;
            _distributedCache = distributedCache;
            _mediator = mediator;
        }

        public async Task AddNewLoginHistoryAsync(PhoneNumber phoneNumber, string msg)
        {
            User user = await FindOneAsync(phoneNumber);
            Guid? userId = null;
            if (user != null)
            {
                userId = user.Id;
            }
            _dbContext.UserLoginHistories.Add(new UserLoginHistory(userId, phoneNumber, msg));
        }

        public async Task<User> FindOneAsync(PhoneNumber phoneNumber)
        {
            var exp = ExpressionHelper.MakeEqual((User u) => u.PhoneNumber, phoneNumber);

            User user = await _dbContext.Users.Include(e => e.AccessFail).SingleOrDefaultAsync(exp);

            return user;
        }

        public async Task<User> FindOneAsync(Guid userId)
        {
            User user = await _dbContext.Users.Include(e => e.AccessFail).SingleOrDefaultAsync(e => e.Id == userId);
            return user;
        }

        public async Task<string> FindPhoneNumberCodeAsync(PhoneNumber phoneNumber)
        {
            string key = $"PhoneNumberCode{phoneNumber.RegionNumber}_{phoneNumber.Number}";

            string code = await _distributedCache.GetStringAsync(key: key);
            _distributedCache.Remove(key);
            return code;
        }

        public async Task PublishEventAsync(UserAccessResultEvent eventData)
        {
            await _mediator.Publish(eventData);
        }

        public Task<string> RetrievePhoneCodeAsync(PhoneNumber phoneNumber)
        {
            throw new NotImplementedException();
        }

        public async Task SavePhoneCodeAsync(PhoneNumber phoneNumber, string code)
        {
            string key = $"PhoneNumberCode{phoneNumber.RegionNumber}_{phoneNumber.Number}";

            await _distributedCache.SetStringAsync(key: key, value: code, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
        }
    }
}
