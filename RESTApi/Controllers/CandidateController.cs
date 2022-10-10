 using Microsoft.AspNetCore.Mvc;
using RESTApi.Mapper;
using RESTApi.DTOs;
using System.Collections.Generic;
using AutoMapper;
using MDWatch.Model;
using RESTApi.Repositories;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RESTApi.Controllers
{
    [Route("api/Candidate")]
    [ApiController]
    public class CandidateController : ControllerBase
    {
        private IMapper _mapper;
        private readonly ICandidateRepository<Candidate> _candidateRepository;
        // GET: api/<CandidateController>
        [HttpGet("{key}")]
        public async Task<IEnumerable<CandidateDTO>> GetbyKeyAsync(string key)

        {
            IEnumerable<Candidate> modelOut = await _candidateRepository.GetbyKeyAsync(key);
            return _mapper.Map<IEnumerable<Candidate>, IEnumerable<CandidateDTO>>(modelOut);
                
            
            
        }

                
        [HttpGet("years")]
        public async Task<IEnumerable<CandidateDTO>> GetbyElectionYearAsync([FromQuery]List<int> years)
        {
            IEnumerable<Candidate> modelOut = await _candidateRepository.GetbyElectionYearsAsync(years);
            return _mapper.Map<IEnumerable<Candidate>, IEnumerable<CandidateDTO>>(modelOut);


        }

        
        public CandidateController(ICandidateRepository<Candidate> candidateRepository,IMapper mapper )
        {
            _candidateRepository= candidateRepository;
            _mapper = mapper;
        }

    }
}
