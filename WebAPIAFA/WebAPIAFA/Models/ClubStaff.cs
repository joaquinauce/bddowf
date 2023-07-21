using System.ComponentModel.DataAnnotations;

namespace WebAPIAFA.Models
{
    public class ClubStaff
    {
        [Key]
        public int IdClubStaff { get; set; }
        public string Name { get; set; }
    }
}
