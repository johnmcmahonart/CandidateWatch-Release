using AutoMapper;
using MDWatch.Model;
using RESTApi.DTOs;

namespace RESTApi.Mapper
{
    public class FinanceTotalsMapperProfile : Profile
    {
        public FinanceTotalsMapperProfile()
        {
            CreateMap<CandidateHistoryTotal, FinanceTotalsDTO>();
        }
    }
}