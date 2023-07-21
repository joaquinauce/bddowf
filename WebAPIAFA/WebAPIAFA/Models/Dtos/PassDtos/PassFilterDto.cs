using WebAPIAFA.Models.Dtos.PaginationDtos;

namespace WebAPIAFA.Models.Dtos.PassDtos
{
    public class PassFilterDto
    {
        public string? Name { get; set; }
        public int? IdPassType { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? IdLeague { get; set; }
        public int? IdClub { get; set; }
        public PaginationDto? Pagination { get; set; }
    }
}
