using MediatR;
using NetCoreStudy.First.Domain;
using NetCoreStudy.First.EFCore;
using NetCoreStudy.First.Web.FxRepository;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.EventHandler
{

    //todo:此处暂时注释了继承，后期需要重新写
    public class UserAccessResultEventHandler /*: INotificationHandler<UserAccessResultEvent>*/
    {
        private readonly IUserDomainRepository _userRepository;
        private readonly UserDbContext _userDb;

        public UserAccessResultEventHandler(IUserDomainRepository userRepository, UserDbContext userDb)
        {
            _userRepository = userRepository;
            _userDb = userDb;
        }

        public async Task Handle(UserAccessResultEvent notification, CancellationToken cancellationToken)
        {
         //await  Task.Delay(5000);
            await _userRepository.AddNewLoginHistoryAsync(notification.PhoneNumber, $"登陆结果:{notification.Result}");

            try
            {
                await _userDb.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {

                System.Console.WriteLine();
            }
       
        }
    }
}
