namespace WebAPIAFA.Models.Dtos.DocumentDtos
{
    public class DocumentAffectedUserHistoryDto
    {
        public int IdDocument { get; set; }
        public string DocumentName { get; set; }
        public string DocumentType { get; set; }//pase o contrato
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
