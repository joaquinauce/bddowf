using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIAFA.Models
{
    public class Club
    {
        [Key]
        public int IdClub { get; set; }
        public string BussinessName { get; set; }
        public string CUIT { get; set; }
        public string? LegalAddress { get; set; }
        public int? IdLocation { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? LogoClub { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public bool IsEnabled { get; set; } = false;
        public string? ClubUrl { get; set; }

        [ForeignKey("IdLocation")]
        public Location Location { get; set; }
        public int IdLeague { get; set; }

        [ForeignKey("IdLeague")]
        public League League { get; set; }
      

    }
}
