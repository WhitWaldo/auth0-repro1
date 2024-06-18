var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.DaprApiPoc>("daprapipoc")
    .WithDaprSidecar("blazor-server");

builder.AddProject<Projects.FeatureApi>("featureapi")
    .WithDaprSidecar("api-feature");

builder.Build().Run();
