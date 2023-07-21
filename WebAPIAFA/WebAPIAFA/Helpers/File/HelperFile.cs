using System.IO;
using WebAPIAFA.Helpers.Config;
using WebAPIAFA.Helpers.Crypto;

namespace WebAPIAFA.Helpers.File
{
    public class HelperFile : IHelperFile
    {
        private readonly IWebHostEnvironment env;
        private readonly ICrypto crypto;
        private readonly IConfig config;
        private readonly string nameFolderImages = "Images";
        private readonly string nameFolderFiles = "Files";
        private readonly string pathRoot;

        public HelperFile(IWebHostEnvironment env, ICrypto crypto, IConfig config)
        {
            this.env = env;
            this.crypto = crypto;
            this.config = config;
            //this.pathRoot = "\\\\10.10.10.2\\comun\\BDDOWF\\wwwroot";
            //this.pathRoot = config.GetSharedFolderPath();
            this.pathRoot = env.WebRootPath;
        }

        public FileStream GetFile(string path, string nameFile)
        {
            string pathFile = Path.Combine(path, nameFile);
            if (!System.IO.File.Exists(pathFile))
            {
                return null;
            }

            FileStream file = System.IO.File.OpenRead(pathFile);
            return file;
        }

        public string GetPathClubImage()
        {
            return Path.Combine(pathRoot, nameFolderImages, "Club");
        }

        public string GetPathSponsorImage()
        {
            return Path.Combine(pathRoot, nameFolderImages, "Sponsor");
        }

        public string GetPathUserImage()
        {
            return Path.Combine(pathRoot, nameFolderImages, "User");
        }

        public string GetPathStadiumImage()
        {
            return Path.Combine(pathRoot, nameFolderImages, "Stadium");
        }

        public string GetPathLeagueImage()
        {
            return Path.Combine(pathRoot, nameFolderImages, "League");
        }

        public async Task<bool> Upload(string path, string nameFile, IFormFile file)
        {
            bool result = false;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (file.Length > 0)
            {
                string pathFull = Path.Combine(path, nameFile);

                using (FileStream stream = new FileStream(pathFull, FileMode.Create))
                {
                    await file.CopyToAsync(stream);

                    result = true;
                }

            }

            return result;
        }

        public bool DeleteFile(string path, string nameFile)
        {
            bool result = false;
            string pathFull = Path.Combine(path, nameFile);
            if (System.IO.File.Exists(pathFull))
            {
                System.IO.File.Delete(pathFull);
                result = true;
            }
            return result;
        }

        public string GetPathClubFile()
        {
            return Path.Combine(pathRoot, nameFolderFiles, "Club");
        }
        public string GetPathBulletinFile()
        {
            return Path.Combine(pathRoot, nameFolderFiles, "Bulletins");
        }

        public string GetPathSanctionFile()
        {
            return Path.Combine(pathRoot, nameFolderFiles, "Sanctions");
        }

        public bool UploadEncrypt(string path, string nameFile, IFormFile file)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            byte[] data = ConvertIFormFileToArrayByte(file);
            
            if (data == null)
            {
                return false;
            }

            byte[] pdfEncrypt = crypto.EncryptFile(data);

            string pathFull = Path.Combine(path, nameFile);

            try
            {
                using (BinaryWriter writer = new BinaryWriter(System.IO.File.OpenWrite(pathFull)))
                {
                    writer.Write(pdfEncrypt);
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public byte[] ConvertIFormFileToArrayByte(IFormFile file)
        {
            byte[] data = null;

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                data = ms.ToArray();
            }

            return data;
        }

        public byte[] GetFileDecrypt(string path, string nameFile)
        {
            byte[] pdfEncrypt = ConvertFileToArrayByte(path, nameFile);
            
            if (pdfEncrypt == null)
            {
                return null;
            }

            byte[] decryptFile = crypto.DecryptFile(pdfEncrypt);

            if (decryptFile == null)
            {
                return null;
            }

            return decryptFile;
        }

        public byte[] ConvertFileToArrayByte(string path, string nameFile)
        {
            string pathFile = Path.Combine(path, nameFile);
            if (!System.IO.File.Exists(pathFile))
            {
                return null;
            }

            return System.IO.File.ReadAllBytes(pathFile);
        }

        public string GetFileName(IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName).ToLower();
            Guid guid = Guid.NewGuid();
            string fileName = $"{guid.ToString()}{extension}";

            return fileName;
        }

        public string GetPathDocumentCurrent()
        {
            return Path.Combine(pathRoot, nameFolderFiles, "Documents", "Current");
        }

        public string GetPathDocumentOriginal()
        {
            return Path.Combine(pathRoot, nameFolderFiles, "Documents", "Original");
        }

        public string CreateFileName()
        {
            Guid guid = Guid.NewGuid();
            string fileName = $"{guid.ToString()}.pdf";
            return fileName;
        }

        public bool ReplaceFileEncrypt(string path, string currentFilename, string newFilename, byte[] file)
        {
            bool delete = DeleteFile(path, currentFilename);

            if (!delete)
            {
                return false;
            }

            byte[] pdfEncrypt = crypto.EncryptFile(file);

            string pathFull = Path.Combine(path, newFilename);

            try
            {
                using (BinaryWriter writer = new BinaryWriter(System.IO.File.OpenWrite(pathFull)))
                {
                    writer.Write(pdfEncrypt);
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;

        }

        public bool ReplaceFileDecrypt(string path, string currentFilename, string newFilename, byte[] file)
        {
            bool delete = DeleteFile(path, currentFilename);

            if (!delete)
            {
                return false;
            }

            string pathFull = Path.Combine(path, newFilename);

            try
            {
                using (BinaryWriter writer = new BinaryWriter(System.IO.File.OpenWrite(pathFull)))
                {
                    writer.Write(file);
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;

        }

        public byte[] GetFileBytes(string path, string fileName)
        {
            return ConvertFileToArrayByte(path, fileName);
        }
    }
}
