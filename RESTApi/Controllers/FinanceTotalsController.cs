using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RESTApi.Mapper;
using System.Collections.Generic;
using RESTApi.DTOs;
using MDWatch.Model;
using AutoMapper;
using RESTApi.Repositories;

namespace RESTApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinanceTotalsController : ControllerBase
    {
        private IMapper _mapper;
        private readonly IFinanceTotalsRepository<CandidateHistoryTotal> _financeTotalsRepository;
        // GET: api/<CandidateController>
        [HttpGet("{key}")]
        public async Task <IEnumerable<FinanceTotalsDTO>> GetbyKeyAsync(string key)

        {
            IEnumerable<CandidateHistoryTotal> modelOut = await _financeTotalsRepository.GetbyKeyAsync(key);
            return MapperHelper.MapIEnumerable<CandidateHistoryTotal, FinanceTotalsDTO>(modelOut, _mapper);
        }


        [HttpGet("{key}/years/")]

        public async Task<IEnumerable<CandidateHistoryTotal>> GetbyCandidateandElectionYearsAsync([FromQuery] List<int> years, string key)
        {

            return await _financeTotalsRepository.GetbyCandidateandElectionYearsAsync(years, key);
        }
        [HttpGet("years")]
        public async Task<IEnumerable<CandidateHistoryTotal>> GetbyElectionYearAsync([FromQuery] List<int> years)
        {
            return await _financeTotalsRepository.GetbyElectionYearsAsync(years);
        }


        public FinanceTotalsController(IFinanceTotalsRepository<CandidateHistoryTotal> financeTotalsRepository, IMapper mapper)
        {
            _mapper = mapper;
            _financeTotalsRepository = financeTotalsRepository;

        }


    }
}
