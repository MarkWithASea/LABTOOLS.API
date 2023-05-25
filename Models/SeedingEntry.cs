using System.ComponentModel.DataAnnotations.Schema;

namespace LABTOOLS.API.Models
{
    [Table("SeedingHistory")]
    public class SeedingEntry
    {
        public int Id { get; set; }

        public string? Name { get; set; }
    }
}