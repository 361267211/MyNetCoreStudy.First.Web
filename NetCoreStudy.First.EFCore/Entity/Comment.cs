using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreStudy.First.EFCore.Entity
{
  public  class Comment
    {
        public long Id { get; set; }
        public Article Article { get; set; }
        public string Message { get; set; }
    }
}
