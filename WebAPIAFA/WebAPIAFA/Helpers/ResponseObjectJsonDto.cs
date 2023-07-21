namespace WebAPIAFA.Helpers
{
    public class ResponseObjectJsonDto
    {
        public object? Response { get; set; }
        public int Code { get; set; } = default!;
        public string? Message { get; set; }
        public bool Status { get; set; } = default!;
    }
}
