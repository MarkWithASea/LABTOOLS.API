using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;

namespace LABTOOLS.API.Managers
{
    public class CognitoUserManager
    {
        private readonly AmazonCognitoIdentityProviderClient adminAmazonCognitoIdentityProviderClient;
        private readonly AmazonCognitoIdentityProviderClient anonymousAmazonCognitoIdentityProviderClient;

        public CognitoUserManager(RegionEndpoint regionEndpoint)
        {
            adminAmazonCognitoIdentityProviderClient = new AmazonCognitoIdentityProviderClient(regionEndpoint);
            anonymousAmazonCognitoIdentityProviderClient = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), regionEndpoint);
        }

        public async Task<AdminInitiateAuthResponse> AdminAuthenticateUserAsync(string username, string password, string userPoolId, string appClientId)
        {
            var adminInitiateAuthRequest = new AdminInitiateAuthRequest
            {
                UserPoolId = userPoolId,
                ClientId = appClientId,
                AuthFlow = "ADMIN_NO_SRP_AUTH",
                AuthParameters = new Dictionary<string, string>
            {
                { "USERNAME", username },
                { "PASSWORD", password },
            },
            };

            return await adminAmazonCognitoIdentityProviderClient
                .AdminInitiateAuthAsync(adminInitiateAuthRequest)
                .ConfigureAwait(false);
        }

        public async Task<AdminCreateUserResponse> AdminCreateUserAsync(string email, string userPoolId, string appClientId, List<AttributeType> attributeTypes)
        {
            var adminCreateUserRequest = new AdminCreateUserRequest
            {
                Username = email,
                UserPoolId = userPoolId,
                UserAttributes = attributeTypes,
                DesiredDeliveryMediums = new List<string>() { "EMAIL" },
            };

            var createUserResponse = await adminAmazonCognitoIdentityProviderClient
                .AdminCreateUserAsync(adminCreateUserRequest)
                .ConfigureAwait(false);

            AdminUpdateUserAttributesRequest adminUpdateUserAttributesRequest = new AdminUpdateUserAttributesRequest
            {
                Username = email,
                UserPoolId = userPoolId,
                UserAttributes = new List<AttributeType>
                        {
                            new AttributeType()
                            {
                                Name = "email_verified",
                                Value = "true",
                            },
                        },
            };

            var adminUpdateUserAttributesResponse = await adminAmazonCognitoIdentityProviderClient
                .AdminUpdateUserAttributesAsync(adminUpdateUserAttributesRequest);

            return createUserResponse;
        }

        public async Task<AdminCreateUserResponse> AdminResendPasswordAsync(string email, string userPoolId, string appClientId)
        {
            var adminCreateUserRequest = new AdminCreateUserRequest
            {
                Username = email,
                UserPoolId = userPoolId,
                MessageAction = "RESEND",
                DesiredDeliveryMediums = new List<string>() { "EMAIL" },
            };

            var createUserResponse = await adminAmazonCognitoIdentityProviderClient
                .AdminCreateUserAsync(adminCreateUserRequest)
                .ConfigureAwait(false);

            return createUserResponse;
        }

        public async Task<AdminGetUserResponse> AdminGetUserAsync(string username, string userPoolId)
        {
            var adminGetUserRequest = new AdminGetUserRequest
            {
                Username = username,
                UserPoolId = userPoolId,
            };

            return await adminAmazonCognitoIdentityProviderClient
                .AdminGetUserAsync(adminGetUserRequest)
                .ConfigureAwait(false);
        }

        public async Task<AdminUpdateUserAttributesResponse> AdminUpdateUserAttributes(string email, string userPoolId, string appClientId, List<AttributeType> attributeTypes)
        {
            AdminUpdateUserAttributesRequest adminUpdateUserAttributesRequest = new AdminUpdateUserAttributesRequest
            {
                Username = email,
                UserPoolId = userPoolId,
                UserAttributes = attributeTypes,
            };

            var adminUpdateUserAttributesResponse = await adminAmazonCognitoIdentityProviderClient
                .AdminUpdateUserAttributesAsync(adminUpdateUserAttributesRequest);

            return adminUpdateUserAttributesResponse;
        }

        public async Task<AdminListGroupsForUserResponse> AdminListGroupsForUserAsync(string username, string userPoolId)
        {
            var adminListGroupsForUserRequest = new AdminListGroupsForUserRequest
            {
                Username = username,
                UserPoolId = userPoolId,
            };

            return await adminAmazonCognitoIdentityProviderClient
                .AdminListGroupsForUserAsync(adminListGroupsForUserRequest)
                .ConfigureAwait(false);
        }

        public async Task AdminAddUserToGroupAsync(string username, string userPoolId, string groupName)
        {
            var adminAddUserToGroupRequest = new AdminAddUserToGroupRequest
            {
                Username = username,
                UserPoolId = userPoolId,
                GroupName = groupName,
            };

            AdminAddUserToGroupResponse adminAddUserToGroupResponse = await adminAmazonCognitoIdentityProviderClient
                .AdminAddUserToGroupAsync(adminAddUserToGroupRequest)
                .ConfigureAwait(false);
        }

        public async Task AdminRemoveUserFromGroupAsync(string username, string userPoolId, string groupName)
        {
            var adminRemoveUserFromGroupRequest = new AdminRemoveUserFromGroupRequest
            {
                Username = username,
                UserPoolId = userPoolId,
                GroupName = groupName,
            };

            await adminAmazonCognitoIdentityProviderClient
                .AdminRemoveUserFromGroupAsync(adminRemoveUserFromGroupRequest)
                .ConfigureAwait(false);
        }

        public async Task AdminDisableUserAsync(string username, string userPoolId)
        {
            var adminDisableUserRequest = new AdminDisableUserRequest
            {
                Username = username,
                UserPoolId = userPoolId,
            };

            await adminAmazonCognitoIdentityProviderClient
                .AdminDisableUserAsync(adminDisableUserRequest)
                .ConfigureAwait(false);
        }

        public async Task AdminEnableUserAsync(string username, string userPoolId)
        {
            var adminEnableUserRequest = new AdminEnableUserRequest
            {
                Username = username,
                UserPoolId = userPoolId,
            };

            await adminAmazonCognitoIdentityProviderClient
                .AdminEnableUserAsync(adminEnableUserRequest)
                .ConfigureAwait(false);
        }

        public async Task AdminDeleteUserAsync(string username, string userPoolId)
        {
            var deleteUserRequest = new AdminDeleteUserRequest
            {
                Username = username,
                UserPoolId = userPoolId,
            };

            await adminAmazonCognitoIdentityProviderClient
                .AdminDeleteUserAsync(deleteUserRequest)
                .ConfigureAwait(false);
        }

        public async Task<ChangePasswordResponse> ChangePasswordAsync(string accessToken, string previousPassword, string proposedPassword)
        {
            var changePasswordRequest = new ChangePasswordRequest
            {
                AccessToken = accessToken,
                PreviousPassword = previousPassword,
                ProposedPassword = proposedPassword,
            };

            ChangePasswordResponse response = await adminAmazonCognitoIdentityProviderClient
                .ChangePasswordAsync(changePasswordRequest);

            return response;
        }
    }
}