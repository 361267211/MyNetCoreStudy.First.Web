using NetCoreStudy.First.Domain.Entity.Fond;
using NetCoreStudy.First.Web.FxDto.FxFond;
using NetCoreStudy.First.Web.Request;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.FxRepository.FxFondRep
{
    public interface IFondEventRepository
    {
        Task<FxFondEvent> GetEventById(string eventId);
        Task<List<FxFondEvent>> GetEventsByCondition(EventSearchConditionRequest condition);
        Task<List<FxFondEvent>> GetEventsByInitiator(string contactId);
        Task<bool> UpdateEvent(FxFondEventDto eventDto);
    }
}