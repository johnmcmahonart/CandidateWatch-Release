﻿using Microsoft.AspNetCore.Http;
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
        private readonly IGetbyElectionYears<CandidateHistoryTotal> _FinanceTotalsRepository;
        // GET: api/<CandidateController>
        [HttpGet("{key}")]
        public async Task <IEnumerable<CandidateHistoryTotal>> GetbyKeyAsync(string key)

        {
            return await _FinanceTotalsRepository.GetbyKeyAsync(key);
        }


        [HttpGet("years")]
        public async Task<IEnumerable<CandidateHistoryTotal>> GetbyElectionYearAsync([FromQuery] List<int> years)
        {
            return await _FinanceTotalsRepository.GetbyElectionYearAsync(years);
        }


        public FinanceTotalsController(IGetbyElectionYears<CandidateHistoryTotal> financeTotalsRepository)
        {
            _FinanceTotalsRepository = financeTotalsRepository;

        }


    }
}
