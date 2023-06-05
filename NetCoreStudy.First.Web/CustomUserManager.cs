using IdentityServer.EFCore.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace NetCoreStudy.First.Web
{
    public class CustomUserManager<TUser> : UserManager<TUser> where TUser : MyUser
    {
        private readonly IUserStore<TUser>  _userStore;
        public CustomUserManager(IUserStore<TUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<TUser> passwordHasher,
            IEnumerable<IUserValidator<TUser>> userValidators, IEnumerable<IPasswordValidator<TUser>> passwordValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<TUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _userStore=store;
        }

        public async Task<List<TUser>> GetUserByCondition(string condition)
        {
            var customUserStore = _userStore as CustomUserStore<TUser>;
            if (customUserStore != null)
            {
                return await customUserStore.GetUserByCondition(condition);
            }

            throw new NotSupportedException("CustomUserStore is not configured.");
        }
    }

}
