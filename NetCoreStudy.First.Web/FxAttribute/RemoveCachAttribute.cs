using System;

namespace NetCoreStudy.First.Web.FxAttribute
{
    public class RemoveCachAttribute : Attribute
    {
        private readonly string Resource;

        public RemoveCachAttribute(string resource=null)
        {
            Resource = resource;
        }

    }
}
