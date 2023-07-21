using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebAPIAFA.Helpers;
using WebAPIAFA.Helpers.Enum;
using WebAPIAFA.Helpers.File;
using WebAPIAFA.Models.Dtos.PlayerDtos;
using WebAPIAFA.Models.Dtos.UserDtos;
using WebAPIAFA.Models.Dtos.UserFirstLoginDtos;
using WebAPIAFA.Services.IServices;

namespace WebAPIAFA.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]

    public class UserController : ControllerBase
    {
        private readonly ILeagueService iLeagueServ;
        private readonly IUserService iUserServ;

        public UserController( ILeagueService iLeagueServ, IUserService iUserServ)
        {
            this.iLeagueServ = iLeagueServ;
            this.iUserServ = iUserServ;
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseObjectJsonDto>> Login(UserLoginDto userLoginDto)
        {
            string userName = userLoginDto.User;
            string password = userLoginDto.Password;

            ResponseObjectJsonDto response = await iUserServ.Login(userName, password);

            if (response.Code == (int)CodeHTTP.OK)
            {
                return Ok(response);
            }
            else
            {
                return NotFound(response);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseObjectJsonDto>> CreateUser([FromForm] UserPostDto userPostDto)
        {
            ResponseObjectJsonDto response = await iUserServ.CreateUser(userPostDto);

            if (response.Code == (int)CodeHTTP.BADREQUEST)
            {
                return BadRequest(response);
            }

            if (response.Code == (int)CodeHTTP.INTERNALSERVER)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("user/fast")]
        public async Task<ActionResult<ResponseObjectJsonDto>> CreateFastUser([FromForm] UserFastPostDto userFastPostDto)
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;

            ResponseObjectJsonDto response = await iUserServ.CreateFastUser(userFastPostDto, encryptedIdUser);

            if (response.Code == (int)CodeHTTP.BADREQUEST)
            {
                return BadRequest(response);
            }

            if (response.Code == (int)CodeHTTP.INTERNALSERVER)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("player")]
        public async Task<ActionResult<ResponseObjectJsonDto>> CreatePlayer([FromForm] PlayerPostDto playerPostDto)
        {
            ResponseObjectJsonDto response = await iUserServ.CreatePlayer(playerPostDto);

            if (response.Code == (int)CodeHTTP.BADREQUEST)
            {
                return BadRequest(response);
            }

            if (response.Code == (int)CodeHTTP.INTERNALSERVER)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("password/reset/mail/{mail}")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseObjectJsonDto>> SendMailPasswordReset(string mail)
        {
            ResponseObjectJsonDto response = await iUserServ.SendMailPasswordReset(mail);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("userRolesAssign")]
        public async Task<ActionResult> userRolesAssign(int idUser, [FromBody] List<int> lstIdRoles)
        {
            ResponseObjectJsonDto response = await iUserServ.userRoleAssign(idUser, lstIdRoles);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("userMassiveUpload")]
        public async Task<ActionResult> UserMassiveUpload(IFormFile file)
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;

            ResponseObjectJsonDto response = await iUserServ.UserMassiveUpload(file, encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }
            return Ok(response);
        }

        [HttpPatch]
        [Route("password/reset")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseObjectJsonDto>> PasswordReset([FromBody] UserResetPasswordDto userResetPasswordDto)
        {
            ResponseObjectJsonDto response = await iUserServ.PasswordReset(userResetPasswordDto);

            if (response.Code == (int)CodeHTTP.BADREQUEST)
            {
                return BadRequest(response);
            }

            if (response.Code == (int)CodeHTTP.NOTFOUND)
            {
                return NotFound(response);
            }

            if (response.Code == (int)CodeHTTP.INTERNALSERVER)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }

        [HttpPatch]
        [Route("unsuscribe/{idUser:int}")]
        public async Task<ActionResult> UnsuscribeUser(int idUser)
        {
            ResponseObjectJsonDto response = await iUserServ.UnsuscribeUser(idUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpPatch]
        [Route("rehab/{idUser:int}")]
        public async Task<ActionResult> RehabUser(int idUser)
        {
            ResponseObjectJsonDto response = await iUserServ.RehabUser(idUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }        

        [HttpPatch]
        [Route("FirstLogin/AFA")]
        public async Task<ActionResult> FirstLoginAFA([FromForm] FirstLoginAFADto firstLoginAFA)
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;

            ResponseObjectJsonDto response = await iUserServ.FirstLoginAFA(firstLoginAFA, encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }
            return Ok(response);
        }

        [HttpPatch]
        [Route("FirstLogin/League")]
        public async Task<ActionResult> FirstLoginLeague([FromForm] FirstLoginLeagueDto firstLoginLeagueDto)
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;

            ResponseObjectJsonDto response = await iUserServ.FirstLoginLeague(firstLoginLeagueDto, encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }
            return Ok(response);
        }

        [HttpPatch]
        [Route("FirstLogin/Club")]
        public async Task<ActionResult> FirstLoginClub([FromForm] FirstLoginClubDto firstLoginClubDto)
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;

            ResponseObjectJsonDto response = await iUserServ.FirstLoginCLub(firstLoginClubDto, encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }
            return Ok(response);
        }

        [HttpPatch]
        [Route("FirstLogin/Player")]
        public async Task<ActionResult<ResponseObjectJsonDto>> FirstLoginPlayer([FromForm] FirstLoginPlayerDto firstLoginPlayerDto)
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;

            ResponseObjectJsonDto response = await iUserServ.FirstLoginPlayer(firstLoginPlayerDto, encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }
            return Ok(response);
        }

        [HttpPatch]
        [Route("FirstLogin/Authority")]
        public async Task<ActionResult<ResponseObjectJsonDto>> FirstLoginAuthority(FirstLoginAuthorityDto firstLoginAuthorityDto)
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;

            ResponseObjectJsonDto response = await iUserServ.FirstLoginAuthority(firstLoginAuthorityDto, encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }
            return Ok(response);
        }

        [HttpPatch]
        [Route("FirstLogin/Discipline")]
        public async Task<ActionResult<ResponseObjectJsonDto>> FirstLoginDiscipline([FromForm] FirstLoginDisciplineDto firstLoginDisciplineDto)
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;

            ResponseObjectJsonDto response = await iUserServ.FirstLoginDiscipline(firstLoginDisciplineDto, encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }
            return Ok(response);
        }

        [HttpPatch]
        [Route("update")]
        public async Task<ActionResult> UserUpdate([FromForm] UserPatchDto userUpdateDto) 
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;

            ResponseObjectJsonDto response = await iUserServ.UserUpdate(userUpdateDto, encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }        

        [HttpPatch]
        [Route("player/update")]
        public async Task<ActionResult<ResponseObjectJsonDto>> UpdatePlayer([FromForm] PlayerPatchDto playerUpdateDto)
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;

            ResponseObjectJsonDto response = await iUserServ.UpdateFirstPlayer(playerUpdateDto, encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }
            return Ok(response);
        }

        [HttpPatch]
        [Route("password/change")]
        public async Task<ActionResult<ResponseObjectJsonDto>> PasswordChange([FromBody] UserChangePasswordDto userChangePasswordDto)
        {
            string encryptedMail = ((ClaimsIdentity)User.Identity).FindFirst("mail").Value;

            ResponseObjectJsonDto response = await iUserServ.PasswordChange(userChangePasswordDto, encryptedMail);

            if (response.Code == (int)CodeHTTP.BADREQUEST)
            {
                return BadRequest(response);
            }

            if (response.Code == (int)CodeHTTP.NOTFOUND)
            {
                return NotFound(response);
            }

            if (response.Code == (int)CodeHTTP.INTERNALSERVER)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("players/contract")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetPlayersLastContract([FromQuery] PlayerFiltersDto playerFiltersDto)
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;

            ResponseObjectJsonDto response = await iUserServ.GetPlayerLastContract(encryptedIdUser, playerFiltersDto);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }        

        [HttpGet]
        [Route("{idClub:int}/{userType}")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetUsersByType(int idClub, string userType)
        {
            ResponseObjectJsonDto response = await iUserServ.GetUsersByType(idClub, userType);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("comision")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetComisionUsers()
        {
            ResponseObjectJsonDto response = await iUserServ.GetComisionUsers();

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }
        [HttpGet]
        [Route("comision/club")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetComisionUsersByClub()
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;
            ResponseObjectJsonDto response = await iUserServ.GetComisionUsersByClub(encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("player/image")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetImagePlayer()
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;
            ResponseObjectJsonDto response = await iUserServ.GetImagePlayer(encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("{idClub:int}")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetUsersByIdClub(int idClub)
        {
            ResponseObjectJsonDto response = await iUserServ.GetUsersByClub(idClub);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("userType/detail")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetDetailUserTypeByRole(int idRole)
        {
            ResponseObjectJsonDto response = await iUserServ.GetDetailUserTypeByRole(idRole);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("player")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetPlayers()
        {
            ResponseObjectJsonDto response = await iUserServ.GetPlayers();

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("player/{idClub:int}")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetPlayersByClub(int idClub)
        {
            ResponseObjectJsonDto response = await iUserServ.GetPlayersByClub(idClub);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("player/info")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetPlayersInformationByClub()
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;
            ResponseObjectJsonDto response = await iUserServ.GetPlayersInformationByClub(encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("cuil/{Cuil}")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetUsersByCuil([MinLength(3), MaxLength(11)] string Cuil)
        {
            ResponseObjectJsonDto response = await iUserServ.GetUsersByCuil(Cuil);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("allUsers")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetAllUsers([FromQuery] UserFilterDto filters)
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;

            ResponseObjectJsonDto response = await iUserServ.GetAllUsers(encryptedIdUser, filters);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("roles")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetRolesByUser(int idUser)
        {
            ResponseObjectJsonDto response = await iUserServ.GetRolesByUser(idUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("info")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetUserInfo()
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;
            ResponseObjectJsonDto response = await iUserServ.GetUserInfo(encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("profile")]
        public async Task<ActionResult<ResponseObjectJsonDto>> GetUserProfile()
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;

            ResponseObjectJsonDto response = await iUserServ.GetUserProfile(encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }  

        [HttpGet("player/cuil/{cuil}")]
        public async Task<ActionResult> GetPlayerByCuil([MinLength(11), MaxLength(11)] string cuil)
        {
            ResponseObjectJsonDto response = await iUserServ.GetPlayerByCuil(cuil);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("league/players/cuil/{cuil}")]
        public async Task<ActionResult> GetPlayersByLeagueResponsibleDiscipline([MinLength(3), MaxLength(11)] string cuil)
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;

            ResponseObjectJsonDto response = await iLeagueServ.GetPlayersByLeagueResponsibleDiscipline(cuil, encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response);
            }

            return Ok(response);
        }

        [HttpGet("player/notClub/{cuil}")]
        public async Task<ActionResult> GetPlayerNotClubByCuil([MinLength(3), MaxLength(11)] string cuil)
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;
            ResponseObjectJsonDto response = await iUserServ.GetNotClubPlayersByCuil(cuil, encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }
            return Ok(response);
        }

        [HttpGet("player/club/{Cuil}")]
        public async Task<ActionResult> GetClubPlayersByCuil([MinLength(3), MaxLength(11)] string Cuil)
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;
            ResponseObjectJsonDto response = await iUserServ.GetClubPlayersByCuil(Cuil, encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }
            return Ok(response);
        }

        [HttpGet("byId/{IdUser:int}")]
        public async Task<ActionResult> GetUserById(int IdUser)
        {

            ResponseObjectJsonDto response = await iUserServ.GetUserById(IdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }

        [HttpGet("leagueResponsibles")]
        public async Task<ActionResult> GetLeagueResponsibles()
        {
            string encryptedIdUser = ((ClaimsIdentity)User.Identity).FindFirst("idUser").Value;
 
            ResponseObjectJsonDto response = await iUserServ.GetLeagueResponsibles(encryptedIdUser);

            if (response.Code != (int)CodeHTTP.OK)
            {
                return StatusCode(response.Code, response.Message);
            }

            return Ok(response);
        }
    }
}
