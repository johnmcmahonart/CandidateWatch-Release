using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MDWatch.Model;
using RESTApi.Mapper;
using RESTApi.Repositories;
namespace RESTApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleBOverviewController : ControllerBase
    {
        private readonly IScheduleBOverviewRepository<ScheduleBCandidateOverview> _scheduleBOverviewRepository;
        // GET: api/<CandidateController>
        [HttpGet("{key}")]
        public async Task<IEnumerable<ScheduleBCandidateOverview>> GetbyKeyAsync(string key)

        {
            return await _scheduleBOverviewRepository.GetbyKeyAsync(key);
        }

        //https://stackoverflow.com/questions/9508265/how-do-i-accept-an-array-as-an-asp-net-mvc-controller-action-parameter
        [HttpGet("keys")]
        public async Task<IEnumerable<IEnumerable<ScheduleBCandidateOverview>>> GetbyKeysAsync([StringArrayBinder]List<string> keys)

        {
            return await _scheduleBOverviewRepository.GetbyKeysAsync(keys);
        }

        public ScheduleBOverviewController(IScheduleBOverviewRepository<ScheduleBCandidateOverview> scheduleBOverviewRepository)
        {
            _scheduleBOverviewRepository = scheduleBOverviewRepository;

        }

    }

}

