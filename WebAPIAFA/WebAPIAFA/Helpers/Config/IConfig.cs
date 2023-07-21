namespace WebAPIAFA.Helpers.Config
{
    public interface IConfig
    {
        public string GetJwt();
        public string GetConectionDB();
        public string GetEmailAccount();
        public string GetEmailPassword();
        public string GetEmailPort();
        public string GetEnableSSL();
        public string GetEmailUser();
        public string GetEmailSmtpServer();
        public string GetEncustodyClientId();
        public string GetEncustodyClientSecret();
        public string GetUrlServicioCustodia();
        public string GetUrlFront();
        public string GetUrlBack();
        public string GetCertificatePath();
        public string GetCertificatePassword();
        public string GetStampRequestUrl();
        public string GetStampObtainUrl();
        string GetSharedFolderPath();
        public string GetUrlServicioValidador();
        public string GetMailValidador();
        public string GetApiKeyValidador();
        public string GetClaveEncriptadora();
        public string GetFastUserIdGender();
        public string GetFastUserIdDocType();
        public string GetFastUserIdLocation();
        public string GetFastUserIdUserType();
    }
}
