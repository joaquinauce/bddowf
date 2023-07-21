namespace WebAPIAFA.Models.Dtos.FileDtos
{
    public class FileGetDto
    {
        public int IdFile { get; set; }
        public int IdDocument { get; set; }
        public byte[] OriginalFile { get; set; }
        public byte[] ActualFile { get; set; }
        public string FileName { get; set; }
    }
}
