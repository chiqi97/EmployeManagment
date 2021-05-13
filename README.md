# EmployeeManagement
The first application based on C#, ASP.Net Core 5.0 and Entity Framework. In this application you can manage employees, 
find them and manage users on this page from the administration panel. If you want to know more please read "Readme_Emp".

EmployeeManagement
General Information:
The EmployeeManagement project was made for the purpose of learning
programming.
User:
 From the main menu you can create new employee, you have to click
create, refill all fields and click create.(you can add unique photo, name,
mail and city)
 From the main page you can also search employees by name or
department.
 The last option of normal user is the view specific information about the
employees.
Admin:
 Admin has all previos functionalities enabled.
 Admin in main page can Edit and Delete employees.
 From the main menu admin can manage users and roles.
 In the section manage users admin can :
o Add new users
o Edit users( update information about them, change role and change
user claims)
o Delete users
 In the role managment section admin can Edit, Delete and Add new only
when these claims are enabled.
Super Admin:
 Super Admin has all previous functionalities enabled.
 Super admin doesnt need enabled claims to manage roles, in manage rol
super admin can:
o Add new roles
o Edit users in specific role
o Delete roles
Technical aspects:
In the main folder of downloaded files is the app_data/EmployeeDB file
which is a database and contains employees and users. Before using the
application attach the file to your SQL server.
Login and passwords for every role:
Super Admin : superadmin@mail.com/qweASD123
Admin: admin@mail.com/qweASD123
User: user@mail.com/qweASD123
