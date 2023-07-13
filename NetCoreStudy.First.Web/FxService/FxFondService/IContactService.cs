using NetCoreStudy.First.Web.FxDto.FxFond;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.FxService.FxFondService
{
    public interface IContactService
    {
        public Task<List<FxContactDto>> GetContactsByName(string nameCondition);

    }
}