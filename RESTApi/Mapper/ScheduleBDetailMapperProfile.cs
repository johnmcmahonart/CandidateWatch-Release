using AutoMapper;
using MDWatch.Model;
using RESTApi.DTOs;

namespace RESTApi.Mapper
{
    public class ScheduleBDetailMapperProfile : Profile
    {
        public ScheduleBDetailMapperProfile()
        {
            CreateMap<ScheduleBByRecipientID, ScheduleBDetailDTO>();
        }
    }
}
