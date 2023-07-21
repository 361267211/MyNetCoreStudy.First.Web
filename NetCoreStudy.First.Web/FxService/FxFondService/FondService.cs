using Mapster;
using NetCoreStudy.First.Web.FxDto.FxFond;
using NetCoreStudy.First.Web.FxRepository.FxFondRep;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.FxService.FxFondService
{
    public class FondService : IFondService
    {
        private readonly IFondRepository _fondRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IFondEventRepository _eventRepository;

        public FondService(IFondRepository fondRepository, IContactRepository contactRepository, IFondEventRepository eventRepository)
        {
            _fondRepository = fondRepository;
            _contactRepository = contactRepository;
            _eventRepository = eventRepository;
        }

        public async Task<List<FxContactDto>> GetAllContactByEvent(string eventId)
        {
            var fonds = await _fondRepository.GetFondsByEvent(eventId);
            var lstContactId = fonds.Select(e => e.InContactId).ToList();
            var lstCons = await _contactRepository.GetFxContactsByFonds(lstContactId);

            var lstdtoCons = lstCons.Adapt<List<FxContactDto>>();
            return lstdtoCons;
        }

        public async Task<List<FxFondEventDto>> GetEventsByInitiator(string contactId)
        {
            var eventsEntity= await _eventRepository.GetEventsByInitiator(contactId);
            return eventsEntity.Adapt<List<FxFondEventDto>>();
        }

        public async Task<List<FxFondDto>> GetFontsByEvent(string eventId)
        {
            var lstFonds = await _fondRepository.GetFondsByEvent(eventId);

            var lstdtoFond = lstFonds.Adapt<List<FxFondDto>>();
            return lstdtoFond;
        }

        public async Task UpdateFondInEvent(string eventId, List<FxFondDto> lstdtoFond)
        {
            //1.删除原有的所有 fonds
           await _fondRepository.DeleteFondInEvent(eventId);
            //2.新增 fonds
            await _fondRepository.AddFonds(lstdtoFond);
            throw new System.NotImplementedException();
        }
    }
}
