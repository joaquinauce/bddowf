using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPIAFA.Models
{
    [Table("ErrorLog")]
    public partial class ErrorLog
    {
        [Key]
        public int ID { get; set; }
        [Unicode(false)]
        public string? Description { get; set; }
        [Unicode(false)]
        public string? Module { get; set; }
        [Unicode(false)]
        public string? ErrorCode { get; set; }
        [Unicode(false)]
        public string? Project { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
    }
}
