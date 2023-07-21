using AutoMapper;
using WebAPIAFA.Models;
using WebAPIAFA.Models.Dtos.CountriesDto;
using WebAPIAFA.Models.Dtos.LocationDtos;
using WebAPIAFA.Models.Dtos.ProvincesDto;
using WebAPIAFA.Models.Dtos.SponsorsDto;
using WebAPIAFA.Models.Dtos.RolesDto;
using WebAPIAFA.Models.Dtos.UserDtos;
using WebAPIAFA.Models.Dtos.UserTypeDtos;
using WebAPIAFA.Models.Dtos.ClubDtos;
using WebAPIAFA.Models.Dtos.ClubAuthorityDtos;
using WebAPIAFA.Models.Dtos.ClubStaffDtos;
using WebAPIAFA.Models.Dtos.TournamentDivisionDtos;
using WebAPIAFA.Models.Dtos.FolderDtos;
using WebAPIAFA.Models.Dtos.ClubSponsorsDto;
using WebAPIAFA.Models.Dtos.ClubSponsorDtos;
using WebAPIAFA.Models.Dtos.ClubInformationDtos;
using WebAPIAFA.Models.Dtos.TeamDtos;
using WebAPIAFA.Models.Dtos.GenderDtos;
using WebAPIAFA.Models.Dtos.PlayerDtos;
using WebAPIAFA.Models.Dtos.BulletinDtos;
using WebAPIAFA.Models.Dtos.BulletinTypeDtos;
using WebAPIAFA.Models.Dtos.LeagueDtos;
using WebAPIAFA.Models.Dtos.StampRequestDtos;
using WebAPIAFA.Models.Dtos.PassDtos;
using WebAPIAFA.Models.Dtos.SanctionDtos;

namespace WebAPIAFA.Helpers.Mapper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Bulletin, BulletinGetDto>();
            CreateMap<BulletinCreateDto, Bulletin>();
            CreateMap<BulletinType, BulletinTypeGetDto>();
            CreateMap<Club, ClubInfoGetDto>();
            CreateMap<Club, ClubByTournamentGetDto>();
            CreateMap<Club, ClubLeagueGetDto>().ForMember(dest =>
                    dest.League,
                    opt => opt.MapFrom(src => src.League.Name));
            CreateMap<ClubAuthorityCreateDto, ClubAuthority>();
            CreateMap<ClubAuthorityPatchDto, ClubAuthority>();
            CreateMap<ClubCreateDto, Club>();
            CreateMap<ClubCreateDto, ClubMandate>()
                .ForMember(dest =>
                    dest.StartMandate,
                    opt => opt.MapFrom(src => src.Mandate.StartMandate))
                .ForMember(dest =>
                    dest.MandateExpiration,
                    opt => opt.MapFrom(src => src.Mandate.MandateExpiration))
                .ForMember(dest =>
                    dest.SignatoryClause,
                    opt => opt.MapFrom(src => src.Mandate.SignatoryClause));
            CreateMap<ClubInformation, ClubInformationGetDto>();
            CreateMap<ClubInformationCreateDto, ClubInformation>();
            CreateMap<ClubInformationPatchDto, ClubInformation>();
            CreateMap<ClubPatchDto, ClubMandate>()
                .ForMember(dest =>
                    dest.StartMandate,
                    opt => opt.MapFrom(src => src.Mandate.StartMandate))
                .ForMember(dest =>
                    dest.MandateExpiration,
                    opt => opt.MapFrom(src => src.Mandate.MandateExpiration))
                .ForMember(dest =>
                    dest.SignatoryClause,
                    opt => opt.MapFrom(src => src.Mandate.SignatoryClause));
            CreateMap<ClubPatchDto, Club>();
            CreateMap<ClubSponsorPostDto, ClubSponsor>();
            CreateMap<ClubSponsor, SponsorsByClubGetDto>()
                    .ForMember(dest =>
                    dest.name,
                    opt => opt.MapFrom(src => src.sponsor.Name))
                    .ForMember(dest =>
                    dest.image,
                    opt => opt.MapFrom(src => src.sponsor.Image));
            CreateMap<ClubStaff, ClubStaffGetDto>();
            CreateMap<Country, CountryGetDto>();
            CreateMap<Folder, FolderInfoGetDto>();
            CreateMap<FolderCreateDto, Folder>();
            CreateMap<Gender, GenderGetDto>();
            CreateMap<LeaguePostDto, League>();
            CreateMap<LeaguePatchDto, League>();
            CreateMap<Location, LocationGetDto>();
            CreateMap<Pass, PassDetailGetDto>()
                .ForMember(dest =>
                dest.TransferorClubName,
                opt => opt.MapFrom(src => src.ClubFrom.BussinessName))
                .ForMember(dest =>
                dest.TransferorClubLogo,
                opt => opt.MapFrom(src => src.ClubFrom.LogoClub))
                .ForMember(dest =>
                dest.AcceptingClubName,
                opt => opt.MapFrom(src => src.ClubTo.BussinessName))
                .ForMember(dest =>
                dest.AcceptingClubLogo,
                opt => opt.MapFrom(src => src.ClubTo.LogoClub))
                .ForMember(dest =>
                dest.PlayerName,
                opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest =>
                dest.PlayerLastName,
                opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest =>
                dest.PlayerCuil,
                opt => opt.MapFrom(src => src.User.Cuil))
                .ForMember(dest =>
                dest.TransferorLeagueName,
                opt => opt.MapFrom(src => src.ClubFrom.League.Name))
                .ForMember(dest =>
                dest.TransferorLeagueLogo,
                opt => opt.MapFrom(src => src.ClubFrom.League.Image))
                .ForMember(dest =>
                dest.AcceptingLeagueName,
                opt => opt.MapFrom(src => src.ClubTo.League.Name))
                .ForMember(dest =>
                dest.AcceptingLeagueLogo,
                opt => opt.MapFrom(src => src.ClubTo.League.Image))
                .ForMember(dest =>
                dest.EndDate,
                opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest =>
                dest.PassType,
                opt => opt.MapFrom(src => src.PassType.Name));
            CreateMap<Pass, PassPlayerGetDto>()
                .ForMember(dest =>
                dest.TransferorClubName,
                opt => opt.MapFrom(src => src.ClubFrom.BussinessName))
                .ForMember(dest =>
                dest.TransferorClubLogo,
                opt => opt.MapFrom(src => src.ClubFrom.LogoClub))
                .ForMember(dest =>
                dest.AcceptingClubName,
                opt => opt.MapFrom(src => src.ClubTo.BussinessName))
                .ForMember(dest =>
                dest.AcceptingClubLogo,
                opt => opt.MapFrom(src => src.ClubTo.LogoClub))
                .ForMember(dest =>
                dest.EndDate,
                opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest =>
                dest.PassType,
                opt => opt.MapFrom(src => src.PassType.Name));
            CreateMap<Player, PlayerInfoGetDto>()
                .ForMember(dest =>
                    dest.Name,
                    opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest =>
                    dest.LastName,
                    opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest =>
                    dest.Image,
                    opt => opt.MapFrom(src => src.User.Image))
                .ForMember(dest =>
                    dest.Position,
                    opt => opt.MapFrom(src => src.User.UserType.Detail))
                .ForMember(dest =>
                    dest.Cuil,
                    opt => opt.MapFrom(src => src.User.Cuil));
            CreateMap<PlayerPatchDto, User>();
            CreateMap<PlayerPostDto, User>();
            CreateMap<PlayerPostDto, Player>();
            CreateMap<Province, ProvinceGetDto>();
            CreateMap<Role, RoleGetDto>();
            CreateMap<Sanction, SanctionGetDto>()
                .ForMember(dest =>
                    dest.User,
                    opt => opt.MapFrom(src => src.User.Name + " " + src.User.LastName))
                .ForMember(dest =>
                    dest.Reason,
                    opt => opt.MapFrom(src => src.Reason.Description))
                .ForMember(dest =>
                    dest.TournamentDivision,
                    opt => opt.MapFrom(src => src.TournamentDivision.Name));
            CreateMap<Sanction, SanctionGralDto>()
                .ForMember(dest =>
                    dest.FullName,
                    opt => opt.MapFrom(src => src.User.Name + " " + src.User.LastName))
                .ForMember(dest =>
                    dest.Cuil,
                    opt => opt.MapFrom(src => src.User.Cuil))
                .ForMember(dest =>
                    dest.Reason,
                    opt => opt.MapFrom(src => src.Reason.Description))
                .ForMember(dest =>
                    dest.TournamentDivision,
                    opt => opt.MapFrom(src => src.TournamentDivision.Name));
            CreateMap<SanctionPostDto, Sanction>();
            CreateMap<SanctionPatchDto, Sanction>();
            CreateMap<Models.StampRequest, StampRequestGetDto>();
            CreateMap<Sponsor, SponsorCreateDto>().ReverseMap();
            CreateMap<SponsorGetDto, Sponsor>();
            CreateMap<TeamCreateDto, Team>();
            CreateMap<TeamPlayer, TeamGetDto>()
               .ForMember(dest =>
               dest.NamePlayer,
               opt => opt.MapFrom(src => src.User.Name))
               .ForMember(dest =>
               dest.LastNamePlayer,
               opt => opt.MapFrom(src => src.User.LastName))
               .ForMember(dest =>
               dest.BirthDate,
               opt => opt.MapFrom(src => src.User.BirthDate))
               .ForMember(dest =>
               dest.IdUser,
               opt => opt.MapFrom(src => src.IdUser))
               .ForMember(dest =>
               dest.BussinessName,
               opt => opt.MapFrom(src => src.Team.Club.BussinessName))
               .ForMember(dest =>
               dest.NameTournament,
               opt => opt.MapFrom(src => src.Team.TournamentDivision.Name));
            CreateMap<TournamentDivisionPostDto, TournamentDivision>();
            CreateMap<TournamentDivision, TournamentDivisionGetDto>();
            CreateMap<User, UserContractGetDto>().ForMember(dest =>
                    dest.BussinessName,
                    opt => opt.MapFrom(src => src.Club.BussinessName))
                    .ForMember(dest =>
                    dest.LogoClub,
                    opt => opt.MapFrom(src => src.Club.LogoClub))
                    .ForMember(dest =>
                    dest.Position,
                    opt => opt.MapFrom(src => src.UserType.Detail))
                    .ForMember(dest =>
                    dest.Country,
                    opt => opt.MapFrom(src => src.Location.Province.Country.Name));
            CreateMap<User, UserInfoGetDto>().ForMember(dest =>
                    dest.Usertype,
                    opt => opt.MapFrom(src => src.UserType));
            CreateMap<User, PlayerClubViewDto>();
            CreateMap<User, UsersTypeGetDto>()
                .ForMember(dest =>
                    dest.Detail,
                    opt => opt.MapFrom(src => src.UserType.Detail));
            CreateMap<User, UserByTypeUserGetDto>();
            CreateMap<User, UserCuilNameGetDto>().ForMember(dest =>
                    dest.FullName,
                    opt => opt.MapFrom(src => src.Name + " " + src.LastName));
            CreateMap<User, UserProfileGetDto>();
            CreateMap<User, UserGetAllDto>();
            CreateMap<UserChangePasswordDto, User>();
            CreateMap<UserType, UserTypeGetDto>();
            CreateMap<UserType, DetailGetDto>();
            CreateMap<UserType, UserTypeInfoGetDto>();
            CreateMap<UserPatchDto, User>();
            CreateMap<UserPostDto, User>();
            CreateMap<UserFastPostDto, User>();
            CreateMap<User, PlayerClubInfoGetDto>().ForMember(dest =>
                   dest.BussinessNameClub,
                   opt => opt.MapFrom(src => src.Club.BussinessName))
                .ForMember(dest =>
                   dest.LogoClub,
                   opt => opt.MapFrom(src => src.Club.LogoClub))
                .ForMember(dest =>
                   dest.BussinessNameLeague,
                   opt => opt.MapFrom(src => src.Club.League.Name))
                .ForMember(dest =>
                   dest.LogoLeague,
                   opt => opt.MapFrom(src => src.Club.League.Image));
            CreateMap<User, PlayerContractGetDto>()
                .ForMember(dest =>
                dest.BussinessName,
                opt => opt.MapFrom(src => src.Club.BussinessName))
                .ForMember(dest =>
                dest.LogoClub,
                opt => opt.MapFrom(src => src.Club.LogoClub));
            
        }
    }
}
