using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RESTApi.Mapper;
using System.Collections.Generic;
using RESTApi.DTOs;
using MDWatch.Model;
using AutoMapper;
using RESTApi.Repositories;
using System.ComponentModel.DataAnnotations;

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
        public async Task <IEnumerable<FinanceTotalsDTO>> GetbyKeyAsync([Required] string key, [Required] string state)

        {
            _financeTotalsRepository.SetState(state);
            IEnumerable<CandidateHistoryTotal> modelOut = await _financeTotalsRepository.GetbyKeyAsync(key);
            return _mapper.Map<IEnumerable<CandidateHistoryTotal>, IEnumerable<FinanceTotalsDTO>>(modelOut);
            
        }

        [HttpGet("keys")]
        public async Task<IEnumerable<IEnumerable<FinanceTotalsDTO>>> GetbyKeysAsync([StringArrayBinder][Required] List<string> keys, [Required] string state)

        {
            _financeTotalsRepository.SetState(state);
            IEnumerable<IEnumerable<CandidateHistoryTotal>> modelOut = await _financeTotalsRepository.GetbyKeysAsync(keys);
            return _mapper.Map<IEnumerable<IEnumerable<CandidateHistoryTotal>>, IEnumerable<IEnumerable<FinanceTotalsDTO>>>(modelOut);

        }
        [HttpGet("{year}/keys")]
        public async Task<IEnumerable<IEnumerable<FinanceTotalsDTO>>> GetbyKeysandElectionYearAsync([StringArrayBinder][Required] List<string> keys, [Required] int year, [Required] string state)

        {
            _financeTotalsRepository.SetState(state);
            IEnumerable<IEnumerable<CandidateHistoryTotal>> modelOut = await _financeTotalsRepository.GetbyKeysandElectionYearAsync(keys, year);
            List<List<ScheduleBDetailDTO>> outlist = new();
            return _mapper.Map<IEnumerable<IEnumerable<CandidateHistoryTotal>>, IEnumerable<IEnumerable<FinanceTotalsDTO>>>(modelOut);

        }

        [HttpGet("{key}/years/")]
        public async Task<IEnumerable<FinanceTotalsDTO>> GetbyCandidateandElectionYearsAsync([FromQuery][Required] List<int> years, [Required] string key, [Required] string state)
        {
            _financeTotalsRepository.SetState(state);
            IEnumerable<CandidateHistoryTotal> modelOut = await _financeTotalsRepository.GetbyCandidateandElectionYearsAsync(years, key);
            return _mapper.Map<IEnumerable<CandidateHistoryTotal>, IEnumerable<FinanceTotalsDTO>>(modelOut);


        }

        
        [HttpGet("years")]
        public async Task<IEnumerable<FinanceTotalsDTO>> GetbyElectionYearAsync([FromQuery][Required] List<int> years, [Required] string state)
        {
            _financeTotalsRepository.SetState(state);
            
            
            IEnumerable<CandidateHistoryTotal> modelOut = await _financeTotalsRepository.GetbyElectionYearsAsync(years);
            return _mapper.Map<IEnumerable<CandidateHistoryTotal>, IEnumerable<FinanceTotalsDTO>>(modelOut);
            
            
        }


        public FinanceTotalsController(IFinanceTotalsRepository<CandidateHistoryTotal> financeTotalsRepository, IMapper mapper)
        {
            _mapper = mapper;
            _financeTotalsRepository = financeTotalsRepository;

        }


    }
}
