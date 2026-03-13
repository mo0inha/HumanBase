using Infrastructure.Shared.Context;
using Service.Api.Shared.DependencyInjection;
using Services.Api.Shared.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BaseContext>();
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsEnvironment("Development") || app.Environment.IsEnvironment("Local"))
{
    app.UseSwagger();
    app.UseStaticFiles();
    app.UseSwaggerUI(c =>
    {
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });
}

app.UseCors(x => x.AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin()
);

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
