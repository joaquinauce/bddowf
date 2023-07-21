using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Utils;
using WebAPIAFA.Entity;
using WebAPIAFA.Helpers;
using WebAPIAFA.Helpers.Config;
using WebAPIAFA.Helpers.Crypto;
using WebAPIAFA.Helpers.Enum;
using WebAPIAFA.Helpers.File;
using WebAPIAFA.Helpers.Hash;
using WebAPIAFA.Helpers.Pagination;
using WebAPIAFA.Helpers.Signature.Encustody;
using WebAPIAFA.Helpers.StampRequest;
using WebAPIAFA.Models;
using WebAPIAFA.Models.Dtos.FolderDtos;
using WebAPIAFA.Models.Dtos.PlayerDtos;
using WebAPIAFA.Models.Dtos.UserDtos;
using WebAPIAFA.Repository.IRepository;
using WebAPIAFA.Services.IServices;

namespace WebAPIAFA.Services
{
    public class FolderService : IFolderService
    {
        private readonly IMapper mapper;
        private readonly IFolderRepository iFolderRepo;
        private readonly IDbOperation dbOperation;
        private readonly IHelperFile iHelperFile;
        private readonly IDocumentRepository iDocumentRepo;
        private readonly IStepRepository iStepRepo;
        private readonly IUserRepository iUserRepo;
        private readonly IConfig iConfig;
        private readonly IStampRequestHelper iStampRequestHelper;
        private readonly ICrypto iCrypto;

        public FolderService(IMapper mapper,
                            IFolderRepository iFolderRepo,
                            IDbOperation dbOperation,
                            IDocumentRepository iDocumentRepo,
                            IHelperFile iHelperFile,
                            IStepRepository iStepRepo,
                            IUserRepository iUserRepo,
                            IConfig iConfig,
                            IStampRequestHelper iStampRequestHelper,
                            ICrypto iCrypto)
        {
            this.mapper = mapper;
            this.iFolderRepo = iFolderRepo;
            this.dbOperation = dbOperation;
            this.iHelperFile = iHelperFile;
            this.iDocumentRepo = iDocumentRepo;
            this.iStepRepo = iStepRepo;
            this.iUserRepo = iUserRepo;
            this.iConfig = iConfig;
            this.iStampRequestHelper = iStampRequestHelper;
            this.iCrypto = iCrypto;
        }

        public async Task<ResponseObjectJsonDto> CreateFolder(FolderCreateDto folderDto, string encryptedMailUser)
        {
            using (IDbContextTransaction dbContextTransaction = await dbOperation.BeginTransaction())
            {
                try
                {
                    string mailUser = iCrypto.DecryptText(encryptedMailUser);
                    User user = await iUserRepo.GetUserByMail(mailUser);
                    if (folderDto.BeginDate < DateTime.Now)
                    {
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "La fecha de inicio no puede ser anterior a la fecha actual" };
                    }
                    Folder folder = mapper.Map<FolderCreateDto, Folder>(folderDto);
                    List<int> users = new List<int>();
                    users.Add(folderDto.IdAffectedUser);
                    users.AddRange(folderDto.Users);
                    foreach (var userId in users)
                    {
                        bool userExist = await iUserRepo.UserExists(userId);
                        if (!userExist)
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "No existe uno de los usuarios seleccionados." };
                        }
                    }
                    folder.IdCreationUser = user.IdUser;
                    if (user.IdClub == null)
                    {
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "El usuario logueado no es responsable de club." };
                    }
                    folder.IdClub = user.IdClub;
                    User us = await iUserRepo.GetUser(folder.IdAffectedUser);
                    folder.Name = us.Cuil + " - " + us.Name + " " + us.LastName;

                    await iFolderRepo.CreateFolder(folder);

                    if (!await dbOperation.Save())
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.INTERNALSERVER, Message = "Error de servidor." };
                    }

                    List<Document> documents = new List<Document>();
                    IList<IFormFile> files = folderDto.Files;
                    bool upload = true;

                    foreach (IFormFile file in files)
                    {
                        string fileName = iHelperFile.GetFileName(file);

                        Document document = new Document
                        {
                            IdFolder = folder.IdFolder,
                            FileName = file.FileName,
                            CurrentFile = fileName,
                            OriginalFile = fileName,
                        };

                        documents.Add(document);

                        bool uploadCurrent = iHelperFile.UploadEncrypt(iHelperFile.GetPathDocumentCurrent(), fileName, file);
                        bool uploadOriginal = iHelperFile.UploadEncrypt(iHelperFile.GetPathDocumentOriginal(), fileName, file);

                        if (!uploadCurrent || !uploadOriginal)
                        {
                            upload = false;
                        }
                    }

                    if (!upload)
                    {
                        foreach (Document document in documents)
                        {
                            iHelperFile.DeleteFile(iHelperFile.GetPathDocumentOriginal(), document.OriginalFile);
                            iHelperFile.DeleteFile(iHelperFile.GetPathDocumentCurrent(), document.CurrentFile);

                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.INTERNALSERVER, Message = "Error de servidor." };
                        }
                    }

                    await iDocumentRepo.CreateDocuments(documents);

                    if (!await dbOperation.Save())
                    {
                        foreach (Document document in documents)
                        {
                            iHelperFile.DeleteFile(iHelperFile.GetPathDocumentOriginal(), document.OriginalFile);
                            iHelperFile.DeleteFile(iHelperFile.GetPathDocumentCurrent(), document.CurrentFile);

                        }
                        await dbOperation.Rollback(dbContextTransaction);
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.INTERNALSERVER, Message = "Error de servidor." };
                    }

                    List<Step> steps = new List<Step>();

                    foreach (int idUser in users)
                    {
                        Step step = new Step
                        {
                            IdFolder = folder.IdFolder,
                            IdUser = idUser,
                            IdActionType = (int)CodeActionType.Firma,
                        };

                        steps.Add(step);
                    }

                    await iStepRepo.CreateSteps(steps);

                    if (!await dbOperation.Save())
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.INTERNALSERVER, Message = "Error de servidor." };
                    }

                    for (int i = 0; i < steps.Count; i++)
                    {
                        if (i == 0)
                        {
                            steps[i].IdBackStep = null;
                            if (i != steps.Count - 1)
                            {
                                steps[i].IdNextStep = steps[i + 1].IdStep;
                            }
                            steps[i].IsCurrent = true;
                        }
                        else if (i == steps.Count - 1)
                        {
                            steps[i].IdBackStep = steps[i - 1].IdStep;
                            steps[i].IdNextStep = null;
                        }
                        else
                        {
                            steps[i].IdBackStep = steps[i - 1].IdStep;
                            steps[i].IdNextStep = steps[i + 1].IdStep;
                        }

                        steps[i].OrderNumber = i + 1;
                    }

                    iStepRepo.UpdateSteps(steps);

                    if (!await dbOperation.Save())
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.INTERNALSERVER, Message = "Error de servidor." };
                    }

                    await dbOperation.Commit(dbContextTransaction);
                }
                catch (Exception)
                {
                    await dbOperation.Rollback(dbContextTransaction);
                    throw;
                }
            }

            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.OK, Message = "Se creo correctamente la carpeta." };
        }

        public async Task<ResponseObjectJsonDto> GetFolder(int idFolder)
        {
            FolderGetDto folderGetDto = await iFolderRepo.GetFolderGetDto(idFolder);
            if (folderGetDto == null)
            {
                return new ResponseObjectJsonDto
                {
                    Code = (int)CodeHTTP.BADREQUEST,
                    Message = "La carpeta no existe"
                };
            }
            folderGetDto.Users = await iUserRepo.GetUsersByIdFolder(idFolder);
            if (folderGetDto.Users == null)
            {
                return new ResponseObjectJsonDto
                {
                    Code = (int)CodeHTTP.BADREQUEST,
                    Message = "No hay usuarios participantes"
                };

            }
            folderGetDto.Documents = await iDocumentRepo.GetDocumentsGetDtoByIdFolder(idFolder);
            if (folderGetDto.Documents == null)
            {
                return new ResponseObjectJsonDto
                {
                    Code = (int)CodeHTTP.BADREQUEST,
                    Message = "La carpeta no contiene documentos"
                };
            }

            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.OK, Response = folderGetDto };
        }

        public async Task<ResponseObjectJsonDto> GetFolderReceived(string encryptedUserName, FolderFilterDto filters)
        {
            string userName = iCrypto.DecryptText(encryptedUserName);
            User user = await iUserRepo.GetUser(userName);

            List<FolderTrayGetDto> lstFolderGetDto = await iFolderRepo.GetFolderReceived(user.IdUser, filters);

            if (lstFolderGetDto == null || lstFolderGetDto.Count == 0)
            {
                return new ResponseObjectJsonDto
                {
                    Code = (int)CodeHTTP.BADREQUEST,
                    Message = "La consulta no arroja resultados"
                };
            }

            //paginacion
            decimal numberOfPages = 0;

            if (filters.Pagination == null)
            {
                filters.Pagination = new();
                filters.Pagination.IsPaginated = false;
            }

            if (filters.Pagination.IsPaginated)
            {
                var result = lstFolderGetDto.Page(filters.Pagination);

                numberOfPages = Math.Ceiling((decimal)lstFolderGetDto.Count / (decimal)filters.Pagination.AmountRegistersPage);

                return new ResponseObjectJsonDto()
                {
                    Code = (int)CodeHTTP.OK,
                    Response = new
                    {
                        TotalQuantity = lstFolderGetDto.Count,
                        numberOfPages,
                        lstFolderGetDto = result
                    }
                };
            }
            else
            {
                return new ResponseObjectJsonDto()
                {
                    Code = (int)CodeHTTP.OK,
                    Response = new
                    {
                        TotalQuantity = lstFolderGetDto.Count,
                        numberOfPages,
                        lstFolderGetDto
                    }
                };
            }
        }

        public async Task<ResponseObjectJsonDto> GetFolderConsult(string encryptedUserName, FolderFilterDto filters)
        {
            string userName = iCrypto.DecryptText(encryptedUserName);
            User user = await iUserRepo.GetUser(userName);

            List<FolderTrayGetDto> lstFolderGetDto = await iFolderRepo.GetFolderConsult(user.IdUser, filters);

            if (lstFolderGetDto == null || lstFolderGetDto.Count == 0)
            {
                return new ResponseObjectJsonDto
                {
                    Code = (int)CodeHTTP.NOTFOUND,
                    Message = "La consulta no arroja resultados"
                };
            }

            //paginacion
            decimal numberOfPages = 0;

            if (filters.Pagination == null)
            {
                filters.Pagination = new();
                filters.Pagination.IsPaginated = false;
            }

            if (filters.Pagination.IsPaginated)
            {
                var result = lstFolderGetDto.Page(filters.Pagination);

                numberOfPages = Math.Ceiling((decimal)lstFolderGetDto.Count / (decimal)filters.Pagination.AmountRegistersPage);

                return new ResponseObjectJsonDto() 
                { 
                    Code = (int)CodeHTTP.OK, 
                    Response = new 
                    { 
                        TotalQuantity = lstFolderGetDto.Count, 
                        numberOfPages, 
                        lstFolderGetDto = result 
                    }
                };
            }
            else
            {
                return new ResponseObjectJsonDto()
                {
                    Code = (int)CodeHTTP.OK,
                    Response = new 
                    { 
                        TotalQuantity = lstFolderGetDto.Count, 
                        numberOfPages, 
                        lstFolderGetDto 
                    }
                };
            }
        }

        public async Task<ResponseObjectJsonDto> GetFoldersByUser(int idUser, FolderFilterDto filters)
        {
            User user = await iUserRepo.GetUser(idUser);

            if (user == null)
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "El usuario no existe" };
            }

            if (user.UserType.Name != "Jugador")
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "El usuario seleccionado debe ser jugador" };
            }

            UserContractGetDto userGetDto = mapper.Map<User, UserContractGetDto>(user);

            IList<Folder> lstFolderGet = await iFolderRepo.GetFoldersByIdAffectedUserAndFilters(idUser, filters);

            if (lstFolderGet != null && lstFolderGet.Count != 0)
            {
                IList<FolderInfoGetDto> lstFolderGetDto = mapper.Map<IList<Folder>, IList<FolderInfoGetDto>>(lstFolderGet);

                foreach (var folder in lstFolderGetDto)
                {
                    Document doc = await iDocumentRepo.GetLastDocumentByIdFolder(folder.IdFolder);
                    folder.IdDocument = doc.IdDocument;
                    folder.FileName = doc.FileName;
                }

                userGetDto.Folder = lstFolderGetDto;
            }

            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.OK, Response = userGetDto };
        }

        public async Task<ResponseObjectJsonDto> GetPlayerContractHistory(string encryptedIdUser)
        {
            int idUser = Convert.ToInt32(iCrypto.DecryptText(encryptedIdUser));
            User user = await iUserRepo.GetUser(idUser);

            if (user == null)
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "El usuario no existe" };
            }

            if (user.UserType.Name != "Jugador")
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "El usuario seleccionado debe ser jugador" };
            }

            IList<PlayerContractHistoryGetDto> lstFolderGet = await iFolderRepo.GetPlayerContractHistory(user.IdUser);

            if (lstFolderGet == null || lstFolderGet.Count == 0)
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.OK, Message = "El jugador no posee contratos." };
            }

            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.OK, Response = lstFolderGet };
        }

        public async Task<ResponseObjectJsonDto> PerformAction(int idFolder, string encryptedUserName, List<FolderSignDto> lstFolderSignDto)
        {
            Step currentStep = await iStepRepo.GetCurrentStep(idFolder);

            string userName = iCrypto.DecryptText(encryptedUserName);
            User currentUser = await iUserRepo.GetUser(userName);

            if (currentStep.IdUser != currentUser.IdUser)
            {
                return new ResponseObjectJsonDto
                {
                    Code = (int)CodeHTTP.BADREQUEST,
                    Message = "Accion no permitida"
                };
            }

            //ResponseObjectJsonDto responseObjectJsonDto = await iStampRequestHelper.StampManagement(currentUser.IdUser, idFolder, currentStep.IdStep, null, null);

            //if (responseObjectJsonDto.Code != (int)CodeHTTP.OK)
            //{
            //    return responseObjectJsonDto;
            //}

            int Code;
            string Message;

            switch (currentStep.IdActionType)
            {
                case (int)CodeActionType.Pase:
                    Code = (int)CodeHTTP.OK;
                    Message = "Acción realizada con exito";
                    break;
                case (int)CodeActionType.Lectura:
                    Code = (int)CodeHTTP.OK;
                    Message = "Acción realizada con exito";
                    break;
                case (int)CodeActionType.Firma:
                    ResponseObjectJsonDto response = await GetTokenEncustody(idFolder, userName, lstFolderSignDto);
                    return response;
                default:
                    Code = (int)CodeHTTP.OK;
                    Message = "Acción incorrecta";
                    break;
            }

            try
            {
                List<Step> steps = new List<Step>();
                if (currentStep.IdNextStep != null)
                {
                    Step nextStep = await iStepRepo.GetStep((int)currentStep.IdNextStep);
                    currentStep.IsCurrent = false;
                    currentStep.IsDone = true;
                    nextStep.IsCurrent = true;
                    steps.Add(currentStep);
                    steps.Add(nextStep);
                }
                else
                {
                    currentStep.IsDone = true;
                    steps.Add(currentStep);
                }

                iStepRepo.UpdateSteps(steps);
                if (!await dbOperation.Save())
                {
                    return new ResponseObjectJsonDto
                    {
                        Code = (int)CodeHTTP.INTERNALSERVER,
                        Message = "Error al intentar actualizar pasos"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto
                {
                    Code = (int)CodeHTTP.INTERNALSERVER,
                    Message = $"Error al intentar actualizar pasos {ex.Message}"
                };
            }

            return new ResponseObjectJsonDto
            {
                Code = Code,
                Message = Message
            };
        }

        protected async Task<ResponseObjectJsonDto> GetTokenEncustody(int idFolder, string username, List<FolderSignDto> lstFolderSignDto)
        {
            // buscamos el usuario
            User user = await iUserRepo.GetUser(username);

            // verificamos existencia del user
            if (user is null)
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST };
            }

            // buscamos la carpeta
            Folder folder = await iFolderRepo.GetFolder(idFolder);

            // verificamos existencia de la carpeta
            if (folder is null)
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST };
            }

            // buscamos el paso actual de la carpeta
            Step stepCurrent = await iStepRepo.GetCurrentStep(idFolder);

            // chequeamos que el usuario del paso actual sea igual al usuario que desea realizar la firma
            if (!stepCurrent.IdUser.Equals(user.IdUser))
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST };
            }

            //List<Document> documents = await iDocumentRepo.GetDocumentsByIdFolder(idFolder);
            List<string> documentsHash = new List<string>();
            string pathDocumentCurrent = iHelperFile.GetPathDocumentCurrent();
            string clientSecret = iConfig.GetEncustodyClientSecret();
            string clientId = iConfig.GetEncustodyClientId();
            string urlServicioCustodia = iConfig.GetUrlServicioCustodia();
            string hash = HelperHash.GenerarHash(clientId + "|" + clientSecret).ToLower();
            string urlResponse = $"{iConfig.GetUrlBack()}/api/folder/{folder.IdFolder}/signature/encustody/lote";
            string otherParams = "";

            for (int i = 0; i < lstFolderSignDto.Count; i++)
            {
                otherParams += lstFolderSignDto[i].IdDocument + "-" +
                    lstFolderSignDto[i].PosX + "-" +
                    lstFolderSignDto[i].PosY;
                if (i < lstFolderSignDto.Count - 1)
                {
                    otherParams += ";";
                }
            }

            foreach (FolderSignDto folderSignDto in lstFolderSignDto)
            {
                Document document = iDocumentRepo.GetDocumentSync(folderSignDto.IdDocument);
                string filenameCurrent = document.CurrentFile; // obtenemos el nombre del documento actual
                byte[] documentBytes = iHelperFile.GetFileDecrypt(pathDocumentCurrent, filenameCurrent); // obtenemos los bytes del pdf desencriptado
                string documentSHA256 = HelperHash.GetSHA256FromFile(documentBytes); // generamos sha256 del documento
                string hashDocument = HelperHash.GenerarHashSHA256(documentSHA256 + "|" + clientSecret); // generamos el hash para encustody
                //otherParams = $"{document.IdDocument}|";
                documentsHash.Add(hashDocument); // agregamos el hash documento a la lista
            }

            string stringHashDocuments = "[";
            for (int i = 0; i < documentsHash.Count(); i++)
            {
                if (i == documentsHash.Count() - 1)
                {
                    stringHashDocuments += $"\"{documentsHash[i]}\"";
                }
                else
                {
                    stringHashDocuments += $"\"{documentsHash[i]}\",";
                }
            }
            stringHashDocuments += "]"; // formato que hay que enviar a encutody ["hash1","hash2"]

            // instanciamos el objeto que vamos a enviar a encustody
            EncustodyDataLote encustodyDataLote = new EncustodyDataLote
            {
                Accion = "solicitudFirmaPorLote",
                Client_Id = clientId,
                Hash = hash,
                UrlResponse = urlResponse,
                Username = user.Cuil,
                CantidadDocumentos = lstFolderSignDto.Count,
                HashDocumentos = stringHashDocuments,
                OtherParams = otherParams,
            };

            EncustodyFront encustodyFront = new EncustodyFront
            {
                EncustodyDataLote = encustodyDataLote,
                UrlRedireccion = urlServicioCustodia,
            };

            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.OK, Response = encustodyFront };
        }

        public ResponseObjectJsonDto SignEncustodyLote(string token, string cuil, string otherParams, int idFolder)
        {
            //List<string> idDocuments = otherParams.Split('|').ToList(); // obtenemos los id de los documentos de la carpeta
            //List<Document> documents = iDocumentRepo.GetDocumentsByIdFolderSync(idFolder);
            List<FolderSignDto> lstFolderSignDto = new List<FolderSignDto>();
            string[] splittedOtherParams = otherParams.Split(";");

            foreach (string param in splittedOtherParams)
            {
                string[] singleDocParam = param.Split("-");
                FolderSignDto folderSignDto = new FolderSignDto();
                folderSignDto.IdDocument = Convert.ToInt32(singleDocParam[0]);
                folderSignDto.PosX = Convert.ToInt32(singleDocParam[1]);
                folderSignDto.PosY = Convert.ToInt32(singleDocParam[2]);
                lstFolderSignDto.Add(folderSignDto);
            }

            List<Tuple<int, string>> documentsSign = new List<Tuple<int, string>>();

            string urlRequest = iConfig.GetUrlServicioCustodia() + "firma-por-lote-old";

            // requester crea el formulari que se manda con los archivos a firmar
            RequestMaker requester = new RequestMaker(urlRequest);
            requester.addParameter("token", token);
            requester.addParameter("aliasCertificado", "");
            requester.addParameter("signatureReason", "Firmado Conformidad.");

            foreach (FolderSignDto folderSignDto in lstFolderSignDto)
            {
                string pathDocumentCurrent = iHelperFile.GetPathDocumentCurrent();

                Document document = iDocumentRepo.GetDocumentSync(folderSignDto.IdDocument);
                string filename = document.CurrentFile;
                byte[] documentBytes = iHelperFile.GetFileDecrypt(pathDocumentCurrent, filename);
                string pdfStringBase64 = Convert.ToBase64String(documentBytes);

                requester.addParameter("base64file", pdfStringBase64);
                requester.addParameter("fileName", filename);
                requester.addParameter("posX", folderSignDto.PosX);
                requester.addParameter("posY", folderSignDto.PosY);
                requester.addParameter("disableSignatureRendering", "false");
                requester.addParameter("dynamicText", "");
                requester.addParameter("base64Image", "");

                string fileSignature = requester.doRequest();

                EncustodyResponseFirmaLote result = JsonConvert.DeserializeObject<EncustodyResponseFirmaLote>(fileSignature);

                if (!string.IsNullOrEmpty(result.Error))
                {
                    return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST };
                }

                documentsSign.Add(Tuple.Create(document.IdDocument, result.SignedFile));
            }

            // chequeamos que la cantidad de documentos firmados sea igual a los documentos de la carpeta
            if (!documentsSign.Count.Equals(lstFolderSignDto.Count))
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST };
            }

            foreach (FolderSignDto folderSignDto in lstFolderSignDto)
            {
                Document document = iDocumentRepo.GetDocumentSync(folderSignDto.IdDocument);
                Tuple<int, string> data = documentsSign.FirstOrDefault(d => d.Item1.Equals(document.IdDocument));
                byte[] pdfSign = Convert.FromBase64String(data.Item2); // convertimos string a byte[]
                string newFilaname = iHelperFile.CreateFileName(); // creamos un nuvo filename
                string currentFilename = document.CurrentFile;
                string pathDocumentCurrent = iHelperFile.GetPathDocumentCurrent();
                // borra el archivo actual, y sube el nuevo archivo con el nuevo nombre
                bool result = iHelperFile.ReplaceFileEncrypt(pathDocumentCurrent, currentFilename, newFilaname, pdfSign);
                if (!result)
                {
                    return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST };
                }
                document.CurrentFile = newFilaname;

                iDocumentRepo.UpdateDocument(document);
            }

            // actualizamos los documentos, el currentFile mas especificamente
            //iDocumentRepo.UpdateDocuments(documents);

            if (!dbOperation.SaveSync())
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST };
            }

            // actualizacion de los pasos
            Step currentStep = iStepRepo.GetCurrentStepSync(idFolder);

            List<Step> steps = new List<Step>();
            if (currentStep.IdNextStep != null)
            {
                Step nextStep = iStepRepo.GetStepSync((int)currentStep.IdNextStep);
                currentStep.IsCurrent = false;
                currentStep.IsDone = true;
                nextStep.IsCurrent = true;
                steps.Add(currentStep);
                steps.Add(nextStep);
            }
            else
            {
                currentStep.IsDone = true;
                steps.Add(currentStep);
            }

            iStepRepo.UpdateSteps(steps);

            if (!dbOperation.SaveSync())
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST };
            }

            // url donde va a redireccionar el back, ojo que todo badrequest en este metodo
            // hay que reemplazar por redirect, solo que en vez de success sera error
            // en front verificara que sea success o error y hara lo que tenga que hacer   
            string urlRedirect = $"{iConfig.GetUrlFront()}/bddoWF/folder/received/success";

            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.REDIRECT, Message = urlRedirect };
        }
    }
}
