using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPIAFA.Helpers;
using WebAPIAFA.Helpers.Enum;
using WebAPIAFA.Models;
using WebAPIAFA.Models.Dtos.ClubAuthorityDtos;
using WebAPIAFA.Models.Dtos.ClubDtos;
using WebAPIAFA.Models.Dtos.ClubInformationDtos;
using WebAPIAFA.Models.Dtos.ClubSponsorsDto;
using WebAPIAFA.Services.IServices;

namespace WebAPIAFA.Controllers
{
    [ApiController]
    [Route("api/club")]
    [Authorize]
    public class ClubController: ControllerBase
    {
        private readonly IClubService iClubService;
        private readonly IClubAuthorityService iClubAuthorityService;


        public ClubController(IClubService iClubService, IClubAuthorityService iClubAuthorityService)
        {
            this.iClubService = iClubService;
            this.iClubAuthorityService = iClubAuthorityService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> CreateClub([FromForm] ClubCreateDto clubDto)
        {
            ResponseObjectJsonDto response = await iClubService.CreateClub(clubDto);
            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("clubAuthority")]
        public async Task<ActionResult> CreateClubAuthority([FromForm] ClubAuthorityCreateDto clubAuthorityDto)
        {
            ResponseObjectJsonDto response = await iClubAuthorityService.CreateClubAuthority(clubAuthorityDto);
            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return StatusCode(response.Code, response);
        }

        [HttpPost]
        [Route("sponsor")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseObjectJsonDto>> CreateClubSponsor(int idClub, [FromBody] List<ClubSponsorPostDto> clubSponsorDto)
        {
            ResponseObjectJsonDto response = await iClubService.CreateClubSponsor(idClub, clubSponsorDto);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);

        }

        [HttpPost("information")]
        public async Task<ActionResult> CreateClubInformation([FromForm] ClubInformationCreateDto clubInfoDto)
        {
            ResponseObjectJsonDto response = await iClubService.CreateClubInformation(clubInfoDto);
            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("clubResponsible")]
        public async Task<ActionResult> GetClubByResponsibleId()
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;

            ResponseObjectJsonDto response = await iClubService.GetClubByResponsibleId(encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet("select")]
        public async Task<ActionResult> GetClubs()
        {
            ResponseObjectJsonDto response = await iClubService.GetClubs();

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }

        [HttpGet("LeagueClubs")]
        public async Task<ActionResult> GetLeagueClubs()
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;

            ResponseObjectJsonDto response = await iClubService.GetLeagueClubs(encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet("authorities/{idClub:int}")]
        public async Task<IActionResult> GetClubAuthoritiesByIdClub(int idClub)
        {
            ResponseObjectJsonDto response = await iClubAuthorityService.GetMandateAuthorities(idClub);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("tournament/division")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetTournamentDivisions()
        {
            ResponseObjectJsonDto response = await iClubService.GetTournamentDivisions();

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);

        }

        [HttpGet]
        [Route("tournament/division/{idTournamentDivision:int}")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetClubsByTournamentDivision(int idTournamentDivision)
        {
            ResponseObjectJsonDto response = await iClubService.GetClubsByTournamentDivision(idTournamentDivision);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet("info/{idClub:int}")]
        public async Task<IActionResult> GetClubInfoById(int idClub)
        {
            ResponseObjectJsonDto response = await iClubService.GetClubInfoById(idClub);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet("stadium/{idClub:int}")]
        public async Task<IActionResult> GetClubStadiumInfoById(int idClub)
        {
            ResponseObjectJsonDto response = await iClubService.GetClubStadiumInfoById(idClub);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet("statute/{idClub:int}")]
        public async Task<IActionResult> GetClubStatuteById(int idClub)
        {
            FileStream file = await iClubService.GetClubStatuteById(idClub);

            if (file == null)
            {
                return NotFound();
            }

            return File(file, "application/pdf");
        }


        [HttpGet]
        [Route("staff")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetClubStaffs()
        {
            ResponseObjectJsonDto response = await iClubService.GetClubStaffs();

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);

        }

        [HttpGet]
        [Route("staffByClub/{idClub:int}")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetClubStaffByIdClub(int idClub)
        {
            ResponseObjectJsonDto response = await iClubService.GetClubStaffsByIdClub(idClub);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }        

        [HttpGet]
        [Route("image")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetImageClub()
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;
            ResponseObjectJsonDto response = await iClubService.GetImageClub(encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet("{idLeague:int}")]
        public async Task<IActionResult> GetClubsByLeague(int idLeague)
        {
            ResponseObjectJsonDto response = await iClubService.GetClubsByLeague(idLeague);

            if (response.Code != 200)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet("sponsor/{idClub:int}")]
        public async Task<IActionResult> GetSponsorsById(int idClub)
        {
            ResponseObjectJsonDto response = await iClubService.GetSponsorsByIdClub(idClub);

            if (response.Code != 200)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet("sponsor/nonclubSponsor/{idClub:int}")]
        public async Task<IActionResult> GetNonclubSponsors(int idClub)
        {
            ResponseObjectJsonDto response = await iClubService.GetNonclubSponsors(idClub);

            if (response.Code != 200)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }        

        [HttpPatch]
        [Route("clubAuthority")]
        public async Task<ActionResult> UpdateClubAuthority([FromBody] ClubAuthorityPatchDto clubAuthorityDto)
        {
            ResponseObjectJsonDto response = await iClubAuthorityService.UpdateClubAuthority(clubAuthorityDto);
            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }

        [HttpPatch("information")]
        public async Task<ActionResult> UpdateClubInformation([FromForm] ClubInformationPatchDto clubInfoDto)
        {
            ResponseObjectJsonDto response = await iClubService.UpdateClubInformation(clubInfoDto);
            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }
            return Ok(response);
        }

        [HttpPatch("update")]
        public async Task<ActionResult> UpdateClub([FromForm] ClubPatchDto clubDto)
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;
            ResponseObjectJsonDto response = await iClubService.UpdateFirstClub(clubDto, encryptedIdUser);
            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }        
    }
}
