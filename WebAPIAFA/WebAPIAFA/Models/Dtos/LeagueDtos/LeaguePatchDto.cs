using System.ComponentModel.DataAnnotations;
using WebAPIAFA.Helpers.Validations;

namespace WebAPIAFA.Models.Dtos.LeagueDtos
{
    public class LeaguePatchDto
    {
        [Required(ErrorMessage = "El ID es requerido.")]
        public int IdLeague { get; set; }
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "El CUIT es requerido.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "CUIT debe contener solo 11 caracteres.")]
        public string CUIT { get; set; }
        public IFormFile Image { get; set; }
    }
}
