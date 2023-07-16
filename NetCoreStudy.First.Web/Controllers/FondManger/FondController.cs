using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCoreStudy.First.Domain.Entity.Fond;
using NetCoreStudy.First.Web.FxDto.FxFond;
using NetCoreStudy.First.Web.FxService.FxFondService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.Controllers.FondManger
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FondController : ControllerBase
    {
        private readonly IFondService _fondService;
        private readonly IContactService _contactService;

        public FondController(IFondService fondService, IContactService contactService)
        {
            _fondService = fondService;
            _contactService = contactService;
        }

        [HttpGet]
        public async Task<List<FxContactDto>> GetContactsByEvent(string eventId)
        {
            return await _fondService.GetAllContactByEvent(eventId);

        }

        [HttpGet]
        public async Task<List<FxFondDto>> GetFondsByEvent(string eventId)
        {
            return await _fondService.GetFontsByEvent(eventId);

        }

        [HttpGet]
        public async Task<List<FxContactDto>> GetContactsByName(string name)
        {
            return await _contactService.GetContactsByName(name);
        }

        /// <summary>
        /// 根据事件主导者查询事件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<FxFondEventDto>> GetEventsByInitiator(string contactId)
        {
            return await _fondService.GetEventsByInitiator(contactId);
        }
    }
}
