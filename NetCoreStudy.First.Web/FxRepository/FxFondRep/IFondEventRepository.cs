using NetCoreStudy.First.Domain.Entity.Fond;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.FxRepository.FxFondRep
{
    public interface IFondEventRepository
    {
        Task<List<FxFondEvent>> GetEventsByInitiator(string contactId);
    }
}