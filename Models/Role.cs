using Microsoft.EntityFrameworkCore;

namespace LABTOOLS.API.Models
{
    [Index(nameof(Name), IsUnique = true)]
    [Index(nameof(CognitoGroupName), IsUnique = true)]
    public class Role : IEntity
    {
        public Role (string name, string cognitoGroupName)
        {
            Name = name;
            CognitoGroupName = cognitoGroupName;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string CognitoGroupName { get; set; }
        public string? Description { get; set; }
        public List<User>? Users { get; set; }
        public List<Permission>? Permissions  { get; set; }
    }
}