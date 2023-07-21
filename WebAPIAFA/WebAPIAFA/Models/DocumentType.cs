using System.ComponentModel.DataAnnotations;

namespace WebAPIAFA.Models
{
    public class DocumentType
    {
        [Key]
        public int IdDocumentType { get; set; }
        public string Name { get; set; }
    }
}
