namespace WebAPIAFA.Models.Dtos.PassDtos
{
    public class PassPerformActionDto
    {      
        public bool? Accepted { get; set; }
        public List<PassSignDto> PassSign { get; set; }
        public List<IFormFile>? Files { get; set; }
    }
}
