using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPIAFA.Helpers;
using WebAPIAFA.Helpers.Config;
using WebAPIAFA.Helpers.Enum;
using WebAPIAFA.Models;
using WebAPIAFA.Models.Dtos.BulletinDtos;
using WebAPIAFA.Models.Dtos.PassDtos;
using WebAPIAFA.Services.IServices;

namespace WebAPIAFA.Controllers
{
    [ApiController]
    [Route("api/bulletin")]
    [Authorize]
    public class BulletinController : ControllerBase
    {
        private readonly IBulletinService iBulletinServ;
        private readonly IConfig iConfig;
        private readonly IHttpContextAccessor iAccessor;

        public BulletinController(IHttpContextAccessor iAccessor, IBulletinService iBulletinServ, IConfig iConfig)
        {
            this.iAccessor = iAccessor;
            this.iBulletinServ = iBulletinServ;
            this.iConfig = iConfig;
        }


        [HttpPost]
        public async Task<ActionResult> CreateBulletin([FromForm] BulletinCreateDto bulletinDto)
        {
            ResponseObjectJsonDto response = await iBulletinServ.CreateBulletin(bulletinDto);
            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("statement/subscribers")]
        public async Task<ActionResult> AddStatementSubscribers([FromForm] StatementSubscribersAddDto subscribersAddDto)
        {
            //string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;
            ResponseObjectJsonDto response = await iBulletinServ.AddStatementSubscribers(subscribersAddDto);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("{idBulletin:int}/signature/encustody/lote")]
        [AllowAnonymous]//No tenia authorize asique se lo pongo por las dudas.
        public IActionResult SignatureLote(int idBulletin)
        {
            /*
             * Encustody responde a traves de un formulario
             * si el formulario que leemos tiene la clave error, significa que ocurrio algo
             * en otro caso te va a devolver el token cuil success
             */
            HttpRequest httpRequest = iAccessor.HttpContext.Request;

            string error = httpRequest.Form.ContainsKey("error") ? httpRequest.Form["error"] : "";

            string urlRedirect = $"{iConfig.GetUrlFront()}/bddoWF/statement/consult/failed";

            if (!string.IsNullOrEmpty(error))
            {
                return Redirect(urlRedirect);
            }

            string token = httpRequest.Form["token"];

            string cuil = httpRequest.Form["cuil"];
            string otherParams = httpRequest.Form["otherParams"];

            ResponseObjectJsonDto result = iBulletinServ.SignEncustodyLote(token, cuil, otherParams);

            if (result.Code == (int)CodeHTTP.REDIRECT)
            {
                return Redirect(result.Message);
            }

            return BadRequest();
        }

        [HttpGet("types")]
        public async Task<ActionResult> GetBulletinTypes()
        {
            ResponseObjectJsonDto response = await iBulletinServ.GetBulletinTypes();
            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("filter")]
        public async Task<ActionResult> GetBulletins([FromQuery] BulletinFiltersDto bulletinFilters)
        {
            ResponseObjectJsonDto response = await iBulletinServ.GetBulletins(bulletinFilters);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("GetStatements")]
        public async Task<ActionResult> GetStatements([FromQuery] BulletinFiltersDto bulletinFilters)
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;

            ResponseObjectJsonDto response = await iBulletinServ.GetStatements(bulletinFilters, encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);             
        }

        [HttpGet]
        [Route("statement/received")]
        public async Task<ActionResult> GetStatementsReceived([FromQuery] BulletinFiltersDto bulletinFilters)
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;

            ResponseObjectJsonDto response = await iBulletinServ.GetStatementsReceived(bulletinFilters, encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("statement/consult")]
        public async Task<ActionResult> GetStatementsConsult([FromQuery] BulletinFiltersDto bulletinFilters)
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;

            ResponseObjectJsonDto response = await iBulletinServ.GetStatementsConsult(bulletinFilters, encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("Statements/Counters")]
        public async Task<ActionResult> GetAFADashboardCounters()
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;

            ResponseObjectJsonDto response = await iBulletinServ.GetStatementCounters(encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpPut]
        [Route("action/{idBulletin:int}")]
        public async Task<IActionResult> PerformAction(int idBulletin)
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;
            StatementSignDto statement = new StatementSignDto();
            //Asigno valores hasta que se haga el cambio en FRONT.
            statement.IdBulletin = idBulletin;
            statement.PosX = 150;
            statement.PosY = 0;

            ResponseObjectJsonDto response = await iBulletinServ.PerformAction(encryptedIdUser, statement);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }        
    }
}
