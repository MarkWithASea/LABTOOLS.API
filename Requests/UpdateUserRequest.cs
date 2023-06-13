using System.ComponentModel.DataAnnotations;

namespace LABTOOLS.API.Requests
{
    public class UpdateUserRequest
    {
        public UpdateUserRequest(UpdateUserData data)
        {
            Data = data;
        }

        [Required]
        public UpdateUserData Data { get; set; }

        public class UpdateUserData
        {
            public UpdateUserData(string firstName, string lastName)
            {
                FirstName = firstName;
                LastName = lastName;
            }

            public string FirstName { get; set; }

            public string LastName { get; set; }
        }
    }
}