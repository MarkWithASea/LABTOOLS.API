using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LABTOOLS.API.Models
{
    public class User: IEntity
    {
        public User(string email, string cognitoId, string firstName, string lastName)
        {
            Email = email;
            CognitoId = cognitoId;
            FirstName = firstName;
            LastName = lastName;
        }

        public int Id { get; set; }

        // AWS Cognito User Id
        public string CognitoId { get; set; }

        [Required]
        public string Email { get; set; }

        public string? FirstName {get; set;}

        public string? LastName { get; set; }

        //public List<Role> Roles {get; set;}

        //public Role Role{get; set;}

        public bool IsDisabled { get; set; }

        public bool IsDeleted { get; set; }
    }
}