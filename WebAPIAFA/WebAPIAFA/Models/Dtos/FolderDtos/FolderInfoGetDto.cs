namespace WebAPIAFA.Models.Dtos.FolderDtos
{
    public class FolderInfoGetDto
    {
        public int IdFolder { get; set; }
        public string Name { get; set; }
        public string Observations { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CancelationDate { get; set; }
        public string FileName { get; set; }
        public int IdDocument { get; set; }
    }
}
