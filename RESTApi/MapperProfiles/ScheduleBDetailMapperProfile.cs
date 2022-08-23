using AutoMapper;
using MDWatch.Model;
using RESTApi.DTOs;

namespace RESTApi.MapperProfiles
{
    public class ScheduleBDetailMapperProfile : Profile
    {
        public ScheduleBDetailMapperProfile()
        {
            CreateMap<ScheduleBByRecipientID, ScheduleBDetailDTO>();
        }
    }
}
