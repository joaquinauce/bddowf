namespace WebAPIAFA.Models.Dtos.PlayerDtos
{
    public class PlayerContractGetDto
    {
        public int IdUser { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Cuil { get; set; }
        public string DocumentNumber { get; set; }
        public string BussinessName { get; set; }
        public string LogoClub { get; set; }
        public int IdFolder { get; set; } 
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsSanctioned {  get; set; }
    }
}
