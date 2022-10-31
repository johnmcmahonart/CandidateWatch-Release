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
        public async Task<IEnumerable<ScheduleBDetailDTO>> GetbyKeyAsync(string key)

        {
            IEnumerable<ScheduleBByRecipientID> modelOut = await _scheduleBDetailRepository.GetbyKeyAsync(key);
            return _mapper.Map<IEnumerable<ScheduleBByRecipientID>, IEnumerable<ScheduleBDetailDTO>>(modelOut);
            
        }

        [HttpGet("{year}/keys")]
        public async Task<IEnumerable<IEnumerable<ScheduleBDetailDTO>>> GetbyKeysandElectionYearAsync([StringArrayBinder] List<string> keys, int year)

        {
            IEnumerable<IEnumerable<ScheduleBByRecipientID>> modelOut = await _scheduleBDetailRepository.GetbyKeysandElectionYearAsync(keys, year);
            List<List<ScheduleBDetailDTO>> outlist = new();
            return _mapper.Map<IEnumerable<IEnumerable<ScheduleBByRecipientID>>, IEnumerable<IEnumerable<ScheduleBDetailDTO>>>(modelOut);
            
        }
        [HttpGet("keys")]
        public async Task<IEnumerable<IEnumerable<ScheduleBDetailDTO>>> GetbyKeysAsync([StringArrayBinder] List<string> keys)

        {
            IEnumerable<IEnumerable<ScheduleBByRecipientID>> modelOut = await _scheduleBDetailRepository.GetbyKeysAsync(keys);
            return _mapper.Map<IEnumerable<IEnumerable<ScheduleBByRecipientID>>, IEnumerable<IEnumerable<ScheduleBDetailDTO>>>(modelOut);
            
        }

        [HttpGet("{key}/years/")]
    public async Task<IEnumerable<ScheduleBDetailDTO>> GetbyCandidateandElectionYearsAsync([FromQuery] List<int> years, string key)
    {
        IEnumerable<ScheduleBByRecipientID> modelOut = await _scheduleBDetailRepository.GetbyCandidateandElectionYearsAsync(years, key);
            return _mapper.Map<IEnumerable<ScheduleBByRecipientID>, IEnumerable<ScheduleBDetailDTO>>(modelOut);
            

    }

    [HttpGet("years")]
    public async Task<IEnumerable<ScheduleBDetailDTO>> GetbyElectionYearAsync([FromQuery] List<int> years)
    {
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