using AutoMapper;
using WebAPIAFA.Entity;
using WebAPIAFA.Helpers;
using WebAPIAFA.Helpers.Enum;
using WebAPIAFA.Models;
using WebAPIAFA.Models.Dtos.ClubAuthorityDtos;
using WebAPIAFA.Repository.IRepository;
using WebAPIAFA.Services.IServices;

namespace WebAPIAFA.Services
{
    public class ClubAuthorityService : IClubAuthorityService
    {
        private readonly IClubAuthorityRepository iClubAuthorityRepository;
        private readonly IClubRepository iClubRepository;
        private readonly IClubMandateRepository iClubMandateRepository;
        private readonly IDbOperation iDbOperation;
        private readonly IMapper mapper;
        private readonly IUserTypeRepository iUserTypeRepository;

        public ClubAuthorityService(IClubAuthorityRepository iClubAuthorityRepository,
            IClubRepository iClubRepository, IClubMandateRepository iClubMandateRepository, IDbOperation iDbOperation, IMapper mapper, IUserTypeRepository userTypeRepository)
        {
            this.iClubAuthorityRepository = iClubAuthorityRepository;
            this.iClubRepository = iClubRepository;
            this.iClubMandateRepository = iClubMandateRepository;
            this.iDbOperation = iDbOperation;
            this.mapper = mapper;
            this.iUserTypeRepository = userTypeRepository;
        }

        public async Task<ResponseObjectJsonDto> CreateClubAuthority(ClubAuthorityCreateDto clubAuthorityDto)
        {
            try
            {
                if (! await iClubMandateRepository.ClubMandateExistId(clubAuthorityDto.IdClubMandate))
                {
                    return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "El mandato no existe" };
                }

                if (!await iUserTypeRepository.UserTypeExistId(clubAuthorityDto.IdUserType))
                {
                    return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "El tipo de usuario no existe" };
                }

                if(await iClubAuthorityRepository.ClubAuthorityExistByFilters(clubAuthorityDto))
                {
                    return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "La autoridad ya se encuentra registrada" };
                }

                ClubAuthority clubAuthority = mapper.Map<ClubAuthority>(clubAuthorityDto);

                await iClubAuthorityRepository.CreateClubAuthority(clubAuthority);

                if (!await iDbOperation.Save())
                {
                    return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Message = "Error al crear la autoridad del club." };
                }

                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Message = "Autoridad creada exitosamente" };
            }
            catch(Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }

        public async Task<ResponseObjectJsonDto> GetMandateAuthorities(int idClub)
        {
            try
            {
                bool result = await iClubRepository.ClubExistId(idClub);
                if (!result)
                {
                    return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "El club no existe" };
                }

                List<ClubAuthority> lstClubMandateAuthorities = await iClubAuthorityRepository.GetMandateAuthorities(idClub);

                if (lstClubMandateAuthorities == null || lstClubMandateAuthorities.Count == 0)
                {
                    return new ResponseObjectJsonDto { Code = (int)CodeHTTP.NOTFOUND, Message = "La consulta no arroja resultados" };
                }

                ClubAuthorityGetDto ClubMandateAuthoritiesDto = new ClubAuthorityGetDto();
                ClubMandateAuthoritiesDto.Authorities = lstClubMandateAuthorities
                    .Select(cM => new NameAndHierarchyGetDto
                    {
                        Name = cM.FullName,
                        Hierarchy = cM.UserType.Detail,
                        IdHierarchy = cM.UserType.IdUserType
                    }).ToList();


                if (lstClubMandateAuthorities[0].ClubMandate.StartMandate != null)
                {
                    ClubMandateAuthoritiesDto.StartYear = ((DateTime)lstClubMandateAuthorities[0].ClubMandate.StartMandate).Year.ToString();
                }
                if (lstClubMandateAuthorities[0].ClubMandate.MandateExpiration != null)
                {
                    ClubMandateAuthoritiesDto.EndYear = ((DateTime)lstClubMandateAuthorities[0].ClubMandate.MandateExpiration).Year.ToString();
                }

                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.OK, Response = ClubMandateAuthoritiesDto };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }

        public async Task<ResponseObjectJsonDto> UpdateClubAuthority(ClubAuthorityPatchDto clubAuthorityPatchDto)
        {
            try
            {
                if(clubAuthorityPatchDto.FullName == null || clubAuthorityPatchDto.FullName == "")
                {
                    return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "Debe ingresar el nombre completo" };
                }

                if(!await iClubAuthorityRepository.ClubAuthorityExistById(clubAuthorityPatchDto.IdClubAuthority))
                {
                    return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "La autoridad no existe" };
                }

                if(await iClubAuthorityRepository.ClubAuthorityUpdateExistByFilters(clubAuthorityPatchDto))
                {
                    return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "Ya existe una autoridad registrada con los mismos datos" };
                }

                if (!await iClubMandateRepository.ClubMandateExistId(clubAuthorityPatchDto.IdClubMandate))
                {
                    return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "El mandato no existe" };
                }

                if (!await iUserTypeRepository.UserTypeExistId(clubAuthorityPatchDto.IdUserType))
                {
                    return new ResponseObjectJsonDto { Code = (int)CodeHTTP.BADREQUEST, Message = "El tipo de usuario no existe" };
                }

                ClubAuthority clubAuthority = mapper.Map<ClubAuthority>(clubAuthorityPatchDto);

                iClubAuthorityRepository.UpdateClubAuthority(clubAuthority);

                if (!await iDbOperation.Save())
                {
                    return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Message = "Error al actualizar la autoridad del club." };
                }

                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.OK, Message = "Autoridad actualizada exitosamente" };
            }
            catch (Exception ex)
            {
                return new ResponseObjectJsonDto() { Code = (int)CodeHTTP.INTERNALSERVER, Response = ex };
            }
        }
    }
}
