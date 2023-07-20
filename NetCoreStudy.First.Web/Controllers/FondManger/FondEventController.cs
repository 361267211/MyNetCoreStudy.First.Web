using Microsoft.AspNetCore.Mvc;
using NetCoreStudy.First.Web.FxDto.FxFond;
using NetCoreStudy.First.Web.FxService.FxFondService;
using NetCoreStudy.First.Web.Request;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.Controllers.FondManger
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FondEventController
    {
        private readonly IFondService _fondService;
        private readonly IContactService _contactService;
        private readonly IFondEventService _eventService;

        public FondEventController(IFondService fondService, IContactService contactService, IFondEventService eventService)
        {
            _fondService = fondService;
            _contactService = contactService;
            _eventService = eventService;
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

        /// <summary>
        /// 根据符合条件查询事件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<List<FxFondEventDto>> GetEventsByCondition([FromBody] EventSearchConditionRequest condition)
        {
            return await _eventService.GetEventsByCondition(condition);
        }

        /// <summary>
        /// 通过id获取事件
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<FxFondEventDto> GetEventById( string eventId)
        {
            return await _eventService.GetEventById(eventId);
        }

        /// <summary>
        /// 更新事件
        /// </summary>
        /// <param name="eventDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> UpdateEvent(FxFondEventDto eventDto)
        {

          return await  _eventService.UpdateEvent(eventDto);

        }
    }
}
