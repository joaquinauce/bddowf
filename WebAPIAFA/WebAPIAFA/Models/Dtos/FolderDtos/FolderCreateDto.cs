using System.ComponentModel.DataAnnotations;
using WebAPIAFA.Helpers.Validations;

namespace WebAPIAFA.Models.Dtos.FolderDtos
{
    public class FolderCreateDto
    {
        public string Observations { get; set; }
        [Required(ErrorMessage = "La fecha de finalización de contrato es obligatoria.")]
        [DiferenceDate("BeginDate")]
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Los documentos son obligatorios")]
        [EnsureMinimumElements(1, ErrorMessage = "Debe ingresar al menos un documento para poder continuar con la solicitud de contrato.")]
        [Files(new string[] { ".pdf" }, 5 * 1024 * 1024)]
        public IList<IFormFile> Files { get; set; }
        [Required]
        [EnsureMinimumElements(1, ErrorMessage = "Debe ingresar al menos un usuario participante para poder continuar con la solicitud de contrato.")]
        public List<int> Users { get; set; }
        [Required(ErrorMessage = "El campo usuario afectado es obligatorio.")]
        public int IdAffectedUser { get; set; }
        [Required(ErrorMessage = "La fecha de inicio de contrato es obligatoria.")]
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
        public DateTime BeginDate { get; set; }
    }
}
