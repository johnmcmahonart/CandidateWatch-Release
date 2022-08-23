using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RESTApi.Repositories;
using System.Collections.Generic;
using AutoMapper;
using RESTApi.DTOs;
using MDWatch.Model;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace RESTApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinanceTotalsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IFinanceTotalsRepository<CandidateHistoryTotal> _financeTotalsRepository;
        // GET: api/<CandidateController>
        [HttpGet("{key}")]
        public async Task <IEnumerable<FinanceTotalsDTO>> GetbyKeyAsync(string key)

        {
            IEnumerable<CandidateHistoryTotal> modelOut = await _financeTotalsRepository.GetbyKeyAsync(key);
            List<FinanceTotalsDTO> dtoOut = new();
            foreach (var item in modelOut)
            {
                dtoOut.Add(_mapper.Map<FinanceTotalsDTO>(item));
            }
            return dtoOut;
            
        }


        [HttpGet("{key}/years/")]

        public async Task<IEnumerable<FinanceTotalsDTO>> GetbyCandidateandElectionYearsAsync([FromQuery] List<int> years, string key)
        {

            IEnumerable<CandidateHistoryTotal> modelOut = await _financeTotalsRepository.GetbyCandidateandElectionYearsAsync(years, key);
            List<FinanceTotalsDTO> dtoOut = new();
            foreach (var item in modelOut)
            {
                dtoOut.Add(_mapper.Map<FinanceTotalsDTO>(item));
            }
            return dtoOut;
            
        }
        [HttpGet("years")]
        public async Task<IEnumerable<FinanceTotalsDTO>> GetbyElectionYearAsync([FromQuery] List<int> years)
        {
            IEnumerable<CandidateHistoryTotal> modelOut = await _financeTotalsRepository.GetbyElectionYearAsync(years);
            List<FinanceTotalsDTO> dtoOut = new();
            foreach (var item in modelOut)
            {
                dtoOut.Add(_mapper.Map<FinanceTotalsDTO>(item));
            }
            return dtoOut;
            
        }


        public FinanceTotalsController(IFinanceTotalsRepository<CandidateHistoryTotal> financeTotalsRepository, IMapper mapper)
        {
            _financeTotalsRepository = financeTotalsRepository;
            _mapper = mapper;
        }


    }
}
