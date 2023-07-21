using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIAFA.Models
{
    public class Bulletin
    {
        [Key]
        public int IdBulletin { get; set; }
        public string? Expedient { get; set; }
        public string? Name { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public string FileName { get; set; }
        public int IdBulletinType { get; set; }
        [ForeignKey("IdBulletinType")]
        public BulletinType BulletinType { get; set; }
        public bool HasSubscribers { get; set; }
    }
}
