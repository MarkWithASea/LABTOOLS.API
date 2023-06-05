namespace LABTOOLS.API.JsonApi
{
    public class ErrorObject
    {
        public int Status { get; set; }

        public string? Source { get; set; }

        public string? Detail { get; set; }
    }
}