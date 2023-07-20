using Mapster;
using NetCoreStudy.First.Domain.Entity.Fond;
using NetCoreStudy.First.Web.FxDto.FxFond;
using NetCoreStudy.First.Web.FxRepository.FxFondRep;
using NetCoreStudy.First.Web.Request;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.FxService.FxFondService
{
    public class FondEventService : IFondEventService
    {
        private readonly IFondEventRepository _eventRepository;

        public FondEventService(IFondEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<FxFondEventDto> GetEventById(string eventId)
        {
            FxFondEvent fondEvent= await _eventRepository.GetEventById(eventId);
            return fondEvent.Adapt<FxFondEventDto>();
        }

        public async Task<List<FxFondEventDto>> GetEventsByCondition(EventSearchConditionRequest condition)
        {
            List<FxFondEvent> lstEvent = await _eventRepository.GetEventsByCondition(condition);
            return lstEvent.Adapt<List<FxFondEventDto>>();
        }

        public async Task<bool> UpdateEvent(FxFondEventDto eventDto)
        {
           return await _eventRepository.UpdateEvent(eventDto);
        }
    }
}
