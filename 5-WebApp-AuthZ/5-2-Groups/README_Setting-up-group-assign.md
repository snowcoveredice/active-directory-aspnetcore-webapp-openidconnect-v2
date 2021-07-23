# Setting up GroupAssign
This project is forked from the official MS example for Group based authorization. It has been expanded with a group administration module.

## User roles
Make sure any user who will be adding or removing members from groups have sufficient rights on the directory. The role 'Directory writers' grants these rights for instance.

### Application
Make sure the Active Directory App has sufficient API Permissions. The required permissions are:
- Directory.AccessAsUser.All
- Directory.ReadWrite.All
- Group.ReadWrite.All
- Groupmember.ReadWrite.All
- User.ReadWrite.All