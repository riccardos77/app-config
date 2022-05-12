using Riccardos77.AppConfig.Api.Providers;
using Riccardos77.AppConfig.DataAccessProviders.AzureBlobStorage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddDataAccessProviderFactory(builder.Configuration, builder =>
{
    builder
        .RegisterAzureBlobStorage()
        .RegisterProvider<LocalFileSystemDataAccessProvider>("DataAccess:LocalFileSystem");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthorization();

app.MapControllers();

app.Run();
