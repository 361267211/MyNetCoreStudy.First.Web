using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLibrary.Core.Ocelot.JWT
{
    public enum AuthClientType
    {
        /// <summary>
        /// SAAS后台
        /// </summary>
        SaasPlatForm = 1,

        /// <summary>
        /// C端小程序
        /// </summary>
        ToCWehcatApp = 2,

        /// <summary>
        /// B端App
        /// </summary>
        ToBApp = 3
        
    }
}
