using WebAPIAFA.Models.Dtos.DocumentDtos;

namespace WebAPIAFA.Models.Dtos.PassDtos
{
    public class PassDetailGetDto
    {
        public int IdPass { get; set; }
        public string PlayerName { get; set; }
        public string PlayerLastName { get; set; }
        public string PlayerCuil { get; set; }
        public string TransferorClubName { get; set; }
        public string TransferorClubLogo { get; set; }
        public string TransferorLeagueName { get; set; }
        public string TransferorLeagueLogo { get; set; }
        public string AcceptingClubName { get; set; }
        public string AcceptingClubLogo { get; set; }
        public string AcceptingLeagueName { get; set; }
        public string AcceptingLeagueLogo { get; set; }
        public DateTime EndDate { get; set; }
        public string PassType { get; set; }
        public List<DocumentGetDto> Documents { get; set; } 
        public bool? Accepted { get; set; }

    }
}
