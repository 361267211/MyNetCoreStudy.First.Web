using NetCoreStudy.First.Web.FxDto.FxFond;
using NetCoreStudy.First.Web.Request;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.FxService.FxFondService
{
    public interface IFondEventService
    {
        Task<FxFondEventDto> GetEventById(string eventId);
        Task<List<FxFondEventDto>> GetEventsByCondition(EventSearchConditionRequest condition);
        Task<bool> UpdateEvent(FxFondEventDto eventDto);
    }
}
