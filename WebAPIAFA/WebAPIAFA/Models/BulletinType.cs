using System.ComponentModel.DataAnnotations;

namespace WebAPIAFA.Models
{
    public class BulletinType
    {
        [Key]
        public int IdBulletinType { get; set; }
        public string Name { get; set; }
    }
}
