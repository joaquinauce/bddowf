using Microsoft.Extensions.Configuration;

namespace WebAPIAFA.Helpers.Config
{
    public class Config : IConfig
    {
        private readonly IConfiguration configuration;

        public Config(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetConectionDB()
        {
            return configuration.GetConnectionString("DefaultConnection");
        }  
        public string GetJwt()
        {
            return configuration["Jwt"];
        }

        public string GetEmailAccount()
        {
            return configuration["Mail:EmailAccount"];
        }

        public string GetEmailPassword()
        {
            return configuration["Mail:EmailPassword"];
        }

        public string GetEmailPort()
        {
            return configuration["Mail:EmailPort"];
        }

        public string GetEnableSSL()
        {
            return configuration["Mail:EnableSSL"];
        }

        public string GetEmailUser()
        {
            return configuration["Mail:EmailUser"];
        }

        public string GetEmailSmtpServer()
        {
            return configuration["Mail:EmailSmtpServer"];
        }

        public string GetEncustodyClientId()
        {
            return configuration["Encustody:ClientId"];
        }

        public string GetEncustodyClientSecret()
        {
            return configuration["Encustody:ClientSecret"];
        }

        public string GetUrlFront()
        {
            return configuration["Url:Front"];
        }

        public string GetUrlBack()
        {
            return configuration["Url:Back"];
        }

        public string GetUrlServicioCustodia()
        {
            return configuration["Url:ServicioCustodia"];
        }

        public string GetCertificatePath()
        {
            return configuration["Certificate:CertPath"];
        }

        public string GetCertificatePassword()
        {
            return configuration["Certificate:CertPass"];
        }

        public string GetStampRequestUrl()
        {
            return configuration["CompetitionStampsApiUrls:stampRequestUrl"];
        }

        public string GetStampObtainUrl()
        {
            return configuration["CompetitionStampsApiUrls:stampObtainUrl"];
        }

        public string GetUrlServicioValidador()
        {
            return configuration["ValidatorSignature:ServicioValidador"];
        }
        public string GetMailValidador()
        {
            return configuration["ValidatorSignature:MailValidador"];
        }
        public string GetApiKeyValidador()
        {
            return configuration["ValidatorSignature:ApiKey"];
        }
        public string GetClaveEncriptadora()
        {
            return configuration["ValidatorSignature:KeyValidatorCripto"];
        }

        public string GetFastUserIdGender()
        {
            return configuration["UserFastCreation:IdGender"];
        }

        public string GetFastUserIdDocType()
        {
            return configuration["UserFastCreation:IdDocumentType"];
        }

        public string GetFastUserIdLocation()
        {
            return configuration["UserFastCreation:IdLocation"];
        }

        public string GetFastUserIdUserType()
        {
            return configuration["UserFastCreation:IdUserType"];
        }

        public string GetSharedFolderPath()
        {
            return configuration["SharedFolderPath:Path"];
        }
    }
}
