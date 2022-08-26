using AutoMapper;
using MDWatch.Model;
using RESTApi.DTOs;

namespace RESTApi.Mapper
{

    public class CandidateMapperProfile : Profile
    {

        public CandidateMapperProfile()
        {
            CreateMap<Candidate, CandidateDTO>();
        }


    }
}
