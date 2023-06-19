--Create Role
INSERT INTO "Roles" ("Id", "Name", "CognitoGroupName", "Description")
VALUES (1, 'Administrator', 'LABTOOLS:Role:Admin', 'Administrator Group');

INSERT INTO "Permission" ("Id", "Name")
VALUES (1, 'View'), 
       (2, 'Edit');

-- Create Test User
INSERT INTO "Users" ("Id", "CognitoId", "Email", "FirstName", "LastName", "IsDisabled", "IsDeleted")
VALUES  (1, 'eede85ba-5241-4d69-9f50-a298278e4aff','marc.reamer@cgm.com','Marc','Reamer', false, false);

-- Add Permissions to Roles
INSERT INTO "PermissionRole" ("PermissionsId", "RolesId")
VALUES (1, 1),
       (2, 1);

-- Add Test User to Admin Role
INSERT INTO "RoleUser" ("RolesId", "UsersId")
VALUES (1, 1);
