using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using WebAPIAFA.Entity;
using WebAPIAFA.Helpers;
using WebAPIAFA.Helpers.Crypto;
using WebAPIAFA.Helpers.Enum;
using WebAPIAFA.Helpers.File;
using WebAPIAFA.Models;
using WebAPIAFA.Models.Dtos.LeagueDtos;
using WebAPIAFA.Models.Dtos.PlayerDtos;
using WebAPIAFA.Models.Dtos.SanctionDtos;
using WebAPIAFA.Models.Dtos.UserDtos;
using WebAPIAFA.Repository.IRepository;
using WebAPIAFA.Services.IServices;

namespace WebAPIAFA.Services
{
    public class LeagueService : ILeagueService
    {
        private readonly ICrypto iCrypto;
        private readonly IDbOperation iDbOperation;
        private readonly IHelperFile iHelperFile;
        private readonly ILeagueRepository iLeagueRepo;
        private readonly IMapper iMapper;
        private readonly IPlayerRepository iPlayerRepo;
        private readonly IResponsibleXLeagueRepository iResponsibleXLeagueRepo;
        private readonly IRoleRepository iRoleRepo;
        private readonly IRoleUserRepository iRoleUserRepo;
        private readonly ISanctionRepository iSanctionRepo;
        private readonly IUserRepository iUserRepo;

        public LeagueService(ICrypto iCrypto, IDbOperation iDbOperation, IHelperFile iHelperFile, 
            ILeagueRepository iLeagueRepo, IMapper iMapper, IPlayerRepository iPlayerRepo,
            IResponsibleXLeagueRepository iResponsibleXLeagueRepo, IRoleRepository iRoleRepo, IRoleUserRepository iRoleUserRepo, 
            ISanctionRepository iSanctionRepo, IUserRepository iUserRepo)
        {
            this.iCrypto = iCrypto;
            this.iDbOperation = iDbOperation;
            this.iHelperFile = iHelperFile;
            this.iLeagueRepo = iLeagueRepo;
            this.iMapper = iMapper;
            this.iPlayerRepo = iPlayerRepo;
            this.iResponsibleXLeagueRepo = iResponsibleXLeagueRepo;
            this.iRoleRepo = iRoleRepo;
            this.iRoleUserRepo = iRoleUserRepo;
            this.iSanctionRepo = iSanctionRepo;
            this.iUserRepo = iUserRepo;
        }

        public async Task<ResponseObjectJsonDto> CreateLeague(LeaguePostDto leaguePostDto)
        {
            try
            {

                if (await iLeagueRepo.LeagueExistCUIT(leaguePostDto.CUIT))
                {
                    return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "Ya existe una liga registrada con ese CUIT" };
                }

                League league = iMapper.Map<LeaguePostDto, League>(leaguePostDto);
                await iLeagueRepo.CreateLeague(league);

                if (!await iDbOperation.Save())
                {
                    return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Message = "Error al crear la liga." };
                }

                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Message = "Liga creada exitosamente" };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }

        public async Task<ResponseObjectJsonDto> GetImageLeague(string encryptedIdUser)
        {
            try
            {
                int idLoggedUser = Convert.ToInt32(iCrypto.DecryptText(encryptedIdUser));

                League league = await iLeagueRepo.GetLeagueByResponsibleId(idLoggedUser);

                if (league == null)
                {
                    return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Message = "El usuario no es responsable de una liga" };
                }
                else if (league.Image == null)
                {
                    return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.BADREQUEST, Message = "La liga no tiene imagen" };
                }
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Response = league.Image };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }

        public async Task<ResponseObjectJsonDto> GetLeagueByResponsibleId(string encryptedIdUser)
        {
            int idLoggedUser = Convert.ToInt32(iCrypto.DecryptText(encryptedIdUser));

            League league= await iLeagueRepo.GetLeagueByResponsibleId(idLoggedUser);

            if (league == null)
            {
                return new ResponseObjectJsonDto
                {
                    Code = (int)CodeHTTP.BADREQUEST,
                    Message = "El usuario no es responsable de liga"
                };
            }

            return new ResponseObjectJsonDto
            {
                Code = (int)CodeHTTP.OK,
                Message = "Liga encontrada",
                Response = league
            };
        }

        public async Task<ResponseObjectJsonDto> GetLeagueSelect()
        {
            List<LeagueSelectGetDto> lstLeaguesSelectDto = await iLeagueRepo.GetLeaguesSelect();

            if (lstLeaguesSelectDto == null || lstLeaguesSelectDto.Count == 0)
            {
                return new ResponseObjectJsonDto
                {
                    Code = (int)CodeHTTP.OK,
                    Message = "La consulta no trajo resultados"
                };
            }

            return new ResponseObjectJsonDto
            {
                Code = (int)CodeHTTP.OK,
                Message = "Consulta exitosa",
                Response = lstLeaguesSelectDto
            };
        }

        public async Task<ResponseObjectJsonDto> GetPlayersByLeague(int idLeague)
        {

            IList<PlayerLeagueGetDto> lstPlayers = await iPlayerRepo.GetPlayersByLeague(idLeague);

            if (lstPlayers == null || lstPlayers.Count == 0)
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.OK, Response = "La consulta no trajo resultados" };
            }

            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.OK, Response = lstPlayers };
        }

        public async Task<ResponseObjectJsonDto> GetPlayersByLeagueResponsibleDiscipline(string cuil, string encryptedIdUser)
        {
            int idUser = Convert.ToInt32(iCrypto.DecryptText(encryptedIdUser));

            ResponsibleXLeague responsible = await iResponsibleXLeagueRepo.GetResponsibleXLeague(idUser);

            if (responsible == null)
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "El usuario no tiene ninguna liga a cargo." };
            }

            IList<User> lstPlayers = await iUserRepo.GetPlayersByLeagueAndCuil(cuil, responsible.IdLeague);

            if (lstPlayers == null || lstPlayers.Count == 0)
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.OK, Message = "La consulta no trajo resultados" };
            }

            IList<UserCuilNameGetDto> lstPlayerGetDto = iMapper.Map<IList<User>, IList<UserCuilNameGetDto>>(lstPlayers);

            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.OK, Response = lstPlayerGetDto };
        }

        public async Task<ResponseObjectJsonDto> UpdateLeague(LeaguePatchDto leagueDto)
        {
            if (!await iLeagueRepo.LeagueExists(leagueDto.IdLeague))
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "No existe la liga." };
            }
            if (await iLeagueRepo.LeagueUpdateExistsCUIT(leagueDto.IdLeague, leagueDto.CUIT))
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "Ya existe una liga registrada con el mismo CUIT." };
            }
            League league = iMapper.Map<League>(leagueDto);
            string fileName = null;

            if (await iLeagueRepo.LeagueHaveImage(leagueDto.IdLeague))
            {
                League current = await iLeagueRepo.GetLeague(leagueDto.IdLeague);
                string currentFileName = current.Image;
                if (leagueDto.Image != null)
                {
                    byte[] currentImage = iHelperFile.ConvertFileToArrayByte(iHelperFile.GetPathLeagueImage(), currentFileName);
                    byte[] newImage = iHelperFile.ConvertIFormFileToArrayByte(leagueDto.Image);
                    //VALIDAR SI LA IMAGEN SUBIDA ES IGUAL A LA ANTERIOR
                    if (!newImage.SequenceEqual(currentImage))
                    {
                        //GENERAR FILENAME DE IMAGEN
                        fileName = iHelperFile.GetFileName(leagueDto.Image);

                        //REEMPLAZA IMAGEN NUEVA POR LA ACTUAL
                        bool result = iHelperFile.ReplaceFileDecrypt(iHelperFile.GetPathLeagueImage(), currentFileName, fileName, newImage);

                        if (!result)
                        {
                            return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Message = "Error al guardar la nueva imagén." };
                        }
                    }
                    else
                    {
                        fileName = currentFileName;
                    }
                }
                else
                {
                    fileName = currentFileName;
                }
            }
            else
            {
                if (leagueDto.Image != null)
                {
                    IFormFile image = leagueDto.Image;
                    fileName = iHelperFile.GetFileName(image);
                    if (!await iHelperFile.Upload(iHelperFile.GetPathLeagueImage(), fileName, image))
                    {
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar la imagén." };
                    }
                }
            }
            league.Image = fileName;
            iLeagueRepo.UpdateLeague(league);

            if (!await iDbOperation.Save())
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.INTERNALSERVER, Message = $"Algo salió mal actualizando la liga {league.Name} - {league.CUIT}" };
            }
            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.OK, Message = "Liga actualizada exitosamente" };
        }
    }
}
