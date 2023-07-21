using NetCoreStudy.First.Domain.Entity.Fond;
using NetCoreStudy.First.Web.FxDto.FxFond;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.FxRepository.FxFondRep
{
    public interface IFondRepository
    {
        Task AddFonds(List<FxFondDto> lstdtoFond);
        Task DeleteFondInEvent(string eventId);
        Task<List<FxFond>> GetFondsByEvent(string eventId);
    }
}
