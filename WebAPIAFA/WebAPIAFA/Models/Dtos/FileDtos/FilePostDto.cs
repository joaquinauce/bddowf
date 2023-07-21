using System.ComponentModel.DataAnnotations;

namespace WebAPIAFA.Models.Dtos.FileDtos
{
    public class FilePostDto
    {
        public int IdDocument { get; set; }
        [Required(ErrorMessage = "El campo OriginalFile es obligatorio")]
        public byte[] OriginalFile { get; set; }
        [Required(ErrorMessage = "El campo ActualFile es obligatorio")]
        public byte[] ActualFile { get; set; }
        [Required(ErrorMessage = "El campo FileName es obligatorio")]
        public string FileName { get; set; }
    }
}
