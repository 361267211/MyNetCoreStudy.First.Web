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

        public async Task<List<FxContactDto>> GetContactsInExcel(List<FxContactDto> lstContactDto)
        {
            //1.取DB中名称和名称列表匹配的列表
            List<FxContactDto> lstDbExistedContact = await _contactRepository.GetContactsByNameList(lstContactDto.Select(e => e.Name).ToList());

            //2.根据1中的结果，取excel中有，DB没有的ContactDto
            var nameList = lstDbExistedContact.Select(e => e.Name);
            lstContactDto.RemoveAll(e => nameList.Contains(e.Name));

            //3.把不存在的Contact 根据 lstContactDto 导入到DB中
            List<FxContactDto> lstContactOnlyInExcel = (await _contactRepository.AddContactBatch(lstContactDto)).Adapt<List<FxContactDto>>();
            
            //4.组装成携带id的集合
            lstDbExistedContact.AddRange(lstContactOnlyInExcel);

            return lstDbExistedContact;

        }

        public async Task<List<OptionDto>> GetContactsOptions()
        {
            return await _contactRepository.GetContactsOptions();
        }
    }
}
