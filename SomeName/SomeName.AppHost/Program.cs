var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.AspnetCoreApp>("aspnetcoreapp");
builder.AddProject<Projects.AspNetMvcApp>("aspnetmvcapp");

builder.Build().Run();
