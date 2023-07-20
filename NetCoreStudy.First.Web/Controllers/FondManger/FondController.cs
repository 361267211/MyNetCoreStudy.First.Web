using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCoreStudy.First.Domain.Entity.Fond;
using NetCoreStudy.First.Web.FxDto.FxCommonDto;
using NetCoreStudy.First.Web.FxDto.FxFond;
using NetCoreStudy.First.Web.FxService.FxFondService;
using NetCoreStudy.First.Web.Request;
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
        private readonly IFondEventService _eventService;

        public FondController(IFondService fondService, IContactService contactService, IFondEventService eventService)
        {
            _fondService = fondService;
            _contactService = contactService;
            _eventService = eventService;
        }

       

        /// <summary>
        /// 根据事件id 查Fonds列表
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<FxFondDto>> GetFondsByEvent(string eventId)
        {
            return await _fondService.GetFontsByEvent(eventId);

        }


    }
}
