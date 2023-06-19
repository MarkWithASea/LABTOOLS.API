using Microsoft.EntityFrameworkCore;

namespace LABTOOLS.API.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Permission : IEntity
    {
        public Permission(string name)
        {
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public List<Role>? Roles { get; set; }
    }
}