using Microsoft.AspNetCore.Mvc;
using WebAPIAFA.Helpers.File;

namespace WebAPIAFA.Controllers
{
    [ApiController]
    [Route("api/file")]
    public class FileController : ControllerBase
    {
        private readonly IHelperFile helperFile;

        public FileController(IHelperFile helperFile)
        {
            this.helperFile = helperFile;
        }

        [HttpPost]
        public async Task<ActionResult> UploadFile(IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName).ToLower();
            Guid guid = Guid.NewGuid();
            string fileName = $"{guid.ToString()}{extension}";
            bool result = await helperFile.Upload(helperFile.GetPathClubImage(), fileName, file);

            return result ? Ok() : BadRequest();
        }

        [HttpGet]
        public ActionResult GetFile()
        {
            FileStream file = helperFile.GetFile(helperFile.GetPathClubImage(), "d455d9b3-1c4c-4f66-9626-2af4c19dd155.png");

            return file != null ? File(file, "image/png")  : NotFound();
        }

        [HttpPost]
        [Route("encrypt")]
        public ActionResult UploadFileEncrypt(IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName).ToLower();
            Guid guid = Guid.NewGuid();
            string fileName = $"{guid.ToString()}{extension}";
            bool result = helperFile.UploadEncrypt(helperFile.GetPathClubFile(), fileName, file);

            return result ? Ok() : BadRequest();

        }

        [HttpGet]
        [Route("decrypt")]
        public ActionResult GetFileDecrypt()
        {
            byte[] pdf = helperFile.GetFileDecrypt(helperFile.GetPathDocumentCurrent(), "fabe1ab5-b22c-4389-aafe-1a1896cddfe2.pdf");
            return File(pdf, "application/pdf", "fabe1ab5-b22c-4389-aafe-1a1896cddfe2.pdf");
        }
    }
}
