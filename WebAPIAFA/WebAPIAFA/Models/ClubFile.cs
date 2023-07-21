using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIAFA.Models
{
    public class ClubFile
    {
        [Key]
        public int IdClubFile { get; set; }
        public int IdClub { get; set; }
        public int IdFileType { get; set; }
        public string FileName { get; set; }

        [ForeignKey("IdFileType")]
        public ClubFileType ClubFileType { get; set; }
    }
}
