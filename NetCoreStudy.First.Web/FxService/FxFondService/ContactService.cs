using Mapster;
using NetCoreStudy.First.Domain.Entity.Fond;
using NetCoreStudy.First.Web.FxDto.FxCommonDto;
using NetCoreStudy.First.Web.FxDto.FxFond;
using NetCoreStudy.First.Web.FxRepository.FxFondRep;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.FxService.FxFondService
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;

        public ContactService(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<List<FxContactDto>> GetContactsByName(string nameCondition)
        {
            List<FxContact> contacts = await _contactRepository.GetContactsByName(nameCondition);
          
            return contacts.Adapt<List<FxContactDto>>().ToList();
        }

        public async Task<List<OptionDto>> GetContactsOptions()
        {
            return await _contactRepository.GetContactsOptions();
        }
    }
}
