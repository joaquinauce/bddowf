using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIAFA.Models
{
    public class ClubInformation
    {
        [Key]
        public int IdClubInformation { get; set; }
        public int IdClub { get; set; }
        public string? StadiumImage { get; set; }
        public string? Dimensions { get; set; }
        public string? Capacity { get; set; }
        public DateTime? FoundationDate { get; set; }
        public DateTime? OpeningDate { get; set; }
        public string? History { get; set; }
        public string? LegalAddress { get; set; }

        [ForeignKey("IdClub")]
        public Club Club { get; set; }

    }
}
