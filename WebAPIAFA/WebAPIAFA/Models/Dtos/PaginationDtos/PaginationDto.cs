namespace WebAPIAFA.Models.Dtos.PaginationDtos
{
    public class PaginationDto
    {
        public bool IsPaginated { get; set; } = false;
        public int Page { get; set; } = 1;
        private int AumontPerPage = 10;
        private readonly int MaximumAumontPerPage = 50;

        public int AmountRegistersPage
        {
            get => AumontPerPage;
            set
            {
                AumontPerPage = (value > MaximumAumontPerPage) ? MaximumAumontPerPage : value;
            }
        }
    }
}
