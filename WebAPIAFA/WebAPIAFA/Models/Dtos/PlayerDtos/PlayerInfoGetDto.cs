namespace WebAPIAFA.Models.Dtos.PlayerDtos
{
    public class PlayerInfoGetDto
    {
        public int IdUser { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Cuil { get; set; }
        public string Image { get; set; }
        public string Position { get; set; }
        public string FifaId { get; set; }
        public string CometId { get; set; }
        public string TmsId { get; set; }
        public string CardNumber { get; set; }
        public IList<string> Roles { get; set; }

    }
}
