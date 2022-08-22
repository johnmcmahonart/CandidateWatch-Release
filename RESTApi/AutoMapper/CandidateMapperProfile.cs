using AutoMapper;
using MDWatch.Model;
using RESTApi.DTOs;

namespace RESTApi.AutoMapper
{

    public class CandidateMapperProfile : Profile
    {

        public CandidateMapperProfile()
        {
            CreateMap<Candidate, CandidateDTO>();
        }


    }
}
