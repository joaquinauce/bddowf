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
using WebAPIAFA.Helpers.Signature.Encustody;
using WebAPIAFA.Models;
using WebAPIAFA.Models.Dtos.BulletinDtos;
using WebAPIAFA.Models.Dtos.BulletinTypeDtos;
using WebAPIAFA.Repository.IRepository;
using WebAPIAFA.Services.IServices;

namespace WebAPIAFA.Services
{
    public class BulletinService : IBulletinService
    {
        private readonly IBulletinSubscriberRepository bulletinSubscriberRepo;
        private readonly IBulletinTypeRepository bulletinTypeRepo;
        private readonly IBulletinRepository bulletinRepo;
        private readonly IResponsibleXClubRepository responsibleXClubRepo;
        private readonly IConfig iConfig;
        private readonly IDbOperation dbOperation;
        private readonly IHelperFile helperFile;
        private readonly ILeagueRepository iLeagueRepo;
        private readonly IMapper mapper;
        private readonly IResponsibleXLeagueRepository iResponsibleXLeagueRepo;
        private readonly IUserRepository iUserRepo;
        private readonly ICrypto iCrypto;
        private readonly IRoleUserRepository roleUserRepo;
        private readonly IRoleRepository roleRepo;

        public BulletinService(IBulletinSubscriberRepository bulletinSubscriberRepo,
            IBulletinTypeRepository bulletinTypeRepo,
            IBulletinRepository bulletinRepo,
            IConfig iConfig,
            IResponsibleXClubRepository responsibleXClubRepo,
            IDbOperation dbOperation,
            IHelperFile helperFile,
            ILeagueRepository iLeagueRepo,
            IMapper mapper,
            IResponsibleXLeagueRepository iResponsibleXLeagueRepo,
            IUserRepository iUserRepo,
            ICrypto iCrypto,
            IRoleUserRepository roleUserRepo,
            IRoleRepository roleRepo)
        {
            this.bulletinSubscriberRepo = bulletinSubscriberRepo;
            this.bulletinTypeRepo = bulletinTypeRepo;
            this.bulletinRepo = bulletinRepo;
            this.iConfig = iConfig;
            this.responsibleXClubRepo = responsibleXClubRepo;
            this.dbOperation = dbOperation;
            this.helperFile = helperFile;
            this.iLeagueRepo = iLeagueRepo;
            this.mapper = mapper;
            this.iResponsibleXLeagueRepo = iResponsibleXLeagueRepo;
            this.iUserRepo = iUserRepo;
            this.iCrypto = iCrypto;
            this.roleUserRepo = roleUserRepo;
            this.roleRepo = roleRepo;
        }

        public async Task<ResponseObjectJsonDto> CreateBulletin(BulletinCreateDto bulletinDto)
        {
            using (IDbContextTransaction dbContextTransaction = await dbOperation.BeginTransaction())
            {
                try
                {
                    if (!await bulletinTypeRepo.BulletynTypeExists(bulletinDto.IdBulletinType))
                    {
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "No existe ese tipo de boletín" };
                    }

                    // Creacion de Archivos
                    IFormFile formFile = bulletinDto.FileName;
                    string fileName = helperFile.GetFileName(formFile);

                    if (!await helperFile.Upload(helperFile.GetPathBulletinFile(), fileName, formFile))
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el boletín." };
                    }

                    //Creacion de boletin                        
                    Bulletin bulletin = mapper.Map<BulletinCreateDto, Bulletin>(bulletinDto);
                    bulletin.FileName = fileName;
                    await bulletinRepo.CreateBulletin(bulletin);
                    if (!await dbOperation.Save())
                    {
                        helperFile.DeleteFile(helperFile.GetPathBulletinFile(), fileName);
                        await dbOperation.Rollback(dbContextTransaction);
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.INTERNALSERVER, Message = "Error de servidor." };
                    }

                    if (bulletin.HasSubscribers)
                    {
                        if (bulletinDto.LeaguesId != null && bulletinDto.LeaguesId.Count != 0)
                        {
                            List<BulletinSubscriber> bulletinSubscribers = new();
                            foreach (var leagueId in bulletinDto.LeaguesId)
                            {
                                bool leagueExist = await iLeagueRepo.LeagueExistId(leagueId);
                                if(!leagueExist)
                                {
                                    helperFile.DeleteFile(helperFile.GetPathBulletinFile(), fileName);
                                    await dbOperation.Rollback(dbContextTransaction);
                                    return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "No existe una de las ligas seleccionadas." };
                                }

                                List<ResponsibleXLeague> responsibles = await iResponsibleXLeagueRepo.GetResponsiblesByLeague(leagueId);
                                foreach (ResponsibleXLeague responsible in responsibles)
                                {
                                    BulletinSubscriber bulletinSubscriber = new BulletinSubscriber
                                    {
                                        IdBulletin = bulletin.IdBulletin,
                                        IdUser = responsible.IdResponsible,
                                        IsSigned = false
                                    };
                                    bulletinSubscribers.Add(bulletinSubscriber);
                                }

                            }
                            await bulletinSubscriberRepo.CreateBulletinSubscriber(bulletinSubscribers);

                            if (!await dbOperation.Save())
                            {
                                helperFile.DeleteFile(helperFile.GetPathBulletinFile(), fileName);
                                await dbOperation.Rollback(dbContextTransaction);
                                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.INTERNALSERVER, Message = "Error de servidor." };
                            }
                        }
                        else
                        {
                            helperFile.DeleteFile(helperFile.GetPathBulletinFile(), fileName);
                            await dbOperation.Rollback(dbContextTransaction);
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.INTERNALSERVER, Message = "Debe seleccionar al menos un usuario participante" };
                        }
                    }

                    await dbOperation.Commit(dbContextTransaction);

                }
                catch (Exception)
                {
                    await dbOperation.Rollback(dbContextTransaction);
                    throw;
                }
            }
            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.OK, Message = "Se creo correctamente el boletín." };
        }

        public async Task<ResponseObjectJsonDto> GetBulletinTypes()
        {
            try
            {
                IList<BulletinType> lstBulletin = await bulletinTypeRepo.GetBulletynTypes();
                if (lstBulletin.Count == 0 || lstBulletin == null)
                {
                    return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Response = "La consulta no arrojó resultados" };
                }
                IList<BulletinTypeGetDto> lstBulletinResponse = mapper.Map<IList<BulletinType>, IList<BulletinTypeGetDto>>(lstBulletin);
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Response = lstBulletinResponse };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }

        public async Task<ResponseObjectJsonDto> GetBulletins(BulletinFiltersDto bulletinFilters)
        {
            try
            {
                if (bulletinFilters.IdBulletinType != null)
                {
                    if (!await bulletinTypeRepo.BulletynTypeExists((int)bulletinFilters.IdBulletinType))
                    {
                        return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.BADREQUEST, Message = "El tipo de boletin no existe" };
                    }
                }

                IList<Bulletin> listBulletins = await bulletinRepo.GetBulletins(bulletinFilters);


                if (listBulletins.Count == 0 || listBulletins == null)
                {
                    return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Response = "La consulta no arrojó resultados" };
                }

                IList<BulletinGetDto> listBulletinsResponse = mapper.Map<IList<Bulletin>, IList<BulletinGetDto>>(listBulletins);

                foreach (var bulletin in listBulletinsResponse)
                {
                    bulletin.Users = await bulletinSubscriberRepo.GetUsersByBulletin(bulletin.IdBulletin);
                }

                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Response = listBulletinsResponse };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }

        public async Task<ResponseObjectJsonDto> GetStatements(BulletinFiltersDto bulletinFilters, string encryptedIdUser)
        {
            int idLoggedUser = Convert.ToInt32(iCrypto.DecryptText(encryptedIdUser));

            List<BulletinGetDto> lstStatements = await bulletinRepo.GetStatements(bulletinFilters, idLoggedUser);

            return new ResponseObjectJsonDto
            {
                Code = (int)CodeHTTP.OK,
                Message = "Consulta exitosa",
                Response = lstStatements
            };
        }

        public async Task<ResponseObjectJsonDto> GetStatementsReceived(BulletinFiltersDto bulletinFilters, string encryptedIdUser)
        {
            int idLoggedUser = Convert.ToInt32(iCrypto.DecryptText(encryptedIdUser));

            List<BulletinGetDto> lstStatements = await bulletinRepo.GetStatementsReceived(bulletinFilters, idLoggedUser);

            return new ResponseObjectJsonDto
            {
                Code = (int)CodeHTTP.OK,
                Message = "Consulta exitosa",
                Response = lstStatements
            };
        }

        public async Task<ResponseObjectJsonDto> GetStatementsConsult(BulletinFiltersDto bulletinFilters, string encryptedIdUser)
        {
            int idLoggedUser = Convert.ToInt32(iCrypto.DecryptText(encryptedIdUser));

            List<BulletinGetDto> lstStatements = await bulletinRepo.GetStatementsConsult(bulletinFilters, idLoggedUser);

            return new ResponseObjectJsonDto
            {
                Code = (int)CodeHTTP.OK,
                Message = "Consulta exitosa",
                Response = lstStatements
            };
        }

        public async Task<ResponseObjectJsonDto> PerformAction(string encryptedIdUser, StatementSignDto statement)
        {
            int idLoggedUser = Convert.ToInt32(iCrypto.DecryptText(encryptedIdUser));

            ResponseObjectJsonDto response = await GetTokenEncustody(idLoggedUser, statement);
            return response;
        }

        protected async Task<ResponseObjectJsonDto> GetTokenEncustody(int idLoggedUser, StatementSignDto statement)
        {
            // buscamos el usuario
            User user = await iUserRepo.GetUser(idLoggedUser);

            // verificamos existencia del user
            if (user is null)
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "El usuario no existe" };
            }

            // buscamos el comunicado
            Bulletin bulletin = await bulletinRepo.GetBulletin(statement.IdBulletin);

            // verificamos existencia del comunicado
            if (bulletin is null)
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "El comunicado no existe" };
            }

            List<string> documentsHash = new();
            string pathDocumentCurrent = helperFile.GetPathBulletinFile();
            string clientSecret = iConfig.GetEncustodyClientSecret();
            string clientId = iConfig.GetEncustodyClientId();
            string urlServicioCustodia = iConfig.GetUrlServicioCustodia();
            string hash = HelperHash.GenerarHash(clientId + "|" + clientSecret).ToLower();
            string urlResponse = $"{iConfig.GetUrlBack()}/api/bulletin/{bulletin.IdBulletin}/signature/encustody/lote";
            string otherParams = "";
            int countDocuments = 1;

            otherParams += bulletin.IdBulletin + "-" + statement.PosX + "-" + statement.PosY + "-" + idLoggedUser;

            string filenameCurrent = bulletin.FileName; // obtenemos el nombre del comunicado actual
            byte[] document = helperFile.GetFileBytes(pathDocumentCurrent, filenameCurrent);
            string documentSHA256 = HelperHash.GetSHA256FromFile(document); // generamos sha256 del documento
            string hashDocument = HelperHash.GenerarHashSHA256(documentSHA256 + "|" + clientSecret); // generamos el hash para encustody
            documentsHash.Add(hashDocument); // agregamos el hash documento a la lista

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
            stringHashDocuments += "]"; // formato que hay que enviar a encustody ["hash1","hash2"]

            // instanciamos el objeto que vamos a enviar a encustody
            EncustodyDataLote encustodyDataLote = new EncustodyDataLote
            {
                Accion = "solicitudFirmaPorLote",
                Client_Id = clientId,
                Hash = hash,
                UrlResponse = urlResponse,
                Username = user.Cuil,
                CantidadDocumentos = countDocuments,
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

        public ResponseObjectJsonDto SignEncustodyLote(string token, string cuil, string otherParams)
        {
            List<Tuple<int, string>> documentsSign = new List<Tuple<int, string>>();

            string urlRequest = iConfig.GetUrlServicioCustodia() + "firma-por-lote-old";
            string[] splittedOtherParams = otherParams.Split("-");

            // requester crea el formulario que se manda con los archivos a firmar
            RequestMaker requester = new RequestMaker(urlRequest);
            requester.addParameter("token", token);
            requester.addParameter("aliasCertificado", "");
            requester.addParameter("signatureReason", "Firmado Conformidad.");

            string pathDocumentCurrent = helperFile.GetPathBulletinFile();

            Bulletin bulletin = bulletinRepo.GetBulletinSync(int.Parse(splittedOtherParams[0]));
            string filename = bulletin.FileName;
            byte[] documentBytes = helperFile.GetFileBytes(pathDocumentCurrent, filename);
            string pdfStringBase64 = Convert.ToBase64String(documentBytes);

            requester.addParameter("base64file", pdfStringBase64);
            requester.addParameter("fileName", filename);
            requester.addParameter("posX", int.Parse(splittedOtherParams[1]));
            requester.addParameter("posY", int.Parse(splittedOtherParams[2]));
            requester.addParameter("disableSignatureRendering", "false");
            requester.addParameter("dynamicText", "");
            requester.addParameter("base64Image", "");

            string fileSignature = requester.doRequest();

            EncustodyResponseFirmaLote result = JsonConvert.DeserializeObject<EncustodyResponseFirmaLote>(fileSignature);

            if (!string.IsNullOrEmpty(result.Error))
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST };
            }

            documentsSign.Add(Tuple.Create(bulletin.IdBulletin, result.SignedFile));

            Tuple<int, string> data = documentsSign.FirstOrDefault(d => d.Item1.Equals(bulletin.IdBulletin));
            byte[] pdfSign = Convert.FromBase64String(data.Item2); // convertimos string a byte[]
            string newFilaname = helperFile.CreateFileName(); // creamos un nuvo filename
            string currentFilename = bulletin.FileName;
            string pathBulletinCurrent = helperFile.GetPathBulletinFile();
            // borra el archivo actual, y sube el nuevo archivo con el nuevo nombre
            bool resultt = helperFile.ReplaceFileDecrypt(pathDocumentCurrent, currentFilename, newFilaname, pdfSign);
            if (!resultt)
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "Ocurrió un error en la subida del archivo" };
            }
            bulletin.FileName = newFilaname;
            bulletinRepo.UpdateBulletin(bulletin);

            if (!dbOperation.SaveSync())
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "Ocurrió un error al actualizar el comunicado" };
            }

            //Actualizo el bulletinSuscribers si fue firmado
            BulletinSubscriber bulletinSubscriber = bulletinSubscriberRepo.GetBulletinSubscriberSync(int.Parse(splittedOtherParams[3]), bulletin.IdBulletin);

            if (bulletinSubscriber == null)
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "El comunicado no tiene suscriptores" };
            }

            bulletinSubscriber.IsSigned = true;
            bulletinSubscriberRepo.UpdateBulletinSuscriber(bulletinSubscriber);

            if (!dbOperation.SaveSync())
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "Ocurrió un error al actualizar la suscripcion del comunicado" };
            }
            // url donde va a redireccionar el back, ojo que todo badrequest en este metodo
            // hay que reemplazar por redirect, solo que en vez de success sera error
            // en front verificara que sea success o error y hara lo que tenga que hacer   
            string urlRedirect = $"{iConfig.GetUrlFront()}/bddoWF/statement/consult/success";

            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.REDIRECT, Message = urlRedirect };
        }

        public async Task<ResponseObjectJsonDto> AddStatementSubscribers(StatementSubscribersAddDto subscribersAddDto)
        {
            Bulletin bulletin = await bulletinRepo.GetBulletin(subscribersAddDto.IdBulletin);
            if (bulletin == null)
            {
                return new ResponseObjectJsonDto
                {
                    Code = (int)CodeHTTP.BADREQUEST,
                    Message = "El comunicado no existe"
                };
            }

            if (!bulletin.HasSubscribers)
            {
                return new ResponseObjectJsonDto
                {
                    Code = (int)CodeHTTP.BADREQUEST,
                    Message = "El documento no es un comunicado dirigido"
                };
            }

            List<int> subscribers = new List<int>();

            //tomar los responsables de cada club y agregarlos a una lista
            foreach (int idClub in subscribersAddDto.idClubs)
            {
                List<int> clubSubscribers = await responsibleXClubRepo.GetResponsibleXClubByIdClub(idClub);

                subscribers.AddRange(clubSubscribers);
            }

            List<BulletinSubscriber> lstBulletinSubscribers = new List<BulletinSubscriber>();
            //crear cada elemento de la lista con el boletin en bulletinsubscribers
            foreach (int idSubscriber in subscribers)
            {
                //validar que no exista ya en statement para ese usuario antes de crear 
                if (! await bulletinSubscriberRepo.BulletinSubscribersExist(subscribersAddDto.IdBulletin, idSubscriber))
                {
                    BulletinSubscriber bulletinSubscriber = new BulletinSubscriber();
                    bulletinSubscriber.IdBulletin = subscribersAddDto.IdBulletin;
                    bulletinSubscriber.IdUser = idSubscriber;
                    bulletinSubscriber.IsSigned = false;

                    lstBulletinSubscribers.Add(bulletinSubscriber);
                }
            }

            if (lstBulletinSubscribers.Count != 0)
            {
                await bulletinSubscriberRepo.CreateBulletinSubscriber(lstBulletinSubscribers);

                if (!await dbOperation.Save())
                {
                    return new ResponseObjectJsonDto
                    {
                        Code = (int)CodeHTTP.INTERNALSERVER,
                        Message = "Hubo un error al intentar reenviar el comunicado"
                    };
                }
            }
            else
            {
                return new ResponseObjectJsonDto
                {
                    Code = (int)CodeHTTP.BADREQUEST,
                    Message = "El reenvio ya fue realizado anteriormente o \nno hay destinatarios posibles para el comunicado"
                };
            }

            return new ResponseObjectJsonDto
            {
                Code = (int)CodeHTTP.OK,
                Message = "Comunicado reenviado correctamente",
            };
        }

        public async Task<ResponseObjectJsonDto> GetStatementCounters(string encryptedIdUser)
        {
            int idUser = Convert.ToInt32(iCrypto.DecryptText(encryptedIdUser));

            UserRole userRole = await roleUserRepo.GetUserRole(idUser);

            Role AFARole = await roleRepo.GetRole("Responsable AFA");

            if (userRole.IdRole != AFARole.IdRole)
            {
                return new ResponseObjectJsonDto
                {
                    Code = (int)CodeHTTP.BADREQUEST,
                    Message = "El usuario no es responsable de AFA"
                };
            }

            BulletinAFADashBoardCounters counters = new BulletinAFADashBoardCounters();

            counters.ReadPending = await bulletinRepo.GetReadPendingCounter();

            counters.Signed = await bulletinRepo.GetSignedCounter();

            counters.Sent = await bulletinRepo.GetSentCounter();

            counters.Forwarded = await bulletinRepo.GetForwardedCounter();

            counters.Seen = await bulletinRepo.GetSeenCounter();

            return new ResponseObjectJsonDto
            {
                Code = (int)CodeHTTP.OK,
                Message = "Consulta exitosa",
                Response = counters
            };
        }
    }
}
