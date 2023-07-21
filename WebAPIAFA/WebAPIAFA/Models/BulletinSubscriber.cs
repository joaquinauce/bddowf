using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIAFA.Models
{
    public class BulletinSubscriber
    {
        [Key]
        public int IdBulletinSubscriber { get; set; }
        public int IdBulletin { get; set; }
        public bool IsSigned { get; set; }
        public bool IsRead { get; set; }
        public int IdUser { get; set; }
        [ForeignKey("IdBulletin")]
        public Bulletin Bulletin { get; set; }
        [ForeignKey("IdUser")]
        public User User { get; set; }
    }
}
