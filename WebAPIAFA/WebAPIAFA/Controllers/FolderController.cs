using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Utils;
using WebAPIAFA.Helpers;
using WebAPIAFA.Helpers.Enum;
using WebAPIAFA.Helpers.Signature.Encustody;
using WebAPIAFA.Models.Dtos.FolderDtos;
using WebAPIAFA.Services.IServices;

namespace WebAPIAFA.Controllers
{
    [Route("api/folder")]
    [ApiController]
    [Authorize]
    public class FolderController : ControllerBase
    {
        private readonly IFolderService iFolderServ;
        private readonly IHttpContextAccessor accessor;

        public FolderController(IFolderService iFolderServ, 
                                IHttpContextAccessor accessor)
        {
            this.iFolderServ = iFolderServ;
            this.accessor = accessor;
        }

        [HttpGet]
        [Route("{idFolder:int}")]
        public async Task<ActionResult> GetFolder(int idFolder)
        {
            ResponseObjectJsonDto result = await iFolderServ.GetFolder(idFolder);

            if (result.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(result.Code, result.Message);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("received")]
        public async Task<IActionResult> GetFolderReceived([FromQuery] FolderFilterDto filters)
        {
            string encryptedUserName = ((ClaimsIdentity)User.Identity).FindFirst("user").Value;

            ResponseObjectJsonDto result = await iFolderServ.GetFolderReceived(encryptedUserName, filters);

            if (result.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(result.Code, result.Message);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("consult")]
        public async Task<IActionResult> GetFolderConsult([FromQuery] FolderFilterDto filters)
        {
            string encryptedUserName = ((ClaimsIdentity)User.Identity).FindFirst("user").Value;

            ResponseObjectJsonDto response = await iFolderServ.GetFolderConsult(encryptedUserName, filters);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("player/{idUser:int}")]
        public async Task<ActionResult> GetFolderByUser(int idUser, [FromQuery] FolderFilterDto filters)
        {
            ResponseObjectJsonDto response = await iFolderServ.GetFoldersByUser(idUser, filters);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("player/contractHistory")]
        public async Task<ActionResult> GetPlayerContractHistory()
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;

            ResponseObjectJsonDto response = await iFolderServ.GetPlayerContractHistory(encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> CreateFolder([FromForm] FolderCreateDto folderDto)
        {
            string encryptedMail = ((ClaimsIdentity)User.Identity).FindFirst("mail").Value;

            ResponseObjectJsonDto response = await iFolderServ.CreateFolder(folderDto, encryptedMail);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }  
        
        [HttpPut]
        [Route("action/{idFolder:int}")]
        public async Task<IActionResult> PerformAction(int idFolder, [FromBody] List<FolderSignDto> lstFolderSignDto)
        {
            string encryptedUserName = ((ClaimsIdentity)User.Identity).FindFirst("user").Value;

            ResponseObjectJsonDto response = await iFolderServ.PerformAction(idFolder, encryptedUserName, lstFolderSignDto);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("{idFolder:int}/signature/encustody/lote")]
        [AllowAnonymous]//No tenia authorize asique se lo pongo por las dudas.
        public IActionResult SignatureLote(int idFolder)
        {
            /*
             * Encustody responde a traves de un formulario
             * si el formulario que leemos tiene la clave error, significa que ocurrio algo
             * en otro caso te va a devolver el token cuil success
             */
            HttpRequest httpRequest = accessor.HttpContext.Request;

            string error = httpRequest.Form.ContainsKey("error") ? httpRequest.Form["error"] : "";

            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest();
            }

            string token = httpRequest.Form["token"];
            string cuil = httpRequest.Form["cuil"];
            string otherParams = httpRequest.Form["otherParams"];

            ResponseObjectJsonDto result = iFolderServ.SignEncustodyLote(token, cuil, otherParams, idFolder);

            if (result.Code == (int)CodeHTTP.REDIRECT)
            {
                return Redirect(result.Message);
            }

            return BadRequest();
        }
    }
}
