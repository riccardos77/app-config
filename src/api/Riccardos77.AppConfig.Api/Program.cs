using Riccardos77.AppConfig;
using Riccardos77.AppConfig.Api.Configuration;
using Riccardos77.AppConfig.Api.Infrastructure;
using Riccardos77.AppConfig.DataProviders;
using Riccardos77.AppConfig.DataProviders.AzureBlobStorage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.AddScheme<ApiKeyAuthenticationHandler>(ApiKeyAuthenticationHandler.SchemeName, null);
    options.DefaultScheme = ApiKeyAuthenticationHandler.SchemeName;
});

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.AddValueManagers();

builder.Services.ConfigureAppDefinitions(
    builder.Configuration,
    providerFactoryBuilder =>
    {
        providerFactoryBuilder
            .RegisterAzureBlobStorageDataProvider()
            .RegisterDataProvider<FileSystemDataProvider>("FileSystem");
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
