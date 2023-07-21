using WebAPIAFA.Models.Dtos.DocumentDtos;
using WebAPIAFA.Models.Dtos.UserDtos;

namespace WebAPIAFA.Models.Dtos.FolderDtos
{
    public class FolderGetDto
    {
        public int IdFolder { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string UserCuil { get; set; }
        public DateTime CreationDate { get; set; }
        public List<UserGetDto> Users { get; set; }
        public List<DocumentGetDto> Documents { get; set; }
    }
}
