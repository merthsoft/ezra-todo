var builder = DistributedApplication.CreateBuilder(args);

var server = builder.AddProject<Projects.ToDoApp_Server>("server")
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();

// Certificate configuration types and members are for evaluation purposes only and are subject to change or removal in future updates.
#pragma warning disable ASPIRECERTIFICATES001
var webfrontend = builder.AddViteApp("webfrontend", "../frontend")
    .WithHttpsDeveloperCertificate()
    .WithDeveloperCertificateTrust(true)
    .WithEnvironment("NODE_OPTIONS", "--use-system-ca")
    .WithReference(server)
    .WaitFor(server);
#pragma warning restore ASPIRECERTIFICATES001

server.PublishWithContainerFiles(webfrontend, "wwwroot");

builder.Build().Run();
