using WebAPIAFA.Models.Dtos.DocumentDtos;
using WebAPIAFA.Models.Dtos.UserDtos;

namespace WebAPIAFA.Models.Dtos.PassDtos
{
    public class PassGetDto
    {
        public int IdPass { get; set; }
        public string Name { get; set; }
        public string? LogoClub { get; set; }
        public DateTime? BeginDate { get; set; }
        public bool? Accepted { get; set; }
        public string? State { get; set; }
        public int? IdPassType { get; set; }
        public DateTime? EndDate { get; set; }
        public int? IdLeague { get; set; }
        public int? IdClub { get; set; }
    }
}
