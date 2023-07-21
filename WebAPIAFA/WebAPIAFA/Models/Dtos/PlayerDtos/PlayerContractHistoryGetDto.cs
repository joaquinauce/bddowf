namespace WebAPIAFA.Models.Dtos.PlayerDtos
{
    public class PlayerContractHistoryGetDto
    {
        public int IdFolder { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string PlayerName { get; set; }
        public string PlayerLastName { get; set; }
        public string ClubName { get; set; }
        public string ClubLogo { get; set; }
    }
}
