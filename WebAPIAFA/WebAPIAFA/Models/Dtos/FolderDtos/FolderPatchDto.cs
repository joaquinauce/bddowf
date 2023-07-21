using System.ComponentModel.DataAnnotations;

namespace WebAPIAFA.Models.Dtos.FolderDtos
{
    public class FolderPatchDto
    {
        public int IdFolder { get; set; }
        [Required(ErrorMessage = "El campo IdStep es obligatorio")]
        public int IdStep { get; set; }
        [Required(ErrorMessage = "El campo IdCreationUser es obligatorio")]
        public int IdCreationUser { get; set; }
        [Required(ErrorMessage = "El campo CreationDate es obligatorio")]
        public DateTime CreationDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CancelationDate { get; set; }
        public string Observations { get; set; }
        [Required(ErrorMessage = "El campo Name es obligatorio")]
        public string Name { get; set; }
    }
}
