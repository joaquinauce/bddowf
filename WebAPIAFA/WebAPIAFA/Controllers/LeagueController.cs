using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPIAFA.Helpers;
using WebAPIAFA.Helpers.Enum;
using WebAPIAFA.Models.Dtos.LeagueDtos;
using WebAPIAFA.Services.IServices;

namespace WebAPIAFA.Controllers
{
    [ApiController]
    [Route("api/league")]
    [Authorize]
    public class LeagueController: ControllerBase
    {
        public ILeagueService iLeagueService;

        public LeagueController(ILeagueService leagueService)
        {
            iLeagueService = leagueService;
        }

        [HttpPost]
        public async Task<ActionResult> CreateLeague([FromForm] LeaguePostDto leaguePostDto)
        {
            ResponseObjectJsonDto response = await iLeagueService.CreateLeague(leaguePostDto);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }
        [HttpPatch]
        public async Task<ActionResult> UpdateLeague([FromForm] LeaguePatchDto leaguePatchDto)
        {
            ResponseObjectJsonDto response = await iLeagueService.UpdateLeague(leaguePatchDto);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("players/{idLeague:int}")]
        public async Task<ActionResult> GetPlayersByLeague(int idLeague)
        {
            ResponseObjectJsonDto response = await iLeagueService.GetPlayersByLeague(idLeague);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("/responsibleLeague")]
        public async Task<ActionResult> GetLeagueByResponsibleId()
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;

            ResponseObjectJsonDto response = await iLeagueService.GetLeagueByResponsibleId(encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("image")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetImageLeague()
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;
            ResponseObjectJsonDto response = await iLeagueService.GetImageLeague(encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("select")]
        public async Task<ActionResult> GetLeagueSelect()
        {
            ResponseObjectJsonDto response = await iLeagueService.GetLeagueSelect();

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }
    }
}
