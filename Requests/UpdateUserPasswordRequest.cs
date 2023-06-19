namespace LABTOOLS.API.Requests
{
    public class UpdateUserPasswordRequest
    {
        public UpdateUserPasswordRequest(UpdateUserPasswordRequestData data)
        {
            Data = data;
        }

        public UpdateUserPasswordRequestData Data { get; set; }

        public class UpdateUserPasswordRequestData
        {
            public UpdateUserPasswordRequestData(string oldPassword, string newPassword)
            {
                OldPassword = oldPassword;
                NewPassword = newPassword;
            }

            public string OldPassword { get; set; }

            public string NewPassword { get; set; }
        }
    }
}