using NetCoreStudy.First.Domain;
using NetCoreStudy.First.Domain.Entity;
using NetCoreStudy.First.Domain.ValueObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.FxRepository
{
    public interface IUserDomainRepository
    {
        /// <summary>
        /// 根据手机号查找user
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        Task<User> FindOneAsync(PhoneNumber phoneNumber);
        /// <summary>
        /// 使用userId查找user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<User> FindOneAsync(Guid userId);
        /// <summary>
        /// 添加登录历史
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        Task AddNewLoginHistoryAsync(PhoneNumber phoneNumber, string msg);
        /// <summary>
        /// 发布登录事件
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        Task PublishEventAsync(UserAccessResultEvent eventData);
        /// <summary>
        /// 保存手机号
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Task SavePhoneCodeAsync(PhoneNumber phoneNumber, string code);

        Task<string> RetrievePhoneCodeAsync(PhoneNumber phoneNumber);
        Task<string> FindPhoneNumberCodeAsync(PhoneNumber phoneNumber);

    }
}
