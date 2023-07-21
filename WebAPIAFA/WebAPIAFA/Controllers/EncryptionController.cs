using Microsoft.AspNetCore.Mvc;
using WebAPIAFA.Helpers;
using WebAPIAFA.Helpers.Crypto;

namespace WebAPIAFA.Controllers
{
    [ApiController]
    [Route("api/encryption")]
    public class EncryptionController: ControllerBase
    {
        private readonly ICrypto crypto;

        public EncryptionController(ICrypto crypto)
        {
            this.crypto = crypto;
        }

        [HttpPost]
        [Route("encrypt")]
        public string EncryptText(string inputText)
        {
            return crypto.EncryptText(inputText);
        }

        [HttpPost]
        [Route("decrypt")]
        public string DecryptText(string inputText)
        {
            return crypto.DecryptText(inputText);
        }
    }
}
