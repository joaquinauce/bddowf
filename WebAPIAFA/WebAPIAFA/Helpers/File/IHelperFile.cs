using Microsoft.AspNetCore.Components.Web;

namespace WebAPIAFA.Helpers.File
{
    public interface IHelperFile
    {
        public Task<bool> Upload(string path, string nameFile, IFormFile file);
        public bool UploadEncrypt(string path, string nameFile, IFormFile file);
        public FileStream GetFile(string path, string nameFile);
        public byte[] GetFileDecrypt(string path, string nameFile);
        public bool DeleteFile(string path, string nameFile);
        public string GetPathClubImage();
        public string GetPathSanctionFile();
        public string GetPathSponsorImage();
        public string GetPathStadiumImage();
        public string GetPathLeagueImage();
        public string GetPathUserImage();
        public string GetPathClubFile();
        public string GetPathBulletinFile();
        public string GetPathDocumentCurrent();
        public string GetPathDocumentOriginal();
        public byte[] ConvertIFormFileToArrayByte(IFormFile file);
        public byte[] ConvertFileToArrayByte(string path, string nameFile);
        public string GetFileName(IFormFile file);
        public string CreateFileName();
        public bool ReplaceFileEncrypt(string path, string currentFilename, string newFilename, byte[] file);
        public bool ReplaceFileDecrypt(string path, string currentFilename, string newFilename, byte[] file);
        public byte[] GetFileBytes(string path, string fileName);
    }
}
