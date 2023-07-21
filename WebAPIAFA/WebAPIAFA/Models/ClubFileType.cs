using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPIAFA.Models
{
    public class ClubFileType
    {
        [Key]
        public int IdClubFileType { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
