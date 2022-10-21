using NetCoreStudy.First.Domain;
using NetCoreStudy.First.Domain.ValueObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.First.EFCore
{
    public class MockSmsCodeSender : ISmsCodeSender
    {
        public async Task SendAsync(PhoneNumber phoneNumber, string code)
        {
            Console.WriteLine($"向{phoneNumber.RegionNumber}-{phoneNumber.Number}发送消息");
        }
    }
}
