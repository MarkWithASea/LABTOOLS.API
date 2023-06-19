using System.ComponentModel.DataAnnotations;

namespace LABTOOLS.API.Requests.Admin
{
    public class RegisterUserRequest
    {
        public RegisterUserRequest(RegisterUserData data)
        {
            Data = data;
        }

        [Required]
        public RegisterUserData Data { get; set; }

        public class RegisterUserData
        {
            public RegisterUserData(string firstName, string lastName, string email) //int roleId)
            {
                FirstName = firstName;
                LastName = lastName;
                Email = email;
            }

            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public int RoleId { get; set; }
        }
    }
}