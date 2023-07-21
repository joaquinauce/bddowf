using WebAPIAFA.Models.Dtos.PaginationDtos;

namespace WebAPIAFA.Helpers.Pagination
{
    public static class ListExtensions
    {
        public static IList<T> Page<T>(this IList<T> queryable, PaginationDto paginationDTO)
        {
            return queryable
                .Skip((paginationDTO.Page - 1) * paginationDTO.AmountRegistersPage)
                .Take(paginationDTO.AmountRegistersPage).ToList();
        }
    }
}
