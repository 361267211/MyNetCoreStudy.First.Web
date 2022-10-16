using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.MediatR
{
    public class PostNotification  : INotification 
    {
        public string ProductName { get; set; }
        public double Price { get; set; }
    }
}
