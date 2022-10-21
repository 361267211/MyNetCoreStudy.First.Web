using NetCoreStudy.First.Domain.Entity;
using NetCoreStudy.First.Domain.ValueObj;
//using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Domain
{
    public class UserDomainService
    {
        private IUserDomainRepository _userDomainRepository;
        private IServiceProvider _serviceProvider;

        public UserDomainService(IUserDomainRepository userDomainRepository, IServiceProvider serviceProvider)
        {
            _userDomainRepository = userDomainRepository;
            _serviceProvider = serviceProvider;
        }

        public async Task<UserAccessResult> CheckPassword(PhoneNumber phoneNumber,string password)
        {
            UserAccessResult result;
            var user = await _userDomainRepository.FindOneAsync(phoneNumber);
            if (user==null)
            {
                result=UserAccessResult.PhoneNumberNotFound;
            }
            else if(IsLockOut(user))
            {
                result = UserAccessResult.Lockout;
            }
            else if (user.HasPassword()==false)
            {
                result = UserAccessResult.NoPassword;
            }
            else if (user.CheckPassword(password))
            {
                result = UserAccessResult.OK;
            }
            else
            {
                result  = UserAccessResult.PasswordError;
            }
            if (user!=null)
            {
                if (result == UserAccessResult.OK)
                {
                    ResetAccessFail(user);
                }
                else
                {
                    AccessFail(user);
                }
            }

            await _userDomainRepository.PublishEventAsync(new UserAccessResultEvent(phoneNumber,result));
            return result;
 
        }

        public async Task<CheckCodeResult> CheckCodeAsync(PhoneNumber phoneNumber,string code)
        {
            User? user = await _userDomainRepository.FindOneAsync(phoneNumber);
            if (user==null)
            {
                return CheckCodeResult.PhoneNumberNotFount;
            }
            else if (IsLockOut(user))
            {
                return CheckCodeResult.Lockout; 
            }

            string? codeInServer = await _userDomainRepository.FindPhoneNumberCodeAsync(phoneNumber);
            if (codeInServer==null)
            {
                return CheckCodeResult.CodeError;
            }
            else if (codeInServer==code)
            {
                return CheckCodeResult.OK;

            }
            else 
            {
                AccessFail(user);
                return CheckCodeResult.CodeError;
            }
        }

        public void ResetAccessFail(User user)
        {
            user.AccessFail.Reset();
        }

        public bool IsLockOut(User user)
        {
            return user.AccessFail.IsLockOut();
        }

        public void AccessFail(User user)
        {
              user.AccessFail.Fail();
        }
    }
}
