using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MDWatch.Model;
using RESTApi.DTOs;
using RESTApi.Mapper;
using AutoMapper;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using RESTApi.Repositories;
//using System.Web.Http;

namespace RESTApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleBDetailController : ControllerBase
    {
        private IMapper _mapper;
        private readonly IScheduleBDetailRepository<ScheduleBByRecipientID> _scheduleBDetailRepository;
        // GET: api/<CandidateController>


        [HttpGet("{key}")]

        public async Task<IEnumerable<ScheduleBDetailDTO>> GetbyKeyAsync(string key)

        {
            IEnumerable<ScheduleBByRecipientID> modelOut = await _scheduleBDetailRepository.GetbyKeyAsync(key);
            return MapperHelper.MapIEnumerable<ScheduleBByRecipientID,ScheduleBDetailDTO>(modelOut, _mapper);
            
        }

        [HttpGet("{key}/years/")]
        
        public async Task<IEnumerable<ScheduleBDetailDTO>> GetbyCandidateandElectionYearsAsync([FromQuery] List<int> years, string key)
        {

            IEnumerable<ScheduleBByRecipientID> modelOut = await _scheduleBDetailRepository.GetbyCandidateandElectionYearsAsync(years,key);
            return MapperHelper.MapIEnumerable<ScheduleBByRecipientID, ScheduleBDetailDTO>(modelOut, _mapper);
            
        }

  
        [HttpGet("years")]

        public async Task<IEnumerable<ScheduleBDetailDTO>> GetbyElectionYearAsync([FromQuery] List<int> years)
        {
            IEnumerable<ScheduleBByRecipientID> modelOut = await _scheduleBDetailRepository.GetbyElectionYearsAsync(years);
            return MapperHelper.MapIEnumerable<ScheduleBByRecipientID, ScheduleBDetailDTO>(modelOut, _mapper);
            
        }


        public ScheduleBDetailController(IScheduleBDetailRepository<ScheduleBByRecipientID> scheduleBDetailRepository, IMapper mapper)
        {
            _mapper = mapper;
            _scheduleBDetailRepository = scheduleBDetailRepository;

        }

    }
}
