using Microsoft.AspNetCore.Mvc.ModelBinding;
using AutoMapper;
using MDWatch.Model;
using Microsoft.AspNetCore.Mvc;
using RESTApi.DTOs;
using RESTApi.Mapper;
using RESTApi.Repositories;
using Newtonsoft.Json.Linq;

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
            return _mapper.Map<IEnumerable<ScheduleBByRecipientID>, IEnumerable<ScheduleBDetailDTO>>(modelOut);
            
        }

        [HttpGet("{year}/keys")]
        public async Task<IEnumerable<IEnumerable<ScheduleBDetailDTO>>> GetbyKeysandElectionYearAsync([FromQuery] String keys, int year)

        {
            List<string> keysResult = keys.Split(',').ToList();

            IEnumerable<IEnumerable<ScheduleBByRecipientID>> modelOut = await _scheduleBDetailRepository.GetbyKeysandElectionYearAsync(keysResult, year);
            List<List<ScheduleBDetailDTO>> outlist = new();
            return _mapper.Map<IEnumerable<IEnumerable<ScheduleBByRecipientID>>, IEnumerable<IEnumerable<ScheduleBDetailDTO>>>(modelOut);
            
        }
        [HttpGet("keys")]
        public async Task<IEnumerable<IEnumerable<ScheduleBDetailDTO>>> GetbyKeysAsync(string keys)

        {
            List<string> keysResult = keys.Split(',').ToList();
            IEnumerable<IEnumerable<ScheduleBByRecipientID>> modelOut = await _scheduleBDetailRepository.GetbyKeysAsync(keysResult);
            return _mapper.Map<IEnumerable<IEnumerable<ScheduleBByRecipientID>>, IEnumerable<IEnumerable<ScheduleBDetailDTO>>>(modelOut);
            
        }

        [HttpGet("{key}/years/")]
    public async Task<IEnumerable<ScheduleBDetailDTO>> GetbyCandidateandElectionYearsAsync([FromQuery] string years, string key)
    {
            List<int> yearsResult = years.Split(',').Select(int.Parse).ToList();
            
            IEnumerable<ScheduleBByRecipientID> modelOut = await _scheduleBDetailRepository.GetbyCandidateandElectionYearsAsync(yearsResult, key);
            return _mapper.Map<IEnumerable<ScheduleBByRecipientID>, IEnumerable<ScheduleBDetailDTO>>(modelOut);
            

    }

    [HttpGet("years")]
    public async Task<IEnumerable<ScheduleBDetailDTO>> GetbyElectionYearAsync([FromQuery] List<int> years)
    {
            List<int> yearsResult = years.Split(',').Select(int.Parse).ToList();
            IEnumerable<ScheduleBByRecipientID> modelOut = await _scheduleBDetailRepository.GetbyElectionYearsAsync(yearsResult);
            return _mapper.Map<IEnumerable<ScheduleBByRecipientID>, IEnumerable<ScheduleBDetailDTO>>(modelOut);
            
    }

    public ScheduleBDetailController(IScheduleBDetailRepository<ScheduleBByRecipientID> scheduleBDetailRepository, IMapper mapper)
    {
        _mapper = mapper;
        _scheduleBDetailRepository = scheduleBDetailRepository;
    }
}
}