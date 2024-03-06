using AutoMapper;
using CoreCodeCamp.Models;

namespace CoreCodeCamp.Data
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            // m = model, e = entity
            CreateMap<Camp, CampModel>()
                .ForMember(campModel => campModel.Venue,
                    options => options.MapFrom(camp => camp.Location.VenueName))
                .ForMember(m => m.Address1,
                    options => options.MapFrom(e => e.Location.Address1))
                .ForMember(m => m.Address2,
                    options => options.MapFrom(e => e.Location.Address2))
                .ForMember(m => m.Address3,
                    options => options.MapFrom(e => e.Location.Address3))
                .ForMember(m => m.CityTown,
                    options => options.MapFrom(e => e.Location.CityTown))
                .ForMember(m => m.StateProvince,
                    options => options.MapFrom(e => e.Location.StateProvince))
                .ForMember(m => m.PostalCode,
                    options => options.MapFrom(e => e.Location.PostalCode))
                .ForMember(m => m.Country,
                    options => options.MapFrom(e => e.Location.Country))
                .ReverseMap(); // same as mapping from CampModel to Camp; any chains after this will apply to CampModel to Camp mapping

            CreateMap<Talk, TalkModel>()
                .ReverseMap()
                .ForMember(e => e.Camp, options => options.Ignore())
                .ForMember(e => e.Camp, options => options.Ignore());

            CreateMap<Speaker, SpeakerModel>()
                .ReverseMap();

            //CreateMap<CampModel, Camp>()
            //    .ForPath(e => e.Location.VenueName,
            //        options => options.MapFrom(m => m.Venue))
            //    .ForPath(e => e.Location.Address1,
            //        options => options.MapFrom(m => m.Address1))
            //    .ForPath(m => m.Location.Address2,
            //        options => options.MapFrom(e => e.Address2))
            //    .ForPath(m => m.Location.Address3,
            //        options => options.MapFrom(e => e.Address3))
            //    .ForPath(m => m.Location.CityTown,
            //        options => options.MapFrom(e => e.CityTown))
            //    .ForPath(m => m.Location.StateProvince,
            //        options => options.MapFrom(e => e.StateProvince))
            //    .ForPath(m => m.Location.PostalCode,
            //        options => options.MapFrom(e => e.PostalCode))
            //    .ForPath(m => m.Location.Country,
            //        options => options.MapFrom(e => e.Country));

            //CreateMap<TalkModel, Talk>();
            
            //CreateMap<SpeakerModel, Speaker>();
        }
    }
}
