using NetCoreStudy.First.Domain.Entity.Fond;
using NetCoreStudy.First.Web.FxDto.FxCommonDto;
using NetCoreStudy.First.Web.FxDto.FxFond;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.FxRepository.FxFondRep
{
    public interface IContactRepository
    {
        Task<List<FxContact>> AddContactBatch(List<FxContactDto> lstContact);
        Task<List<FxContact>> GetContactsByName(string nameCondition);
        Task<List<FxContactDto>> GetContactsByNameList(List<string> nameList);
        Task<List<OptionDto>> GetContactsOptions();
        Task<List<FxContact>> GetFxContactsByFonds(List<string> fondIds);
    }
}