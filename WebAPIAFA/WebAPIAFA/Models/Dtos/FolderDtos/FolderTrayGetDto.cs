using WebAPIAFA.Models.Dtos.DocumentDtos;
using WebAPIAFA.Models.Dtos.UserDtos;

namespace WebAPIAFA.Models.Dtos.FolderDtos
{
    public class FolderTrayGetDto
    {   
        public int IdFolder { get; set; }
        public string PlayerCUIL { get; set; }
        public string PlayerName { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public string LogoClub { get; set; }
        public List<UserGetDto> Users { get; set; }
        public List<DocumentGetDto> Documents { get; set; }
    }
}
