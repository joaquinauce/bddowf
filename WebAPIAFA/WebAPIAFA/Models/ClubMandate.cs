using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIAFA.Models
{
    public class ClubMandate
    {
        [Key]
        public int IdClubMandate { get; set; }
        public int IdClub { get; set; }
        public DateTime? StartMandate { get; set; }
        public DateTime? MandateExpiration { get; set; }
        public string? SignatoryClause { get; set; }

        [ForeignKey("IdClub")]
        public Club Club { get; set; }
    }
}
