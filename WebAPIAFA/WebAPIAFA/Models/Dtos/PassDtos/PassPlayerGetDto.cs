namespace WebAPIAFA.Models.Dtos.PassDtos
{
    public class PassPlayerGetDto
    {
        public int IdPass { get; set; }
        public string TransferorClubName { get; set; }
        public string TransferorClubLogo { get; set; }
        public string AcceptingClubName { get; set; }
        public string AcceptingClubLogo { get; set; }
        public string PassType { get; set; }
        public DateTime EndDate { get; set; }
    }
}
