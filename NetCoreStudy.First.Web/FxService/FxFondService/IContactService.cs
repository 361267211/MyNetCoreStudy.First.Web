using NetCoreStudy.First.Web.FxDto.FxCommonDto;
using NetCoreStudy.First.Web.FxDto.FxFond;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.FxService.FxFondService
{
    public interface IContactService
    {
        public Task<List<FxContactDto>> GetContactsByName(string nameCondition);
        Task<List<FxContactDto>> GetContactsInExcel(List<FxContactDto> lstContactDto);
        public Task<List<OptionDto>> GetContactsOptions();
    }
}