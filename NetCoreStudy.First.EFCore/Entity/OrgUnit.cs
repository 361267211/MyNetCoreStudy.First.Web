using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreStudy.First.EFCore.Entity
{
  public  class OrgUnit
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public OrgUnit Parent { get; set; }
        public List<OrgUnit> Children { get; set; } = new List<OrgUnit>();
    }
}
