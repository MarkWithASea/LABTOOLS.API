using LABTOOLS.API.Models;

namespace LABTOOLS.API.DataTransferObjects
{
    public class UserDTO : IDataTransferObject
    {        
        public int Id { get; set; }

        public string? CognitoId { get; set; }
        
        public string? Email { get; set; }

        public string? FirstName {get; set;}

        public string? LastName { get; set; }

        public Role Role{get; set;}

        public bool IsDisabled { get; set; }

        public bool IsDeleted { get; set; }

        public bool? IsConfirmed { get; set; } = null;
    }
}