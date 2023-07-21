using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPIAFA.Helpers;
using WebAPIAFA.Helpers.Enum;
using WebAPIAFA.Models.Dtos.TournamentDivisionDtos;
using WebAPIAFA.Services.IServices;

namespace WebAPIAFA.Controllers
{
    [ApiController]
    [Route("api/administration")]
    public class AdministrationController : ControllerBase
    {
        private readonly IAdministrationService iAdminServ;

        public AdministrationController(IAdministrationService iAdminServ)
        {
            this.iAdminServ = iAdminServ;
        }

        [HttpGet]
        [Route("country")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetCountries()
        {
            ResponseObjectJsonDto response = await iAdminServ.GetCountries();

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);

        }

        [HttpGet]
        [Route("gender")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetGenders()
        {
            ResponseObjectJsonDto response = await iAdminServ.GetGenders();

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);

        }

        [HttpGet]
        [Route("province/{idCountry:int}")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetProvinciesByIdCountry(int idCountry)
        {
            ResponseObjectJsonDto response = await iAdminServ.GetProvinciesByIdCountry(idCountry);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);

        }

        [HttpGet]
        [Route("reasons")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetReasons()
        {
            ResponseObjectJsonDto response = await iAdminServ.GetReasons();

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);

        }

        [HttpGet]
        [Route("location/{idProvince:int}")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetLocationsByIdProvince(int idProvince)
        {
            ResponseObjectJsonDto response = await iAdminServ.GetLocationsByIdProvince(idProvince);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);

        }


        [HttpGet]
        [Route("role")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetRoles()
        {
            ResponseObjectJsonDto response = await iAdminServ.GetRoles();

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("user/roles")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetEspecificRoles()
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;

            ResponseObjectJsonDto response = await iAdminServ.GetEspeficRoles(encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);

        }


        [HttpGet]
        [Route("userTypes")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetUserTypes()
        {
            ResponseObjectJsonDto response = await iAdminServ.GetUserTypes();

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);

        }

        [HttpGet]
        [Route("userType/{name}")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetUserTypesByName(string name)
        {
            ResponseObjectJsonDto response = await iAdminServ.GetUserTypesByName(name);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);

        }

        [HttpGet]
        [Route("documentType")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetDocumentTypes()
        {
            ResponseObjectJsonDto response = await iAdminServ.GetDocumentTypes();

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("tournamentDivision")]
        public async Task<ActionResult> CreateTournamentDivision([FromForm] TournamentDivisionPostDto tournament)
        {
            ResponseObjectJsonDto response = await iAdminServ.CreateTournamentDivision(tournament);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("tournamentDivision/{IdTournament:int}")]
        public async Task<ActionResult> GetTournamentDivision(int IdTournament)
        {
            ResponseObjectJsonDto response = await iAdminServ.GetTournamentDivision(IdTournament);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }
        
    }
}
