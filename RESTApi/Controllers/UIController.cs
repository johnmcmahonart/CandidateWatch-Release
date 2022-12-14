using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RESTApi.DTOs;
using RESTApi.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RESTApi.Controllers
{
    
    [Route("api/UI")]
    [ApiController]
    public class UIController : ControllerBase
    {
        private readonly IUINavRepository _UIRepository;

        [HttpGet("CandidatesbyYear/{year}")]
        public async Task<IEnumerable<CandidateUIDTO>> GetCandidates([FromQuery] bool wasElected, [Required] int year, [Required] string state)

        {
            _UIRepository.SetState(state);
            return await _UIRepository.GetCandidates(year, wasElected);
        }

        [HttpGet("ElectionYears")]
        public async Task<IEnumerable<int>> GetElectionYearsAsync([Required] string state)
        {

            _UIRepository.SetState(state);
            return await _UIRepository.GetElectionYears();
        }

        public UIController(IUINavRepository uiRepository)
        {
            _UIRepository = uiRepository;
        }
    }
}