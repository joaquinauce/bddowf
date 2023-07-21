using WebAPIAFA.Models.Dtos.PaginationDtos;

namespace WebAPIAFA.Models.Dtos.PlayerDtos
{
    public class PlayerFiltersDto
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Cuil { get; set; }
        public int? IdClub { get; set; }
        public int? IdLeague { get; set; }
        public bool? IsSanctioned { get; set; }
        public string? ContractStatus { get; set; }
        public PaginationDto? Pagination { get; set; }

    }
}
