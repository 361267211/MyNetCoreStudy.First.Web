using System;

namespace NetCoreStudy.First.Web.Attr
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UnitOfWorkAttribute : Attribute
    {
        public UnitOfWorkAttribute(params Type[] dbContextType)
        {
            _dbContextType = dbContextType;
        }

        public Type[] _dbContextType { get; init; }
    }
}
