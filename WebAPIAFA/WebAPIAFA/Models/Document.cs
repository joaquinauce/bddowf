using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIAFA.Models
{
    public class Document
    {
        [Key]
        public int IdDocument { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public string FileName { get; set; }
        public string CurrentFile { get; set; }
        public string OriginalFile { get; set; }

        public int? IdFolder { get; set; }
        [ForeignKey("IdFolder")]
        public Folder Folder { get; set; }
        public int? IdPass { get; set; }
        [ForeignKey("IdPass")]
        public Pass Pass { get; set; }
    }
}
