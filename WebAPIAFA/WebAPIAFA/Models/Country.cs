using System.ComponentModel.DataAnnotations;

namespace WebAPIAFA.Models
{
    public class Country
    {
        [Key]
        public int IdCountry { get; set; }
        public string Name { get; set; }
    }
}
