using Microsoft.AspNetCore.Mvc;
using RESTApi.Repositories;
using RESTApi.DTOs;
using System.Collections.Generic;
using AutoMapper;
using MDWatch.Model;
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
            List<CandidateDTO> dtoOut = new();
            foreach (var item in modelOut)
            {
                dtoOut.Add(_mapper.Map<CandidateDTO>(item));
            }
            return dtoOut;
            
        }

                
        [HttpGet("years")]
        public async Task<IEnumerable<CandidateDTO>> GetbyElectionYearAsync([FromQuery]List<int> years)
        {
            IEnumerable<Candidate> modelOut = await _candidateRepository.GetbyElectionYearAsync(years);
            List<CandidateDTO> dtoOut = new();
            foreach (var item in modelOut)
            {
                dtoOut.Add(_mapper.Map<CandidateDTO>(item));
            }
            return dtoOut;

        }

        
        public CandidateController(ICandidateRepository<Candidate> candidateRepository,IMapper mapper )
        {
            _candidateRepository=candidateRepository;
            _mapper = mapper;
        }

    }
}
