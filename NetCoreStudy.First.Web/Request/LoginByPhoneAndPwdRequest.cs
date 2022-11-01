using NetCoreStudy.First.Domain.ValueObj;

namespace NetCoreStudy.First.Web.Request
{
    public record LoginByPhoneAndPwdRequest(PhoneNumber PhoneNumber,string password);    
}
