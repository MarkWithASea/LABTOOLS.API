using System.ComponentModel;

namespace LABTOOLS.API.Requests
{
    public class IndexQueryParameters
    {
        [DefaultValue(null)]
        public Dictionary<string, int>? Page { get; set; }

        [DefaultValue(null)]
        public Dictionary<string, string>? Filter { get; set; }

        [DefaultValue(null)]
        public Dictionary<string, string>? Search { get; set; }

        [DefaultValue(null)]
        public Dictionary<string, string>? OrderBy { get; set; }
    }
}