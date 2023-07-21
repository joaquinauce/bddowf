using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIAFA.Models
{
    public class ClubAuthority
    {
        [Key]
        public int IdClubAuthority { get; set; }
        public int IdClubMandate { get; set; }
        public string FullName { get; set; }
        public int IdUserType { get; set; }

        [ForeignKey("IdUserType")]
        public UserType UserType { get; set; }

        [ForeignKey("IdClubMandate")]
        public ClubMandate ClubMandate { get; set; }
    }
}
