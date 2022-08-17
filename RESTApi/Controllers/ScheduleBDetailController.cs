using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MDWatch.Model;
using RESTApi.Repositories;
namespace RESTApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleBDetailController : ControllerBase
    {
        private readonly IGetbyElectionYears<ScheduleBByRecipientID> _scheduleBDetailRepository;
        // GET: api/<CandidateController>
        [HttpGet("{key}")]
        public async Task<IEnumerable<ScheduleBByRecipientID>> GetbyKeyAsync(string key)

        {
            return await _scheduleBDetailRepository.GetbyKeyAsync(key);
        }


        [HttpGet("years")]
        public async Task<IEnumerable<ScheduleBByRecipientID>> GetbyElectionYearAsync([FromQuery] List<int> years)
        {
            return await _scheduleBDetailRepository.GetbyElectionYearAsync(years);
        }


        public ScheduleBDetailController(IGetbyElectionYears<ScheduleBByRecipientID> scheduleBDetailRepository)
        {
            _scheduleBDetailRepository = scheduleBDetailRepository;

        }

    }
}
