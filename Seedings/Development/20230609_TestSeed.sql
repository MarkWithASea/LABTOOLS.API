--Create Roles
INSERT INTO "Roles" ("Id", "Name", "CognitoGroupName", "Description")
VALUES (1, 'Administrator', 'LABTOOLS:Role:Admin', 'Administrator Group'),
       (2, 'User', 'LABTOOLS:Role:User', 'User Group');

INSERT INTO "Permission" ("Id", "Name")
VALUES (1, 'View'), 
       (2, 'Edit');

-- Create Test Users
INSERT INTO "Users" ("Id", "CognitoId", "Email", "FirstName", "LastName", "IsDisabled", "IsDeleted")
VALUES  (1, 'eede85ba-5241-4d69-9f50-a298278e4aff','marc.reamer@cgm.com','Marc','Reamer', false, false),
        (2, 'ebd13003-95d3-4946-a99c-20b601749f6f', 'Test.Marc@cgmuslabdevelopment.com', 'Test', 'Marc1' false, false),
        (3, 'fef07aae-2c14-481b-91ff-70e398f1e11e', 'Test.Marc2@cgmuslabdevelopment.com', 'Test', 'Marc2', false, false);

-- Add Permissions to Roles
INSERT INTO "PermissionRole" ("PermissionsId", "RolesId")
VALUES (1, 1),
       (2, 1),
       (2, 1);

-- Add Test Users to Roles
INSERT INTO "RoleUser" ("RolesId", "UsersId")
VALUES (1, 1),
       (1, 2),
       (2, 3);
