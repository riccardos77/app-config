using Riccardos77.AppConfig.Api.Infrastructure;
using Riccardos77.AppConfig.Api.Providers;
using Riccardos77.AppConfig.DataAccessProviders.AzureBlobStorage;

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

builder.Services.ConfigureAppDefinitions(
    builder.Configuration,
    providerFactoryBuilder =>
    {
        providerFactoryBuilder
            .RegisterAzureBlobStorage()
            .RegisterDataAccessProvider<FileSystemDataAccessProvider>("FileSystem");
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
