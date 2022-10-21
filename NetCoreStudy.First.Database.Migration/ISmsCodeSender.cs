using NetCoreStudy.First.Domain.ValueObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Domain
{
    public interface ISmsCodeSender
    {
        Task SendAsync(PhoneNumber phoneNumber, string code);
    }
}
