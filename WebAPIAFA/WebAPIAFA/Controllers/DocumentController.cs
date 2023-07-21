using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPIAFA.Helpers;
using WebAPIAFA.Helpers.Enum;
using WebAPIAFA.Helpers.File;
using WebAPIAFA.Helpers.ValidatorSignature;
using WebAPIAFA.Models;
using WebAPIAFA.Models.Dtos.DocumentDtos;
using WebAPIAFA.Services.IServices;

namespace WebAPIAFA.Controllers
{
    [ApiController]
    [Route("api/document")]
    [Authorize]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService documentService;
        private readonly IHelperFile helperFile;

        public DocumentController(IDocumentService documentService, IHelperFile helperFile)
        {
            this.documentService = documentService;
            this.helperFile = helperFile;
        }

        [HttpGet]
        [Route("file/{idDocument:int}")]
        public async Task<ActionResult> GetFileDecrypt(int idDocument)
        {
            Document doc = await documentService.GetFile(idDocument);

            if (doc == null)
            {
                return BadRequest("El documento no existe");
            }
            else
            {
                byte[] pdf = helperFile.GetFileDecrypt(helperFile.GetPathDocumentCurrent(), doc.CurrentFile);
                return File(pdf, "application/pdf", doc.CurrentFile);
            }

        }
        [HttpPost]

        [Route("validator")]
        public async Task<ActionResult<ResponseObjectJsonDto>> Validator(int idDocument)
        {
            ResponseObjectJsonDto response = await documentService.ValidateSignature(idDocument);
            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("affectedUserDocuments")]
        public async Task<ActionResult <ResponseObjectJsonDto>> GetAffectedUserDocuments(int idAffectedUser)
        {
            ResponseObjectJsonDto response = await documentService.GetAffectedUserDocuments(idAffectedUser);
            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }
    }
}
