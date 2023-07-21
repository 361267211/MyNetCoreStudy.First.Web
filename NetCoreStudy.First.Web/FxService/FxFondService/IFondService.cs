using NetCoreStudy.First.Web.FxDto.FxFond;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.FxService.FxFondService
{
    public interface IFondService
    {
        Task<List<FxContactDto>> GetAllContactByEvent(string eventId);
        Task<List<FxFondEventDto>> GetEventsByInitiator(string contactId);
        Task<List<FxFondDto>> GetFontsByEvent(string eventId);
        Task UpdateFondInEvent(string eventId, List<FxFondDto> lstdtoFond);
    }
}