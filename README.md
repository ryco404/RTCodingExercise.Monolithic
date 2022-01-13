# RTCodingExercise Monolithic Application Template

If you have found your way here, you have more than likely been asked by Regtransfers Ltd to complete a Coding Exercise as a part of your interview process. 

It’s great that you are eager to join our team. To make your life a bit easier we have provided you with a basic boilerplate project. 

This is the Monolithic style application, you can find a Microservices based boilerplate here

This project is:

ASP.NET Core 6.0.1 Web App (Model-View-Controller) with a seeded database.

- ApplicationDbContext inside the data folder is your Entity Framework Code First DatabaseContext.
- CodeFirst Migrations are enabled, any changes you make to the database can be implemented with 
  `Add-Migration` or `dotnet ef migrations add`
- ApplicationDbContextFactory enables these migrations to work
- ApplicationDbContextSeed seeds the DbContext with sample data, if you wish you can update this with more
- WebHostExtention in the extensions folder enables code first migrations to be run when the project starts
- Models contains the Plate model for the DbContext

For ease of use, Startup.cs has been created and initiated in the Program.cs.

An xUnit test project has also been added, and the MVC project has been referenced.

Good Luck.
