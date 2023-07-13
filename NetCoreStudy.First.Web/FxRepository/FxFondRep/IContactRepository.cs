using NetCoreStudy.First.Domain.Entity.Fond;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.FxRepository.FxFondRep
{
    public interface IContactRepository
    {
        Task<List<FxContact>> GetContactsByName(string nameCondition);
        Task<List<FxContact>> GetFxContactsByFonds(List<string> fondIds);
    }
}