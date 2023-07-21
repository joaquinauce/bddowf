namespace WebAPIAFA.Models.Dtos.PlayerDtos
{
    public class PlayerClubInfoGetDto
    {
        public int IdUser { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string BussinessNameClub { get; set; }
        public string LogoClub { get; set; }
        public string BussinessNameLeague { get; set; }
        public string LogoLeague { get; set; }
    }
}
