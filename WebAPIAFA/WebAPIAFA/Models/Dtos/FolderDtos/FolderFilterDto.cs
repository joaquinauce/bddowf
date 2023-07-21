using WebAPIAFA.Models.Dtos.PaginationDtos;

namespace WebAPIAFA.Models.Dtos.FolderDtos
{
    public class FolderFilterDto
    {
        public string? Name { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateUntil { get; set; }
        public bool? IsCurrent { get; set; }
        public PaginationDto? Pagination { get; set; }
    }
}
