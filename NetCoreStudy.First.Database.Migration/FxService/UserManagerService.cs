using IdentityServer.EFCore.Entity;
using NetCoreStudy.First.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Domain
{
    public class UserManagerService: IUserManagerService
    {
        private IUserManagerRepository _userManagerRepository;

        public UserManagerService(IUserManagerRepository userManagerRepository)
        {
            _userManagerRepository = userManagerRepository;
        }

        public async Task<List<MyUser>> GetUsersByDynamicConditionAsync(UserQueryCondition queryCondition)
        {
            return await _userManagerRepository.GetUsersByDynamicConditionAsync(queryCondition);
        }
    }
}
