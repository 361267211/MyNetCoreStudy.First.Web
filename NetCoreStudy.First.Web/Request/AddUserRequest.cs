using NetCoreStudy.First.Domain.ValueObj;

namespace NetCoreStudy.First.Web.Request
{
    public record AddUserRequest(PhoneNumber phoneNo,string password);
  
}
