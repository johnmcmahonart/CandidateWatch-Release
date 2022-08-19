using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RESTApi.Repositories;
using System.Collections.Generic;
using MDWatch.Model;

namespace RESTApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinanceTotalsController : ControllerBase
    {
        private readonly IFinanceTotalsRepository<CandidateHistoryTotal> _financeTotalsRepository;
        // GET: api/<CandidateController>
        [HttpGet("{key}")]
        public async Task <IEnumerable<CandidateHistoryTotal>> GetbyKeyAsync(string key)

        {
            return await _financeTotalsRepository.GetbyKeyAsync(key);
        }


        [HttpGet("{key}/years/")]

        public async Task<IEnumerable<CandidateHistoryTotal>> GetbyCandidateandElectionYearsAsync([FromQuery] List<int> years, string key)
        {

            return await _financeTotalsRepository.GetbyCandidateandElectionYearsAsync(years, key);
        }
        [HttpGet("years")]
        public async Task<IEnumerable<CandidateHistoryTotal>> GetbyElectionYearAsync([FromQuery] List<int> years)
        {
            return await _financeTotalsRepository.GetbyElectionYearAsync(years);
        }


        public FinanceTotalsController(IFinanceTotalsRepository<CandidateHistoryTotal> financeTotalsRepository)
        {
            _financeTotalsRepository = financeTotalsRepository;

        }


    }
}
