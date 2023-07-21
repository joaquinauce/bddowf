using WebAPIAFA.Models.Dtos.PaginationDtos;

namespace WebAPIAFA.Models.Dtos.PassDtos
{
    public class PassPlayerFilterDto
    {
        public int? IdPassType { get; set; }
        public bool? IsCurrent { get; set; }
        public PaginationDto Pagination { get; set; }
    }
}
