namespace LABTOOLS.API.JsonApi
{
    public class DocumentBase
    {
        public object? Meta { get; set; }

        public List<ErrorObject>? Errors { get; set; }

        public TopLevelLinks? Links { get; set; }
    }
}