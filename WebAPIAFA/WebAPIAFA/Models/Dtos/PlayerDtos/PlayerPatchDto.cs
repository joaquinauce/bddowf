using System.ComponentModel.DataAnnotations;
using WebAPIAFA.Helpers.Validations;

namespace WebAPIAFA.Models.Dtos.PlayerDtos
{
    public class PlayerPatchDto
    {
        [Required(ErrorMessage = "El campo Cuil es obligatorio")]
        public string Cuil { get; set; }
        [Required(ErrorMessage = "El campo Genero es obligatorio")]
        public int IdGender { get; set; }
        [Required(ErrorMessage = "El campo Tipo de documento es obligatorio")]
        public int IdDocumentType { get; set; }
        [Required(ErrorMessage = "El campo Numero de documento es obligatorio")]
        public string DocumentNumber { get; set; }
        [Required(ErrorMessage = "El campo Fecha de nacimiento es obligatorio")]
        [AllowedDateTime]
        public DateTime BirthDate { get; set; }
        [Required(ErrorMessage = "El campo Nombre es obligatorio")]
        public string Name { get; set; }
        [Required(ErrorMessage = "El campo Apellido es obligatorio")]
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        [Required(ErrorMessage = "La imagén es requerida.")]
        [AllowedExtensionsFile(new string[] { ".jpg", ".png" })]
        [MaxSizeFile(2 * 1024 * 1024)] // 2Mb
        public IFormFile Image { get; set; }

        [Required(ErrorMessage = "El campo Localidad es obligatorio")]
        public int IdLocation { get; set; }

        [Required(ErrorMessage = "El campo Tipo de usuario es obligatorio")]
        public int IdUserType { get; set; }
        [Required(ErrorMessage = "El campo Club es obligatorio")]
        public int IdClub { get; set; }
        [Required(ErrorMessage = "El campo Staff es obligatorio.")]
        public int IdStaff { get; set; }
    }
}
