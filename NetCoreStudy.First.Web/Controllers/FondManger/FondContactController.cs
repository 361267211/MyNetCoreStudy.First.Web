using Microsoft.AspNetCore.Mvc;
using NetCoreStudy.First.Web.FxDto.FxCommonDto;
using NetCoreStudy.First.Web.FxDto.FxFond;
using NetCoreStudy.First.Web.FxService.FxFondService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.Controllers.FondManger
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FondContactController
    {
        private readonly IFondService _fondService;
        private readonly IContactService _contactService;
        private readonly IFondEventService _eventService;

        public FondContactController(IFondService fondService, IContactService contactService, IFondEventService eventService)
        {
            _fondService = fondService;
            _contactService = contactService;
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<List<FxContactDto>> GetContactsByEvent(string eventId)
        {
            return await _fondService.GetAllContactByEvent(eventId);

        }

        /// <summary>
        /// 名称检索Contact
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<FxContactDto>> GetContactsByName(string name)
        {
            return await _contactService.GetContactsByName(name);
        }

        /// <summary>
        /// 获取联系人键值对
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<OptionDto>> GetContactsOptions()
        {
            return await _contactService.GetContactsOptions();
        }
    }
}
