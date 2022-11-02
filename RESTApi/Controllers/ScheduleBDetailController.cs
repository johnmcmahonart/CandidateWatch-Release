using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MDWatch.Model;
using Microsoft.AspNetCore.Mvc;
using RESTApi.DTOs;
using RESTApi.Mapper;
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
        public async Task<IEnumerable<ScheduleBDetailDTO>> GetbyKeyAsync([Required] string key, [Required] string state)

        {
            _scheduleBDetailRepository.SetState(state);
            IEnumerable<ScheduleBByRecipientID> modelOut = await _scheduleBDetailRepository.GetbyKeyAsync(key);
            return _mapper.Map<IEnumerable<ScheduleBByRecipientID>, IEnumerable<ScheduleBDetailDTO>>(modelOut);
            
        }

        [HttpGet("{year}/keys")]
        public async Task<IEnumerable<IEnumerable<ScheduleBDetailDTO>>> GetbyKeysandElectionYearAsync([StringArrayBinder][Required] List<string> keys, [Required] int year, [Required] string state)

        {
            _scheduleBDetailRepository.SetState(state);
            IEnumerable<IEnumerable<ScheduleBByRecipientID>> modelOut = await _scheduleBDetailRepository.GetbyKeysandElectionYearAsync(keys, year);
            List<List<ScheduleBDetailDTO>> outlist = new();
            return _mapper.Map<IEnumerable<IEnumerable<ScheduleBByRecipientID>>, IEnumerable<IEnumerable<ScheduleBDetailDTO>>>(modelOut);
            
        }
        [HttpGet("keys")]
        public async Task<IEnumerable<IEnumerable<ScheduleBDetailDTO>>> GetbyKeysAsync([StringArrayBinder][Required] List<string> keys, [Required] string state)

        {
            _scheduleBDetailRepository.SetState(state);
            IEnumerable<IEnumerable<ScheduleBByRecipientID>> modelOut = await _scheduleBDetailRepository.GetbyKeysAsync(keys);
            return _mapper.Map<IEnumerable<IEnumerable<ScheduleBByRecipientID>>, IEnumerable<IEnumerable<ScheduleBDetailDTO>>>(modelOut);
            
        }

        [HttpGet("{key}/years/")]
    public async Task<IEnumerable<ScheduleBDetailDTO>> GetbyCandidateandElectionYearsAsync([FromQuery][Required] List<int> years, [Required] string key, [Required] string state)
    {
            _scheduleBDetailRepository.SetState(state);
            IEnumerable<ScheduleBByRecipientID> modelOut = await _scheduleBDetailRepository.GetbyCandidateandElectionYearsAsync(years, key);
            return _mapper.Map<IEnumerable<ScheduleBByRecipientID>, IEnumerable<ScheduleBDetailDTO>>(modelOut);
            

    }

    [HttpGet("years")]
    public async Task<IEnumerable<ScheduleBDetailDTO>> GetbyElectionYearAsync([FromQuery][Required] List<int> years, [Required] string state)
    {
            _scheduleBDetailRepository.SetState(state);
            IEnumerable<ScheduleBByRecipientID> modelOut = await _scheduleBDetailRepository.GetbyElectionYearsAsync(years);
            return _mapper.Map<IEnumerable<ScheduleBByRecipientID>, IEnumerable<ScheduleBDetailDTO>>(modelOut);
            
    }

    public ScheduleBDetailController(IScheduleBDetailRepository<ScheduleBByRecipientID> scheduleBDetailRepository, IMapper mapper)
    {
        _mapper = mapper;
        _scheduleBDetailRepository = scheduleBDetailRepository;
    }
}
}