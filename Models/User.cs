using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LABTOOLS.API.Models
{
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(CognitoId), IsUnique = true)]
    public class User : IEntity
    {
        public User(string email, string cognitoId, string firstName, string lastName)
        {
            Email = email;
            CognitoId = cognitoId;
            FirstName = firstName;
            LastName = lastName;
            Roles = new List<Role>();
            IsDisabled = false;
            IsDeleted = false;
        }

        public int Id { get; set; }

        // AWS Cognito User Id
        public string CognitoId { get; set; }

        [Required]
        public string Email { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public List<Role> Roles { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsDeleted { get; set; }
    }
}