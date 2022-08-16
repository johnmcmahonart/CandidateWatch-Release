using Microsoft.AspNetCore.Mvc;
using RESTApi.Repositories;
using System.Collections.Generic;
using MDWatch.Model;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RESTApi.Controllers
{
    [Route("api/Candidate")]
    [ApiController]
    public class CandidateController : ControllerBase
    {
        private readonly ICandidateRepository<Candidate> _candidateRepository;
        // GET: api/<CandidateController>
        [HttpGet("{key}")]
        public async Task<Candidate> GetbyKeyAsync(string key)

        {
            return await _candidateRepository.GetbyKeyAsync(key);
        }

                
        [HttpGet("years")]
        public async Task<IEnumerable<Candidate>> GetbyElectionYearAsync([FromQuery]List<int> years)
        {
            return await _candidateRepository.GetbyElectionYearAsync(years);
        }

        
        public CandidateController(ICandidateRepository<Candidate> candidateRepository)
        {
            _candidateRepository=candidateRepository;        

        }

    }
}
