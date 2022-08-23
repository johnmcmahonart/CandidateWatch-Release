using AutoMapper;
using MDWatch.Model;
using RESTApi.DTOs;

namespace RESTApi.MapperProfiles
{
    public class FinanceTotalsMapperProfile : Profile
    {
        public FinanceTotalsMapperProfile()
        {
            CreateMap<CandidateHistoryTotal, FinanceTotalsDTO>();
        }
    }
}