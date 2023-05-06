
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.EFCore.Entity
{
    public class MyUser:IdentityUser<long>
    {
        public string? WeiChatAccount { get; set; }
    }
}
