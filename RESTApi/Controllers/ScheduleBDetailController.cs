using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MDWatch.Model;
using RESTApi.Repositories;
using RESTApi.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
//using System.Web.Http;

namespace RESTApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleBDetailController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IScheduleBDetailRepository<ScheduleBByRecipientID> _scheduleBDetailRepository;
        // GET: api/<CandidateController>


        [HttpGet("{key}")]

        public async Task<IEnumerable<ScheduleBDetailDTO>> GetbyKeyAsync(string key)

        {
            IEnumerable<ScheduleBByRecipientID> modelOut = await _scheduleBDetailRepository.GetbyKeyAsync(key);
            List<ScheduleBDetailDTO> dtoOut = new();
            foreach (var item in modelOut)
            {
                dtoOut.Add(_mapper.Map<ScheduleBDetailDTO>(item));
            }
            return dtoOut;
            
        }

        [HttpGet("{key}/years/")]
        
        public async Task<IEnumerable<ScheduleBDetailDTO>> GetbyCandidateandElectionYearsAsync([FromQuery] List<int> years, string key)
        {

            IEnumerable<ScheduleBByRecipientID> modelOut = await _scheduleBDetailRepository.GetbyCandidateandElectionYearsAsync(years,key);
            List<ScheduleBDetailDTO> dtoOut = new();
            foreach (var item in modelOut)
            {
                dtoOut.Add(_mapper.Map<ScheduleBDetailDTO>(item));
            }
            return dtoOut;
            
        }

  
        [HttpGet("years")]

        public async Task<IEnumerable<ScheduleBDetailDTO>> GetbyElectionYearAsync([FromQuery] List<int> years)
        {
            IEnumerable<ScheduleBByRecipientID> modelOut = await _scheduleBDetailRepository.GetbyElectionYearAsync(years);
            List<ScheduleBDetailDTO> dtoOut = new();
            foreach (var item in modelOut)
            {
                dtoOut.Add(_mapper.Map<ScheduleBDetailDTO>(item));
            }
            return dtoOut;
            
        }


        public ScheduleBDetailController(IScheduleBDetailRepository<ScheduleBByRecipientID> scheduleBDetailRepository, IMapper mapper)
        {
            _scheduleBDetailRepository = scheduleBDetailRepository;
            _mapper=mapper;
        }

    }
}
