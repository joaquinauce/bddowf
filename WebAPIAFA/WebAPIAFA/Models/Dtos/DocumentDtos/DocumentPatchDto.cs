using System.ComponentModel.DataAnnotations;

namespace WebAPIAFA.Models.Dtos.DocumentDtos
{
    public class DocumentPatchDto
    {
        public int IdDocument { get; set; }
        [Required(ErrorMessage = "El campo CreationDate es obligatorio")]
        public DateTime CreationDate { get; set; }
        public string QRCode { get; set; }
        [Required(ErrorMessage = "El campo IdFolder es obligatorio")]
        public int IdFolder { get; set; }
        public string Observations { get; set; }
        [Required(ErrorMessage = "El campo Name es obligatorio")]
        public string Name { get; set; }
        public string RejectMotive { get; set; }
    }
}
