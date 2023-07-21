using WebAPIAFA.Helpers;
using WebAPIAFA.Models.Dtos.ClubAuthorityDtos;

namespace WebAPIAFA.Services.IServices
{
    public interface IClubAuthorityService
    {
        public Task<ResponseObjectJsonDto> GetMandateAuthorities(int idClub);
        public Task<ResponseObjectJsonDto> CreateClubAuthority(ClubAuthorityCreateDto clubAuthorityDto);
        public Task<ResponseObjectJsonDto> UpdateClubAuthority(ClubAuthorityPatchDto clubAuthorityPatchDto);
    }
}
