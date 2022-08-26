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
        private readonly IRepository<ScheduleBCandidateOverview> _scheduleBOverviewRepository;
        // GET: api/<CandidateController>
        [HttpGet("{key}")]
        public async Task<IEnumerable<ScheduleBCandidateOverview>> GetbyKeyAsync(string key)

        {
            return await _scheduleBOverviewRepository.GetbyKeyAsync(key);
        }


        
        public ScheduleBOverviewController(IRepository<ScheduleBCandidateOverview> scheduleBOverviewRepository)
        {
            _scheduleBOverviewRepository = scheduleBOverviewRepository;

        }

    }

}

