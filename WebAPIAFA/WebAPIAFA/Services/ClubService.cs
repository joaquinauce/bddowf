using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using WebAPIAFA.Entity;
using WebAPIAFA.Helpers;
using WebAPIAFA.Helpers.Crypto;
using WebAPIAFA.Helpers.Enum;
using WebAPIAFA.Helpers.File;
using WebAPIAFA.Models;
using WebAPIAFA.Models.Dtos.ClubAuthorityDtos;
using WebAPIAFA.Models.Dtos.ClubDtos;
using WebAPIAFA.Models.Dtos.ClubInformationDtos;
using WebAPIAFA.Models.Dtos.ClubSponsorDtos;
using WebAPIAFA.Models.Dtos.ClubSponsorsDto;
using WebAPIAFA.Models.Dtos.ClubStaffDtos;
using WebAPIAFA.Models.Dtos.TeamDtos;
using WebAPIAFA.Models.Dtos.TournamentDivisionDtos;
using WebAPIAFA.Repository.IRepository;
using WebAPIAFA.Services.IServices;

namespace WebAPIAFA.Services
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository clubRepository;
        private readonly ICrypto crypto;
        private readonly IDbOperation dbOperation;
        private readonly IHelperFile helperFile;
        private readonly IClubMandateRepository clubMandateRepository;
        private readonly IClubAuthorityRepository clubAuthorityRepository;
        private readonly IClubInformationRepository clubInformationRepository;
        private readonly IClubFileRepository clubFileRepository;
        private readonly ILeagueRepository leagueRepository;
        private readonly IResponsibleXLeagueRepository responsibleLeagueRepo;
        private readonly IRoleUserRepository iRoleUserRepo;
        private readonly ITournamentDivisionRepository tournamentDivisionRepository;
        private readonly ISponsorRepository sponsorRepository;
        private readonly ITeamRepository teamRepository;
        private readonly IUserRepository userRepository;
        private readonly IClubStaffRepository clubStaffRepository;
        private readonly IMapper mapper;

        public ClubService(IClubRepository clubRepository,
            ICrypto crypto,
                            IDbOperation dbOperation,
                            IHelperFile helperFile,
                            IClubMandateRepository clubMandateRepository,
                            IClubAuthorityRepository clubAuthorityRepository,
                            IClubInformationRepository clubInformationRepository,
                            IClubFileRepository clubFileRepository,
                            IClubStaffRepository clubStaffRepository,
                            ILeagueRepository leagueRepository,
                            IResponsibleXLeagueRepository responsibleLeagueRepo,
                            IRoleUserRepository iRoleUserRepo,
                            ISponsorRepository sponsorRepository,
                            ITournamentDivisionRepository tournamentDivisionRepository,
                            ITeamRepository teamRepository,
                            IUserRepository userRepository,
                            IMapper mapper)
        {
            this.clubRepository = clubRepository;
            this.crypto = crypto;
            this.dbOperation = dbOperation;
            this.helperFile = helperFile;
            this.clubMandateRepository = clubMandateRepository;
            this.clubAuthorityRepository = clubAuthorityRepository;
            this.clubInformationRepository = clubInformationRepository;
            this.clubFileRepository = clubFileRepository;
            this.clubStaffRepository = clubStaffRepository;
            this.leagueRepository = leagueRepository;
            this.iRoleUserRepo = iRoleUserRepo;
            this.responsibleLeagueRepo = responsibleLeagueRepo;
            this.mapper = mapper;
            this.tournamentDivisionRepository = tournamentDivisionRepository;
            this.sponsorRepository = sponsorRepository;
            this.teamRepository = teamRepository;
            this.userRepository = userRepository;
        }
        public async Task<ResponseObjectJsonDto> CreateClub(ClubCreateDto clubDto)
        {
            using (IDbContextTransaction dbContextTransaction = await dbOperation.BeginTransaction())
            {
                try
                {
                    string businessName = clubDto.BussinessName;
                    string cuit = clubDto.CUIT;
                    string email = clubDto.Email;

                    if (await clubRepository.ClubExistBussinessName(businessName))
                    {
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "El nombre del equipo ya esta registrado." };
                    }

                    if (await clubRepository.ClubExistCuit(cuit))
                    {
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "El cuit del equipo ya esta registrado." };
                    }

                    if (await clubRepository.ClubExistEmail(email))
                    {
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "El email del equipo ya esta registrado." };
                    }

                    if (!await tournamentDivisionRepository.TournamentDivisionExistId(clubDto.IdTournamentDivision))
                    {
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "No existe el torneo seleccionado." };
                    }

                    // creacion club
                    Club club = mapper.Map<ClubCreateDto, Club>(clubDto);

                    IFormFile logo = clubDto.FileLogo;
                    string fileNameLogo = helperFile.GetFileName(logo);

                    if (!await helperFile.Upload(helperFile.GetPathClubImage(), fileNameLogo, logo))
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el logo." };
                    }

                    club.LogoClub = fileNameLogo;

                    club.IdLeague = 1;

                    await clubRepository.CreateClub(club);

                    if (!await dbOperation.Save())
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        if (helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                        }

                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el club" };
                    }

                    // creacion mandato
                    ClubMandate mandate = mapper.Map<ClubCreateDto, ClubMandate>(clubDto);
                    mandate.IdClub = club.IdClub;

                    await clubMandateRepository.CreateMandate(mandate);

                    if (!await dbOperation.Save())
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                        }
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el club" };
                    }

                    //CREACION EQUIPO, DIVISIONAL Y ASIGNACION DE TORNEO
                    foreach (var idStaff in clubDto.lstStaff)
                    {
                        ClubStaff staff = await clubStaffRepository.GetClubStaff(idStaff);
                        if (staff == null)
                        {
                            await dbOperation.Rollback(dbContextTransaction);
                            if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                            {
                                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                            }
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "No existe la división " + idStaff };
                        }
                        Team team = new Team
                        {
                            IdClubStaff = staff.IdClubStaff,
                            IdTournamentDivision = clubDto.IdTournamentDivision,
                            IdClub = club.IdClub
                        };
                        await teamRepository.CreateTeam(team);
                        if (!await dbOperation.Save())
                        {
                            await dbOperation.Rollback(dbContextTransaction);
                            if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                            {
                                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                            }
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el equipo" };
                        }
                    }


                    // Creacion de Archivos
                    // ESTATUTO
                    IFormFile formFileStatute = clubDto.FileStatute;
                    string fileNameStatute = helperFile.GetFileName(formFileStatute);

                    if (!await helperFile.Upload(helperFile.GetPathClubFile(), fileNameStatute, formFileStatute))
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                        }
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el estatuto." };
                    }

                    ClubFile fileStatute = new ClubFile
                    {
                        IdClub = club.IdClub,
                        IdFileType = (int)CodeFileTypes.Estatuto,
                        FileName = fileNameStatute,
                    };

                    await clubFileRepository.CreateClubFile(fileStatute);

                    if (!await dbOperation.Save())
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameStatute))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el estatuto." };
                        }

                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el estatuto." };
                    }

                    // ACTA DE AUTORIDADES
                    IFormFile formFileAuthorities = clubDto.FileAuthorities;
                    string fileNameAuthorities = helperFile.GetFileName(formFileAuthorities);

                    if (!await helperFile.Upload(helperFile.GetPathClubFile(), fileNameAuthorities, formFileAuthorities))
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameStatute))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el estatuto." };
                        }
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el acta de autoridades." };
                    }

                    ClubFile fileAuthorities = new ClubFile
                    {
                        IdClub = club.IdClub,
                        IdFileType = (int)CodeFileTypes.ActaAutoridades,
                        FileName = fileNameAuthorities,
                    };

                    await clubFileRepository.CreateClubFile(fileAuthorities);

                    if (!await dbOperation.Save())
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameStatute))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el estatuto." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameAuthorities))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el acta de autoridades." };
                        }

                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el acta de autoridades." };
                    }

                    // DNI PRESIDENTE
                    IFormFile formFileDniPresident = clubDto.FileDniPresident;
                    string fileNameDniPresident = helperFile.GetFileName(formFileDniPresident);

                    if (!await helperFile.Upload(helperFile.GetPathClubFile(), fileNameDniPresident, formFileDniPresident))
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameStatute))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el estatuto." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameAuthorities))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el acta de autoridades." };
                        }
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el dni del presidente." };
                    }

                    ClubFile fileDniPresident = new ClubFile
                    {
                        IdClub = club.IdClub,
                        IdFileType = (int)CodeFileTypes.DniPresidente,
                        FileName = fileNameDniPresident,
                    };

                    await clubFileRepository.CreateClubFile(fileDniPresident);

                    if (!await dbOperation.Save())
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameStatute))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el estatuto." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameAuthorities))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el acta de autoridades." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameDniPresident))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el dni del presidente." };
                        }

                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el dni del presidente." };
                    }

                    // CUENTA BANCARIA
                    IFormFile formFileBankAccount = clubDto.FileBankAccount;
                    string fileNameBankAccount = helperFile.GetFileName(formFileBankAccount);

                    if (!await helperFile.Upload(helperFile.GetPathClubFile(), fileNameBankAccount, formFileBankAccount))
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameStatute))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el estatuto." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameAuthorities))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el acta de autoridades." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameDniPresident))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el dni del presidente." };
                        }
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar la cuenta bancaria." };
                    }

                    ClubFile fileBankAccount = new ClubFile
                    {
                        IdClub = club.IdClub,
                        IdFileType = (int)CodeFileTypes.CuentaBancaria,
                        FileName = fileNameAuthorities,
                    };

                    await clubFileRepository.CreateClubFile(fileBankAccount);

                    if (!await dbOperation.Save())
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameStatute))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el estatuto." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameAuthorities))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el acta de autoridades." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameDniPresident))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el dni del presidente." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameBankAccount))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar la cuenta bancaria." };
                        }

                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar la cuenta bancaria." };
                    }

                    // CUIT
                    IFormFile formFileCuit = clubDto.FileCuit;
                    string fileNameCuit = helperFile.GetFileName(formFileCuit);

                    if (!await helperFile.Upload(helperFile.GetPathClubFile(), fileNameCuit, formFileCuit))
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameStatute))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el estatuto." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameAuthorities))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el acta de autoridades." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameDniPresident))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el dni del presidente." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameBankAccount))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar la cuenta bancaria." };
                        }
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el cuit." };
                    }

                    ClubFile fileCuit = new ClubFile
                    {
                        IdClub = club.IdClub,
                        IdFileType = (int)CodeFileTypes.Cuit,
                        FileName = fileNameCuit,
                    };

                    await clubFileRepository.CreateClubFile(fileCuit);

                    if (!await dbOperation.Save())
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameStatute))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el estatuto." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameAuthorities))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el acta de autoridades." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameDniPresident))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el dni del presidente." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameBankAccount))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar la cuenta bancaria." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameCuit))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el cuit." };
                        }

                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el cuit." };
                    }

                    await dbOperation.Commit(dbContextTransaction);
                }
                catch (Exception)
                {
                    await dbOperation.Rollback(dbContextTransaction);
                }
            }

            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.OK, Message = "Club creado." };

        }

        public async Task<ResponseObjectJsonDto> GetTournamentDivisions()
        {
            try
            {
                IList<TournamentDivision> listTournamentDivisions = await tournamentDivisionRepository.GetTournamentDivisions();
                IList<TournamentDivisionGetDto> listTournamentDivisionsResponse = mapper.Map<IList<TournamentDivision>, IList<TournamentDivisionGetDto>>(listTournamentDivisions);

                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Response = listTournamentDivisionsResponse };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }

        public async Task<ResponseObjectJsonDto> GetClubsByTournamentDivision(int idTournamentDivision)
        {
            try
            {
                IList<Club> lstClubs = await clubRepository.GetClubsByTournamentDivision(idTournamentDivision);
                IList<ClubByTournamentGetDto> lstAuthorityHierarchyGetDto = mapper.Map<IList<ClubByTournamentGetDto>>(lstClubs);

                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Response = lstAuthorityHierarchyGetDto };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }

        public async Task<ResponseObjectJsonDto> GetClubInfoById(int idClub)
        {
            try
            {
                Club club = await clubRepository.GetClubInfoById(idClub);

                if (club == null)
                {
                    return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "El club no existe" };
                }

                ClubInfoGetDto clubInfo = mapper.Map<Club, ClubInfoGetDto>(club);

                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Response = clubInfo };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }

        public async Task<ResponseObjectJsonDto> GetClubStadiumInfoById(int idClub)
        {
            try
            {
                ClubInformation clubInfo = await clubInformationRepository.GetClubInformationByClub(idClub);

                if (clubInfo == null)
                {
                    return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "La información del estadio no existe" };
                }

                ClubInformationGetDto clubStadiumInfo = mapper.Map<ClubInformation, ClubInformationGetDto>(clubInfo);

                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Response = clubStadiumInfo };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }

        public async Task<ResponseObjectJsonDto> GetClubStaffs()
        {
            try
            {
                IList<ClubStaff> listClubStaffs = await clubStaffRepository.GetClubStaffs();
                IList<ClubStaffGetDto> listClubStaffsResponse = mapper.Map<IList<ClubStaff>, IList<ClubStaffGetDto>>(listClubStaffs);

                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Response = listClubStaffsResponse };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }

        public async Task<FileStream> GetClubStatuteById(int idClub)
        {
            string fileName = await clubFileRepository.GetClubStatuteById(idClub);

            if (fileName == null)
            {
                return null;
            }

            FileStream file = helperFile.GetFile(helperFile.GetPathClubFile(), fileName);

            if (file == null)
            {
                return null;
            }

            return file;
        }

        public async Task<ResponseObjectJsonDto> GetImageClub(string encryptedIdUser)
        {
            try
            {
                int idLoggedUser = Convert.ToInt32(crypto.DecryptText(encryptedIdUser));
                User user = await userRepository.GetUser(idLoggedUser);
                Club club = await clubRepository.GetClub((int)user.IdClub);

                if (club == null)
                {
                    return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Message = "El club no existe" };
                }
                else if (club.LogoClub == null)
                {
                    return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.BADREQUEST, Message = "El club no tiene imagen" };
                }
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Response = club.LogoClub };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }

        public async Task<ResponseObjectJsonDto> GetClubsByLeague(int idLeague)
        {
            try
            {
                if (await leagueRepository.LeagueExistId(idLeague))
                {
                    ICollection<Club> lstClubs = await clubRepository.GetClubsByLeague(idLeague);
                    if (lstClubs.Count == 0)
                    {
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "La liga no posee clubes" };
                    }
                    ICollection<ClubLeagueGetDto> lstClubLeagueDto = mapper.Map<ICollection<ClubLeagueGetDto>>(lstClubs);

                    return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Response = lstClubLeagueDto };
                }
                else
                {
                    return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.BADREQUEST, Message = "La liga no existe" };
                }
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }

        public async Task<ResponseObjectJsonDto> GetSponsorsByIdClub(int idClub)
        {
            try
            {
                if (await clubRepository.ClubExistId(idClub))
                {
                    ICollection<ClubSponsor> lstClubSponsors = await clubRepository.GetClubSponsorsByClub(idClub);
                    if (lstClubSponsors.Count == 0)
                    {
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "El club no posee sponsors" };
                    }
                    ICollection<SponsorsByClubGetDto> lstClubSponsorGetDto = mapper.Map<ICollection<SponsorsByClubGetDto>>(lstClubSponsors);

                    return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Response = lstClubSponsorGetDto };
                }
                else
                {
                    return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.BADREQUEST, Message = "El club no existe" };
                }
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }

        public async Task<ResponseObjectJsonDto> GetNonclubSponsors(int idClub)
        {
            try
            {
                if (await clubRepository.ClubExistId(idClub))
                {
                    ICollection<Sponsor> lstSponsors = await clubRepository.GetNonclubSponsors(idClub);
                    if (lstSponsors.Count == 0)
                    {
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "No se encontraron sponsors ajenos al club" };
                    }

                    return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Response = lstSponsors };
                }
                else
                {
                    return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.BADREQUEST, Message = "El club no existe" };
                }

            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }


        public async Task<ResponseObjectJsonDto> CreateClubSponsor(int idClub,List<ClubSponsorPostDto> lstClubSponsorDto)
        {
            try
            {
                List<ClubSponsor> lstClubSponsor = mapper.Map<List<ClubSponsor>>(lstClubSponsorDto);
                if (lstClubSponsor.Count == 0)
                {
                    ICollection<ClubSponsor> lstClubSponsorBD = await clubRepository.GetListClubSponsor(idClub);

                    foreach(var clubSponsorBD in lstClubSponsorBD)
                    {
                        clubRepository.DeleteClubSponsor(clubSponsorBD);
                    }
                }

                foreach (var clubSponsor in lstClubSponsor)
                {
                    ICollection<ClubSponsor> lstClubSponsorBD = await clubRepository.GetListClubSponsor(clubSponsor.IdClub);

                    bool clubSponsorSinCambio = true;
                    //verifica si hay cambios en los roles
                    if (lstClubSponsorDto.Count == lstClubSponsorBD.Count)
                    {
                        foreach (var clubSponsorBD in lstClubSponsorBD)
                        {
                            if (clubSponsor.IdClub != clubSponsorBD.IdClub || clubSponsor.IdSponsor != clubSponsorBD.IdSponsor)
                            {
                                clubSponsorSinCambio = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        clubSponsorSinCambio = false;
                    }
                    if (!clubSponsorSinCambio)
                    {
                        foreach (var ClubSponsorBD in lstClubSponsorBD)
                        {
                            clubRepository.DeleteClubSponsor(ClubSponsorBD);
                        }
                        for (int i = 0; i < lstClubSponsor.Count; i++)
                        {
                            lstClubSponsor[i].CreationDate = DateTime.Now;
                        }
                        await clubRepository.CreateClubSponsor(lstClubSponsor);
                    }

                }

                if (!await dbOperation.Save())
                {
                    return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Message = "Error al asignar el sponsor al club" };
                }
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Message = "Sponsor asignado exitosamente al club" };

            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Message = ex.Message };
            }
        }

        public async Task<ResponseObjectJsonDto> GetClubs()
        {
            List<ClubGetDto> lstClubs = await clubRepository.GetClubs();

            if (lstClubs.Count == 0)
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.OK, Message = "La consulta de clubes no arroja resultados" };
            }

            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.OK, Response = lstClubs };
        }

        public async Task<ResponseObjectJsonDto> GetClubStaffsByIdClub(int idClub)
        {
            if (!await clubRepository.ClubExistId(idClub))
            {
                return new ResponseObjectJsonDto
                {
                    Code = (int)CodeHTTP.BADREQUEST,
                    Message = "El club no existe"
                };
            }

            List<ClubDivisionTeamDto> lstClubDivisionTeams = await teamRepository.GetTeamsDivisionByIdClub(idClub);

            if (lstClubDivisionTeams.Count == 0)
            {
                return new ResponseObjectJsonDto
                {
                    Code = (int)CodeHTTP.OK,
                    Message = "La consulta no arroja resultados"
                };
            }

            return new ResponseObjectJsonDto
            {
                Code = (int)CodeHTTP.OK,
                Response = lstClubDivisionTeams
            };
        }

        public async Task<ResponseObjectJsonDto> CreateClubInformation(ClubInformationCreateDto clubInfoDto)
        {
            try
            {
                if (await clubInformationRepository.ClubInformationExistsClub(clubInfoDto.IdClub))
                {
                    return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.BADREQUEST, Message = "Ya existe información del estadio del club" };
                }

                if(clubInfoDto.OpeningDate != null && clubInfoDto.OpeningDate > DateTime.Now)
                {
                    return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.BADREQUEST, Message = "La fecha de apertura debe ser anterior al dia actual" };
                }

                if (clubInfoDto.StadiumImage != null)
                {
                    //VALIDAR IMAGEN
                    string extension = Path.GetExtension(clubInfoDto.StadiumImage.FileName).ToLower();
                    long peso = clubInfoDto.StadiumImage.Length;

                    if (!(extension.Equals(".jpg") || extension.Equals(".png")))
                    {
                        return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.BADREQUEST, Message = "La imagen debe estar en formato .jpg o .png" };
                    }

                    if (peso > (2 * 1024 * 1024))
                    {
                        return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.BADREQUEST, Message = "La imagen debe pesar menos de 2MB" };
                    }

                    //SUBIR IMAGEN

                    Guid guid = Guid.NewGuid();
                    string fileName = $"{guid.ToString()}{extension}";
                    bool result = await helperFile.Upload(helperFile.GetPathStadiumImage(), fileName, clubInfoDto.StadiumImage);
                    if (!result)
                    {
                        return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Message = "Error al cargar la imagén" };
                    }

                    //CREACIÓN
                    ClubInformation clubInfo = mapper.Map<ClubInformation>(clubInfoDto);
                    clubInfo.StadiumImage = fileName;
                    clubInfo.FoundationDate = clubInfo.OpeningDate;
                    await clubInformationRepository.CreateClubInformation(clubInfo);

                    if (!await dbOperation.Save())
                    {
                        if (!helperFile.DeleteFile(helperFile.GetPathStadiumImage(), fileName))
                        {
                            return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Message = "Error al crear la información del estadio del club, no se pudo borrar la imagén subida." };
                        }
                        return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Message = "Error al crear la información del estadio del club" };
                    }

                }
                else
                {
                    //CREACIÓN
                    ClubInformation clubInfo = mapper.Map<ClubInformation>(clubInfoDto);
                    clubInfo.StadiumImage = null;
                    clubInfo.FoundationDate = clubInfo.OpeningDate;
                    await clubInformationRepository.CreateClubInformation(clubInfo);

                    if (!await dbOperation.Save())
                    {
                        return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Message = "Error al crear la información del estadio del club" };
                    }
                }
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Message = "Información del club creada exitosamente" };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Message = ex.Message };
            }
        }

        public async Task<ResponseObjectJsonDto> UpdateClubInformation(ClubInformationPatchDto clubInfoDto)
        {
            try
            {
                if (!await clubInformationRepository.ClubInformationExists(clubInfoDto.IdClubInformation))
                {
                    return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.BADREQUEST, Message = "No existe la información del club" };
                }

                ClubInformation clubInfo = mapper.Map<ClubInformation>(clubInfoDto);

                //BUSCAR IMAGEN NUEVA Y ACTUAL PARA VERIFICAR SI HAY QUE CAMBIARLA
                string pathImage = helperFile.GetPathStadiumImage();
                string currentFileName = await clubInformationRepository.GetClubInformation(clubInfo.IdClubInformation);

                if ((currentFileName == null || currentFileName == ""))
                {
                    if(clubInfoDto.StadiumImage != null)
                    {
                        //VALIDAR IMAGEN
                        string extension = System.IO.Path.GetExtension(clubInfoDto.StadiumImage.FileName).ToUpper();
                        long peso = clubInfoDto.StadiumImage.Length;

                        if (!(extension.Equals(".JPG") || extension.Equals(".PNG")))
                        {
                            return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.BADREQUEST, Message = "La imagen debe estar en formato .jpg o .png" };
                        }

                        if (peso > (2 * 1024 * 1024))
                        {
                            return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.BADREQUEST, Message = "La imagen debe pesar menos de 2MB" };
                        }

                        //GENERAR FILENAME DE IMAGEN
                        string newFileName = helperFile.GetFileName(clubInfoDto.StadiumImage);
                        clubInfo.StadiumImage = newFileName;

                        //GENERAR NUEVA IMAGEN
                        if (!await helperFile.Upload(pathImage, newFileName, clubInfoDto.StadiumImage))
                        {
                            return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Message = "Error al cargar la imagen" };
                        }
                    }
                }
                else
                {
                    if(clubInfo.StadiumImage== null)
                    {
                        clubInfo.StadiumImage = currentFileName;
                    }
                    else
                    {
                        //VALIDAR IMAGEN
                        string extension = System.IO.Path.GetExtension(clubInfoDto.StadiumImage.FileName).ToUpper();
                        long peso = clubInfoDto.StadiumImage.Length;

                        if (!(extension.Equals(".JPG") || extension.Equals(".PNG")))
                        {
                            return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.BADREQUEST, Message = "La imagen debe estar en formato .jpg o .png" };
                        }

                        if(peso > (2*1024*1024))
                        {
                            return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.BADREQUEST, Message = "La imagen debe pesar menos de 2MB" };
                        }

                        byte[] currentImage = helperFile.ConvertFileToArrayByte(pathImage, currentFileName);
                        byte[] newImage = helperFile.ConvertIFormFileToArrayByte(clubInfoDto.StadiumImage);

                        if (!newImage.SequenceEqual(currentImage))
                        {
                            //GENERAR FILENAME DE IMAGEN
                            string newFileName = helperFile.GetFileName(clubInfoDto.StadiumImage);
                            clubInfo.StadiumImage = newFileName;

                            //REEMPLAZA IMAGEN NUEVA POR LA ACTUAL
                            bool result = helperFile.ReplaceFileDecrypt(pathImage, currentFileName, newFileName, newImage);

                            if (!result)
                            {
                                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Message = "Error al cargar la imagen" };
                            }
                        }
                        else
                        {
                            clubInfo.StadiumImage = currentFileName;
                        }
                    }
                }

                //ACTUALIZACIÓN
                clubInformationRepository.UpdateClubInformation(clubInfo);

                if (!await dbOperation.Save())
                {
                    if ((currentFileName == null || currentFileName == ""))
                    {
                        if (clubInfo.StadiumImage != null)
                        {
                            if (!helperFile.DeleteFile(pathImage, clubInfo.StadiumImage))
                            {
                                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Message = "Error al recuperar la imagen original." };
                            }
                        }

                        return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Message = "Error al crear la información del estadio del club" };
                    }
                    else
                    {
                        if (clubInfo.StadiumImage != null)
                        {
                            //Verificar si la imagen se cambió en disco
                            byte[] currentImage = helperFile.ConvertFileToArrayByte(pathImage, currentFileName);
                            byte[] newImage = helperFile.ConvertIFormFileToArrayByte(clubInfoDto.StadiumImage);

                            if (!newImage.SequenceEqual(currentImage))
                            {
                                //Volver a la imagen original
                                if (!helperFile.ReplaceFileDecrypt(pathImage, clubInfo.StadiumImage, currentFileName, currentImage))
                                {
                                    return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Message = "Error al recuperar la imagen original." };
                                }
                            }
                        }

                        return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Message = "Error al crear la información del estadio del club" };
                    }
                }
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Message = "Información del estadio del club actualizada exitosamente" };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Message = ex.Message };
            }
        }

        public async Task<ResponseObjectJsonDto> UpdateFirstClub(ClubPatchDto clubDto, string encryptedIdUser)
        {
            using (IDbContextTransaction dbContextTransaction = await dbOperation.BeginTransaction())
            {
                try
                {
                    int idLoggedUser = Convert.ToInt32(crypto.DecryptText(encryptedIdUser));
                    string roles = await iRoleUserRepo.GetNameRoleByUser(idLoggedUser);
                    if (roles != "Responsable Club")
                    {
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "No posee los roles correspondientes" };
                    }
                    if (await clubRepository.ClubUpdateExistsCUIT(clubDto.IdClub, clubDto.CUIT))
                    {
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "El cuit del equipo ya esta registrado." };
                    }

                    if (await clubRepository.ClubUpdateExistsMail(clubDto.IdClub, clubDto.Email))
                    {
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "El email del equipo ya esta registrado." };
                    }

                    if (!await tournamentDivisionRepository.TournamentDivisionExistId(clubDto.IdTournamentDivision))
                    {
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "No existe el torneo seleccionado." };
                    }

                    // creacion club
                    Club clubBd = await clubRepository.GetClub(clubDto.IdClub);
                    Club club = mapper.Map<ClubPatchDto, Club>(clubDto);

                    IFormFile logo = clubDto.FileLogo;
                    string fileNameLogo = helperFile.GetFileName(logo);

                    if (!await helperFile.Upload(helperFile.GetPathClubImage(), fileNameLogo, logo))
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el logo." };
                    }

                    club.LogoClub = fileNameLogo;
                    club.IsEnabled = true;
                    club.IdLeague = clubBd.IdLeague;
                    clubRepository.UpdateClub(club);

                    if (!await dbOperation.Save())
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        if (helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                        }

                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el club" };
                    }

                    // creacion mandato
                    ClubMandate mandate = mapper.Map<ClubPatchDto, ClubMandate>(clubDto);
                    mandate.IdClub = club.IdClub;

                    await clubMandateRepository.CreateMandate(mandate);

                    if (!await dbOperation.Save())
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                        }
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el club" };
                    }

                    //CREACION EQUIPO, DIVISIONAL Y ASIGNACION DE TORNEO
                    foreach (var idStaff in clubDto.lstStaff)
                    {
                        ClubStaff staff = await clubStaffRepository.GetClubStaff(idStaff);
                        if (staff == null)
                        {
                            await dbOperation.Rollback(dbContextTransaction);
                            if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                            {
                                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                            }
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "No existe la división " + idStaff };
                        }
                        Team team = new()
                        {
                            IdClubStaff = staff.IdClubStaff,
                            IdTournamentDivision = clubDto.IdTournamentDivision,
                            IdClub = club.IdClub
                        };
                        await teamRepository.CreateTeam(team);
                        if (!await dbOperation.Save())
                        {
                            await dbOperation.Rollback(dbContextTransaction);
                            if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                            {
                                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                            }
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el equipo" };
                        }
                    }


                    // Creacion de Archivos
                    // ESTATUTO
                    IFormFile formFileStatute = clubDto.FileStatute;
                    string fileNameStatute = helperFile.GetFileName(formFileStatute);

                    if (!await helperFile.Upload(helperFile.GetPathClubFile(), fileNameStatute, formFileStatute))
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                        }
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el estatuto." };
                    }

                    ClubFile fileStatute = new()
                    {
                        IdClub = club.IdClub,
                        IdFileType = (int)CodeFileTypes.Estatuto,
                        FileName = fileNameStatute,
                    };

                    await clubFileRepository.CreateClubFile(fileStatute);

                    if (!await dbOperation.Save())
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameStatute))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el estatuto." };
                        }

                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el estatuto." };
                    }

                    // ACTA DE AUTORIDADES
                    IFormFile formFileAuthorities = clubDto.FileAuthorities;
                    string fileNameAuthorities = helperFile.GetFileName(formFileAuthorities);

                    if (!await helperFile.Upload(helperFile.GetPathClubFile(), fileNameAuthorities, formFileAuthorities))
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameStatute))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el estatuto." };
                        }
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el acta de autoridades." };
                    }

                    ClubFile fileAuthorities = new()
                    {
                        IdClub = club.IdClub,
                        IdFileType = (int)CodeFileTypes.ActaAutoridades,
                        FileName = fileNameAuthorities,
                    };

                    await clubFileRepository.CreateClubFile(fileAuthorities);

                    if (!await dbOperation.Save())
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameStatute))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el estatuto." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameAuthorities))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el acta de autoridades." };
                        }

                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el acta de autoridades." };
                    }

                    // DNI PRESIDENTE
                    IFormFile formFileDniPresident = clubDto.FileDniPresident;
                    string fileNameDniPresident = helperFile.GetFileName(formFileDniPresident);

                    if (!await helperFile.Upload(helperFile.GetPathClubFile(), fileNameDniPresident, formFileDniPresident))
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameStatute))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el estatuto." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameAuthorities))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el acta de autoridades." };
                        }
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el dni del presidente." };
                    }

                    ClubFile fileDniPresident = new()
                    {
                        IdClub = club.IdClub,
                        IdFileType = (int)CodeFileTypes.DniPresidente,
                        FileName = fileNameDniPresident,
                    };

                    await clubFileRepository.CreateClubFile(fileDniPresident);

                    if (!await dbOperation.Save())
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameStatute))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el estatuto." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameAuthorities))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el acta de autoridades." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameDniPresident))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el dni del presidente." };
                        }

                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el dni del presidente." };
                    }

                    // CUENTA BANCARIA
                    IFormFile formFileBankAccount = clubDto.FileBankAccount;
                    string fileNameBankAccount = helperFile.GetFileName(formFileBankAccount);

                    if (!await helperFile.Upload(helperFile.GetPathClubFile(), fileNameBankAccount, formFileBankAccount))
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameStatute))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el estatuto." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameAuthorities))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el acta de autoridades." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameDniPresident))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el dni del presidente." };
                        }
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar la cuenta bancaria." };
                    }

                    ClubFile fileBankAccount = new()
                    {
                        IdClub = club.IdClub,
                        IdFileType = (int)CodeFileTypes.CuentaBancaria,
                        FileName = fileNameAuthorities,
                    };

                    await clubFileRepository.CreateClubFile(fileBankAccount);

                    if (!await dbOperation.Save())
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameStatute))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el estatuto." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameAuthorities))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el acta de autoridades." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameDniPresident))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el dni del presidente." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameBankAccount))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar la cuenta bancaria." };
                        }

                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar la cuenta bancaria." };
                    }

                    // CUIT
                    IFormFile formFileCuit = clubDto.FileCuit;
                    string fileNameCuit = helperFile.GetFileName(formFileCuit);

                    if (!await helperFile.Upload(helperFile.GetPathClubFile(), fileNameCuit, formFileCuit))
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameStatute))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el estatuto." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameAuthorities))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el acta de autoridades." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameDniPresident))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el dni del presidente." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameBankAccount))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar la cuenta bancaria." };
                        }
                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el cuit." };
                    }

                    ClubFile fileCuit = new()
                    {
                        IdClub = club.IdClub,
                        IdFileType = (int)CodeFileTypes.Cuit,
                        FileName = fileNameCuit,
                    };

                    await clubFileRepository.CreateClubFile(fileCuit);

                    if (!await dbOperation.Save())
                    {
                        await dbOperation.Rollback(dbContextTransaction);
                        if (!helperFile.DeleteFile(helperFile.GetPathClubImage(), fileNameLogo))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el logo." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameStatute))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el estatuto." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameAuthorities))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el acta de autoridades." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameDniPresident))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el dni del presidente." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameBankAccount))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar la cuenta bancaria." };
                        }
                        if (helperFile.DeleteFile(helperFile.GetPathClubFile(), fileNameCuit))
                        {
                            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "No se pudo borrar el cuit." };
                        }

                        return new ResponseObjectJsonDto { Code = (int)CodeHTTP.CONFLICT, Message = "Error al guardar el cuit." };
                    }

                    await dbOperation.Commit(dbContextTransaction);
                }
                catch (Exception)
                {
                    await dbOperation.Rollback(dbContextTransaction);
                }
            }

            return new ResponseObjectJsonDto { Code = (int)CodeHTTP.OK, Message = "Club creado." };

        }

        public async Task<ResponseObjectJsonDto> GetClubByResponsibleId(string encryptedIdUser)
        {
            int idLoggedUser = Convert.ToInt32(crypto.DecryptText(encryptedIdUser));

            Club club = await clubRepository.GetClubByResponsibleId(idLoggedUser);

            if (club == null)
            {
                return new ResponseObjectJsonDto
                {
                    Code = (int)CodeHTTP.BADREQUEST,
                    Message = "El usuario no es responsable de club"
                };
            }

            return new ResponseObjectJsonDto
            {
                Code = (int)CodeHTTP.OK,
                Message = "Club encontrado",
                Response = club
            };
        }

        public async Task<ResponseObjectJsonDto> GetLeagueClubs(string encryptedIdUser)
        {
            int idUser = Convert.ToInt32(crypto.DecryptText(encryptedIdUser));

            ResponsibleXLeague responsibleXLeague = await responsibleLeagueRepo.GetResponsibleXLeague(idUser);

            if (responsibleXLeague == null)
            {
                return new ResponseObjectJsonDto
                {
                    Code = (int)CodeHTTP.BADREQUEST,
                    Message = "El usuario no es responsable de ligas"
                };
            }

            List<ClubGetDto> lstLeagueClubs = await clubRepository.GetLeagueClubs(responsibleXLeague.IdLeague);

            if (lstLeagueClubs.Count == 0)
            {
                return new ResponseObjectJsonDto
                {
                    Code = (int)CodeHTTP.OK,
                    Message = "La consulta no arroja resultados"
                };
            }

            return new ResponseObjectJsonDto
            {
                Code = (int)CodeHTTP.OK,
                Message = "Consulta exitosa",
                Response = lstLeagueClubs
            };
        }
    }
}
