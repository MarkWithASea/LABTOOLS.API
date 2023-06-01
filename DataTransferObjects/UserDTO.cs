namespace LABTOOLS.API.DataTransferObjects
{
    public class UserDTO : IDataTransferOjbect
    {        
        public int Id { get; set; }

        public string CognitoId { get; set; }
        
        public string Email { get; set; }

        public string? FirstName {get; set;}

        public string? LastName { get; set; }

        //public List<Role> Roles {get; set;}

        //public Role Role{get; set;}

        public bool IsDisabled { get; set; }

        public bool IsDeleted { get; set; }
    }
}