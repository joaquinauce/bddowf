using AutoMapper;
using WebAPIAFA.Helpers;
using WebAPIAFA.Models.Dtos.CountriesDto;
using WebAPIAFA.Models;
using WebAPIAFA.Repository.IRepository;
using WebAPIAFA.Services.IServices;
using WebAPIAFA.Models.Dtos.ProvincesDto;
using WebAPIAFA.Models.Dtos.LocationDtos;
using WebAPIAFA.Models.Dtos.RolesDto;
using WebAPIAFA.Models.Dtos.UserTypeDtos;
using WebAPIAFA.Helpers.Enum;
using WebAPIAFA.Models.Dtos.GenderDtos;
using WebAPIAFA.Helpers.Crypto;
using WebAPIAFA.Models.Dtos.TournamentDivisionDtos;
using WebAPIAFA.Entity;

namespace WebAPIAFA.Services
{
    public class AdministrationService : IAdministrationService
    {
        private readonly ICountryRepository iCountryRepo;
        private readonly ICrypto crypto;
        private readonly IDbOperation iDbOperation;
        private readonly IDocumentTypeRepository iDocumentTypeRepo;
        private readonly IGenderRepository iGenderRepo;
        private readonly ILocationRepository iLocationRepo;
        private readonly IMapper mapper;
        private readonly IProvinceRepository iProvinceRepo;
        private readonly IReasonRepository iReasonRepo;
        private readonly IRoleRepository iRoleRepo;
        private readonly IRoleUserRepository iRoleUserRepo;
        private readonly ITournamentDivisionRepository iTournamentDivisionRepository;
        private readonly IUserTypeRepository iUserTypeRepo;

        public AdministrationService(ICountryRepository iCountryRepo, ICrypto crypto, IDbOperation iDbOperation,
            IDocumentTypeRepository iDocumentTypeRepo, IGenderRepository iGenderRepo, ILocationRepository iLocationRepo, IMapper mapper,
            IProvinceRepository iProvinceRepo, IReasonRepository iReasonRepo,
            IRoleRepository iRoleRepo, IRoleUserRepository iRoleUserRepo, ITournamentDivisionRepository iTournamentDivisionRepository, IUserTypeRepository iUserTypeRepo)
        {
            this.iCountryRepo = iCountryRepo;
            this.crypto = crypto;
            this.iDbOperation = iDbOperation;
            this.iDocumentTypeRepo = iDocumentTypeRepo;
            this.iGenderRepo = iGenderRepo;
            this.iLocationRepo = iLocationRepo;
            this.mapper = mapper;
            this.iProvinceRepo = iProvinceRepo;
            this.iReasonRepo = iReasonRepo;
            this.iRoleRepo = iRoleRepo;
            this.iRoleUserRepo = iRoleUserRepo;
            this.iTournamentDivisionRepository = iTournamentDivisionRepository;
            this.iUserTypeRepo = iUserTypeRepo;
        }

        public ICountryRepository ICountryRepo { get; }
        public IUserTypeRepository IUserTypeRepo { get; }

        public async Task<ResponseObjectJsonDto> CreateTournamentDivision(TournamentDivisionPostDto tournament)
        {
            try
            {

                if (await iTournamentDivisionRepository.TournamentDivisionExistName(tournament.Name))
                {
                    return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "Ya existe un torneo con el mismo nombre" };
                }

                TournamentDivision tournamentDivision = mapper.Map<TournamentDivisionPostDto, TournamentDivision>(tournament);

                await iTournamentDivisionRepository.CreateTournamentDivision(tournamentDivision);

                if (!await iDbOperation.Save())
                {
                    return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Message = "Error al crear el torneo." };
                }

                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Message = "Torneo creado exitosamente" };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }

        public async Task<ResponseObjectJsonDto> GetCountries()
        {
            try
            {
                IList<Country> listCountries = await iCountryRepo.GetCountries();
                IList<CountryGetDto> listCountriesResponse = mapper.Map<IList<Country>, IList<CountryGetDto>>(listCountries);

                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Response = listCountriesResponse };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }

        public async Task<ResponseObjectJsonDto> GetDocumentTypes()
        {
            try
            {
                List<DocumentType> lstDocType = await iDocumentTypeRepo.GetDocumentTypes();
                if (lstDocType == null || lstDocType.Count == 0)
                {
                    return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "La consulta no arroja resultados" };
                }

                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.OK, Response = lstDocType };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }

        public async Task<ResponseObjectJsonDto> GetEspeficRoles(string encryptedIdUser)
        {
            try
            {
                int idLoggedUser = Convert.ToInt32(crypto.DecryptText(encryptedIdUser));
                string role = await iRoleUserRepo.GetNameRoleByUser(idLoggedUser);
                IList<Role> listRoles = await iRoleRepo.GetRoles();
                if (listRoles == null)
                {
                    return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.BADREQUEST, Message = "No existen roles cargados en el sistema." };
                }
                if (role == null)
                {
                    return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.BADREQUEST, Message = "El usuario no posee roles." };
                }
                switch (role.ToUpper())
                {
                    case "RESPONSABLE AFA":
                        listRoles = listRoles.Where(x => x.Name.ToUpper().Equals("RESPONSABLE AFA") || x.Name.ToUpper().Equals("RESPONSABLE LIGA")).ToList();
                        break;
                    case "RESPONSABLE LIGA":
                        listRoles = listRoles.Where(x => x.Name.ToUpper().Equals("RESPONSABLE LIGA") || x.Name.ToUpper().Equals("RESPONSABLE CLUB")).ToList();
                        break;
                    case "RESPONSABLE CLUB":
                        listRoles = listRoles.Where(x => x.Name.ToUpper().Equals("RESPONSABLE CLUB") || x.Name.ToUpper().Equals("JUGADOR") || x.Name.ToUpper().Equals("DIRECTIVO")).ToList();
                        break;
                    default:
                        return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.BADREQUEST, Message = "El usuario no posee los roles correspondientes." };
                };
                IList<RoleGetDto> listRolesResponse = mapper.Map<IList<Role>, IList<RoleGetDto>>(listRoles);

                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Response = listRolesResponse };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }

        public async Task<ResponseObjectJsonDto> GetGenders()
        {
            try
            {
                IList<Gender> listGenders = await iGenderRepo.GetGenders();
                IList<GenderGetDto> listGendersResponse = mapper.Map<IList<Gender>, IList<GenderGetDto>>(listGenders);

                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Response = listGenders };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }

        public async Task<ResponseObjectJsonDto> GetLocationsByIdProvince(int idProvince)
        {
            try
            {
                IList<Location> listLocations = await iLocationRepo.GetLocationsByIdProvince(idProvince);
                IList<LocationGetDto> listLocationsResponse = mapper.Map<IList<Location>, IList<LocationGetDto>>(listLocations);

                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Response = listLocationsResponse };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }

        public async Task<ResponseObjectJsonDto> GetProvinciesByIdCountry(int idCountry)
        {
            try
            {
                IList<Province> listProvincies = await iProvinceRepo.GetProvincesByIdCountry(idCountry);
                IList<ProvinceGetDto> listProvinciesResponse = mapper.Map<IList<Province>, IList<ProvinceGetDto>>(listProvincies);

                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Response = listProvinciesResponse };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }

        public async Task<ResponseObjectJsonDto> GetReasons()
        {
            try
            {
                IList<Reason> listReasons = await iReasonRepo.GetReasons();

                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Response = listReasons };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }

        public async Task<ResponseObjectJsonDto> GetRoles()
        {
            try
            {
                IList<Role> listRoles = await iRoleRepo.GetRoles();
                IList<RoleGetDto> listRolesResponse = mapper.Map<IList<Role>, IList<RoleGetDto>>(listRoles);

                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Response = listRolesResponse };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }

        public async Task<ResponseObjectJsonDto> GetTournamentDivision(int idTournament)
        {
            TournamentDivision lstTournament = await iTournamentDivisionRepository.GetTournamentDivision(idTournament);

            if (lstTournament == null)
            {
                return new ResponseObjectJsonDto
                {
                    Code = (int)CodeHTTP.BADREQUEST,
                    Message = "El torneo no existe"
                };
            }

            return new ResponseObjectJsonDto
            {
                Code = (int)CodeHTTP.OK,
                Message = "Consulta exitosa",
                Response = lstTournament
            };
        }

        public async Task<ResponseObjectJsonDto> GetUserTypes()
        {
            string con = crypto.EncryptText("Server=10.1.1.102;Database=17-BddoWFApi-AFA-Demo;UserID=sa;Password=qwe-7890;TrustServerCertificate=True;MultipleActiveResultSets=true");
            try
            {
                IList<UserType> listUserTypes = await iUserTypeRepo.GetUserTypes();
                if (listUserTypes.Count == 0)
                {
                    return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Message = "No se encontraron registros" };
                }
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Response = listUserTypes };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }

        public async Task<ResponseObjectJsonDto> GetUserTypesByName(string name)
        {
            try
            {
                IList<UserType> listUserTypes = await iUserTypeRepo.GetUserTypesByName(name);
                IList<DetailGetDto> listUserTypesResponse = mapper.Map<IList<UserType>, IList<DetailGetDto>>(listUserTypes);

                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Response = listUserTypesResponse };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }
    }
}
