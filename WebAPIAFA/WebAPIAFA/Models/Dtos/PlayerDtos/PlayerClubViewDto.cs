namespace WebAPIAFA.Models.Dtos.PlayerDtos
{
    public class PlayerClubViewDto
    {
        public int IdUser { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Image { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int IdLastDocument { get; set; }
    }
}
