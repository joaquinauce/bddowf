using System.ComponentModel.DataAnnotations;

namespace WebAPIAFA.Models
{
    public class ActionType
    {
        [Key]
        public int IdActionType { get; set; }
        public string Name { get; set; }
    }
}
