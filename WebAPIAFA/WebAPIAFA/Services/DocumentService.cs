using AutoMapper;
using WebAPIAFA.Helpers;
using WebAPIAFA.Helpers.Enum;
using WebAPIAFA.Helpers.File;
using WebAPIAFA.Helpers.ValidatorSignature;
using WebAPIAFA.Models;
using WebAPIAFA.Models.Dtos.DocumentDtos;
using WebAPIAFA.Models.Dtos.StampRequestDtos;
using WebAPIAFA.Models.Dtos.TournamentDivisionDtos;
using WebAPIAFA.Repository.IRepository;
using WebAPIAFA.Services.IServices;

namespace WebAPIAFA.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository documentRepository;
        private readonly IValidator validator;
        private readonly IHelperFile helperFile;
        private readonly IFolderRepository folderRepository;
        private readonly IStampRequestRepository stampRepository;
        private readonly IUserRepository userRepository;
        private readonly IPassRepository passRepository;
        private readonly IMapper mapper;

        public DocumentService(IDocumentRepository documentRepository, IValidator validator,
            IHelperFile helperFile, IStampRequestRepository stampRepository, IMapper mapper,
            IUserRepository userRepository, IPassRepository passRepository,
            IFolderRepository folderRepository)
        {
            this.documentRepository = documentRepository;
            this.validator = validator;
            this.helperFile = helperFile;
            this.stampRepository = stampRepository;
            this.mapper = mapper;
            this.userRepository = userRepository;
            this.passRepository = passRepository;
            this.folderRepository = folderRepository;
        }

        public async Task<ResponseObjectJsonDto> GetAffectedUserDocuments(int idAffectedUser)
        {
            try
            {
                if (!await userRepository.UserExists(idAffectedUser))
                {
                    return new ResponseObjectJsonDto
                    {
                        Code = (int)CodeHTTP.BADREQUEST,
                        Message = "El usuario no existe"
                    };
                }

                List<DocumentAffectedUserHistoryDto> lstDocAffectedUser = new List<DocumentAffectedUserHistoryDto>();

                IList<Pass> lstPasses = await passRepository.GetPassesByAffectedUser(idAffectedUser);

                foreach (Pass pass in lstPasses)
                {
                    DocumentAffectedUserHistoryDto docAffectedUser = new();
                    List<Document> lstDocs = await documentRepository.GetDocumentsByIdPass(pass.IdPass);
                    foreach (Document document in lstDocs)
                    {
                        docAffectedUser = new DocumentAffectedUserHistoryDto
                        {
                            IdDocument = document.IdDocument,
                            DocumentName = document.FileName,
                            DocumentType = pass.PassType.Name,
                            BeginDate = pass.BeginDate,
                            EndDate = pass.EndDate
                        };
                        lstDocAffectedUser.Add(docAffectedUser);
                    }
                }

                List<Folder> lstFolders = (List<Folder>)await folderRepository.GetFoldersByIdAffectedUser(idAffectedUser);

                foreach (Folder folder in lstFolders)
                {
                    DocumentAffectedUserHistoryDto docAffectedUser = new();
                    List<Document> lstDocs = await documentRepository.GetDocumentsByIdFolder(folder.IdFolder);
                    foreach (Document document in lstDocs)
                    {
                        docAffectedUser = new DocumentAffectedUserHistoryDto
                        {
                            IdDocument = document.IdDocument,
                            DocumentName = document.FileName,
                            DocumentType = "Contrato",
                            BeginDate = folder.BeginDate,
                            EndDate = folder.EndDate
                        };
                        lstDocAffectedUser.Add(docAffectedUser);
                    }
                }
                return new ResponseObjectJsonDto
                {
                    Code = (int)CodeHTTP.OK,
                    Message = "Consulta realizada con éxito",
                    Response = lstDocAffectedUser
                };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto
                {
                    Code = (int)CodeHTTP.INTERNALSERVER,
                    Message = "Error" + ex.Message
                };
            }
        }

        public async Task<Document> GetFile(int idDocument)
        {
            return await documentRepository.GetFile(idDocument);
        }

        public async Task<ResponseObjectJsonDto> ValidateSignature(int idDocument)
        {
            try
            {
                Document doc = await documentRepository.GetFile(idDocument);
                if (doc == null)
                {
                    return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "El documento no existe" };
                }

                //CONTENIDO DEL PDF EN BASE64
                byte[] content = helperFile.GetFileDecrypt(helperFile.GetPathDocumentCurrent(), doc.CurrentFile);
                if (content == null)
                {
                    return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "El documento no existe en el disco" };
                }
                string contentBase64 = Convert.ToBase64String(content);


                DtoGetValidate singDto = await validator.ValidatorSignature(contentBase64, doc.CurrentFile);
                if (singDto.Errors != "")
                {
                    return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = singDto.Errors };
                }
                //OBTENGO LOS DISTINTOS PASOS QUE PARTICIPA EL DOCUMENTO
                List<int?> step = new();
                if (doc.IdFolder != null)
                {
                    step = await stampRepository.GetStepStampRequestsByFolder((int)doc.IdFolder);
                }
                else
                {
                    step = await stampRepository.GetStepStampRequestsByPass((int)doc.IdPass);
                }

                if (step == null || step.Count == 0)
                {
                    return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "El documento no posse un sello de competencia" };
                }
                List<StampRequestGetDto> lst = new();
                foreach (int item in step)
                {
                    //OBTENGO LOS ULTIMOS SELLOS DE CADA PASO
                    StampRequest stamp = new();
                    if (doc.IdFolder != null)
                    {
                        stamp = await stampRepository.GetStampRequestsByFolder((int)doc.IdFolder, item);
                    }
                    else
                    {
                        //TODO: VER COMO SE HACE CON PASES
                    }
                    if (stamp != null)
                    {
                        StampRequestGetDto stampRequestsResponse = mapper.Map<StampRequest, StampRequestGetDto>(stamp);
                        lst.Add(stampRequestsResponse);
                    }
                }
                for (int i = 0; i < singDto.Signatures.Count; i++)
                {
                    //ASIGNO EL SELLO A LA FIRMA CORRESPONDIENTE
                    singDto.Signatures[i].Stamp = lst[i];
                }

                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.OK, Response = singDto };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }

        }
    }
}
