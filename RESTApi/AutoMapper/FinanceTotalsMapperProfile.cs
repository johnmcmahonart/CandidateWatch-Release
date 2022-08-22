using AutoMapper;
using MDWatch.Model;
using RESTApi.DTOs;

namespace RESTApi.AutoMapper
{
    public class FinanceTotalsMapperProfile : Profile
    {
        public FinanceTotalsMapperProfile()
        {
            CreateMap<CandidateHistoryTotal, FinanceTotalsDTO>();
        }
    }
}