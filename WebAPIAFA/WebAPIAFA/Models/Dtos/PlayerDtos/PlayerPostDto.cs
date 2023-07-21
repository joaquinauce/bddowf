using System.ComponentModel.DataAnnotations;
using WebAPIAFA.Helpers.Validations;

namespace WebAPIAFA.Models.Dtos.PlayerDtos
{
    public class PlayerPostDto
    {
        public string? Cuil { get; set; }
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
        [Required(ErrorMessage = "El campo Mail es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo electronico no cumple con el formato deseado")]
        public string Mail { get; set; }
        public IFormFile Image { get; set; }

        [Required(ErrorMessage = "El campo Localidad es obligatorio")]
        public int IdLocation { get; set; }

        [Required(ErrorMessage = "El campo Tipo de usuario es obligatorio")]
        public int IdUserType { get; set; }
        [Required(ErrorMessage = "El campo Club es obligatorio")]
        public int IdClub { get; set; }
        [Required(ErrorMessage = "El campo Fifa Id es obligatorio.")]
        public string FifaId { get; set; }
        [Required(ErrorMessage = "El campo Comet Id es obligatorio.")]
        public string CometId { get; set; }
        public string TmsId { get; set; }
        public string CardNumber { get; set; }
        [Required(ErrorMessage = "El campo Staff es obligatorio.")]
        public int IdStaff { get; set; }

    }
}
