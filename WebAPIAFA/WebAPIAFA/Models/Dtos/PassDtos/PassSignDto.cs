using System.ComponentModel.DataAnnotations;

namespace WebAPIAFA.Models.Dtos.PassDtos
{
    public class PassSignDto
    {
        [Required(ErrorMessage = "El campo idDocument es requerido")]
        public int IdDocument { get; set; }
        [Required(ErrorMessage = "El campo PosX es requerido")]
        public int PosX { get; set; }
        [Required(ErrorMessage = "El campo PosY es requerido")]
        public int PosY { get; set; }
    }
}
