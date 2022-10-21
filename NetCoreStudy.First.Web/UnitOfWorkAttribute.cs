using System;

namespace NetCoreStudy.First.Web
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UnitOfWorkAttribute: Attribute
    {
        public UnitOfWorkAttribute(Type[] dbContextType)
        {
            _dbContextType = dbContextType;
        }

        public Type[] _dbContextType { get; init; }   
    }
}
