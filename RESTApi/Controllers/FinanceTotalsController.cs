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

        public async Task<IEnumerable<FinanceTotalsDTO>> GetbyCandidateandElectionYearsAsync([FromQuery] List<int> years, string key)
        {

            IEnumerable<CandidateHistoryTotal> modelOut =await _financeTotalsRepository.GetbyCandidateandElectionYearsAsync(years, key);
            return MapperHelper.MapIEnumerable<CandidateHistoryTotal, FinanceTotalsDTO>(modelOut, _mapper);
        }
        [HttpGet("years")]
        public async Task<IEnumerable<FinanceTotalsDTO>> GetbyElectionYearAsync([FromQuery] List<int> years)
        {
            
            IEnumerable<CandidateHistoryTotal> modelOut = await _financeTotalsRepository.GetbyElectionYearsAsync(years);
            return MapperHelper.MapIEnumerable<CandidateHistoryTotal, FinanceTotalsDTO>(modelOut, _mapper);
            
        }


        public FinanceTotalsController(IFinanceTotalsRepository<CandidateHistoryTotal> financeTotalsRepository, IMapper mapper)
        {
            _mapper = mapper;
            _financeTotalsRepository = financeTotalsRepository;

        }


    }
}
