using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MDWatch.Model;
using RESTApi.Repositories;
//using System.Web.Http;

namespace RESTApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleBDetailController : ControllerBase
    {
        private readonly IScheduleBDetailRepository<ScheduleBByRecipientID> _scheduleBDetailRepository;
        // GET: api/<CandidateController>


        [HttpGet("{key}")]

        public async Task<IEnumerable<ScheduleBByRecipientID>> GetbyKeyAsync(string key)

        {
            return await _scheduleBDetailRepository.GetbyKeyAsync(key);
        }

        [HttpGet("{key}/years/")]
        
        public async Task<IEnumerable<ScheduleBByRecipientID>> GetbyCandidateandElectionYearsAsync([FromQuery] List<int> years, string key)
        {
            
            return await _scheduleBDetailRepository.GetbyCandidateandElectionYearsAsync(years,key);
        }

  
        [HttpGet("years")]

        public async Task<IEnumerable<ScheduleBByRecipientID>> GetbyElectionYearAsync([FromQuery] List<int> years)
        {
            return await _scheduleBDetailRepository.GetbyElectionYearAsync(years);
        }


        public ScheduleBDetailController(IScheduleBDetailRepository<ScheduleBByRecipientID> scheduleBDetailRepository)
        {
            _scheduleBDetailRepository = scheduleBDetailRepository;

        }

    }
}
