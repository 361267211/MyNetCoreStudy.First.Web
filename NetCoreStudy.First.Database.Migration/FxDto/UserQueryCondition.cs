using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Domain.FxDto
{
    public class UserQueryCondition
    {
        public List<UesrQueryCondition> ConditionList { get; set; }
    }

    public class UesrQueryCondition
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }
}
