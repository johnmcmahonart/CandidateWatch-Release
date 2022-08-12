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


        [HttpGet]
        public async Task<IEnumerable<Candidate>> GetbyAllAsync()

        {
            return await _candidateRepository.GetAllAsync();
        }
        [HttpGet("Cycle")]
        public async Task<IEnumerable<Candidate>> GetbyCycleAsync([FromQuery]int[] cycles)
        {
            return await _candidateRepository.GetbyCycleAsync(cycles);
        }

        // POST api/<CandidateController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CandidateController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CandidateController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        public CandidateController(ICandidateRepository<Candidate> candidateRepository)
        {
            _candidateRepository=candidateRepository;        

        }

    }
}
