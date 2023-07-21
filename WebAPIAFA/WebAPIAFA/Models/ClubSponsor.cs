using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIAFA.Models
{
    public class ClubSponsor
    {
        [Key]
        public int IdClubSponsor { get; set; }
        public int IdClub { get; set; }
        public int IdSponsor { get; set; }
        public DateTime CreationDate { get; set; }

        [ForeignKey("IdClub")]
        public Club club { get; set; }

        [ForeignKey("IdSponsor")]
        public Sponsor sponsor { get; set; }
    }
}
