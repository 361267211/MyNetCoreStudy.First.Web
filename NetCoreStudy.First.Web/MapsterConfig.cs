using Furion.DatabaseAccessor;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using NetCoreStudy.First.Domain.Entity.Fond;
using NetCoreStudy.First.Web.FxDto.FxFond;
using NetCoreStudy.First.Web.IdentityService4;
using System;
using System.Reflection;

namespace NetCoreStudy.First.Web
{
    public static class MapsterConfig 
    {
        public static void RegisterMapsterConfiguration(this IServiceCollection services)
        {
            TypeAdapterConfig<FxFond, FxFondDto>.ForType()
                .Map(dest => dest.Description, src => src.Description);


            TypeAdapterConfig<FxFondDto, FxFond>.ForType ()
                .Map(dest => dest.Id, src => AddIdAtEty2Dto(src.Id));



            TypeAdapterConfig<FxContactDto, FxContact>.ForType ()
                .Map(dest => dest.Id, src => AddIdAtEty2Dto(src.Id));




            TypeAdapterConfig<FxFondEventDto, FxFondEvent>.ForType ()
                .Map(dest => dest.Id, src => AddIdAtEty2Dto(src.Id));
        }



        private static string AddIdAtEty2Dto(string etyId)
        {
            return string.IsNullOrEmpty(etyId) ? Guid.NewGuid().ToString() : etyId;
        }
    }
}
