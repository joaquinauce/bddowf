using System.ComponentModel.DataAnnotations;
using WebAPIAFA.Helpers.Validations;

namespace WebAPIAFA.Models.Dtos.PassDtos
{
    public class PassCreateDto
    {
        [Required(ErrorMessage = "El identificador del usuario es obligatorio")]
        public int IdUser { get; set; }
        [Required(ErrorMessage = "El identificador del tipo de pase es obligatorio")]
        public int IdPassType { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage ="Debe ingresar al menos un documento para poder continuar con la solicitud de pase")]
        [EnsureMinimumElements(1)]
        public List<IFormFile> Files { get; set; }
    }
}
