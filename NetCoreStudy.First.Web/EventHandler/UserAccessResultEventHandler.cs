using MediatR;
using NetCoreStudy.First.Domain;
using NetCoreStudy.First.EFCore;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.EventHandler
{
    public class UserAccessResultEventHandler : INotificationHandler<UserAccessResultEvent>
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
