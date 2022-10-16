using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.JWT
{
    public class JWTSettings
    {
        public string SecKey { get; set; }
        public int ExpireSeconds { get; set; }
    }
}
