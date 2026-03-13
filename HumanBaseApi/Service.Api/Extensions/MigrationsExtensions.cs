using Infrastructure.Shared.Context;
using Microsoft.EntityFrameworkCore;

namespace Service.Api.Extensions
{
    public static class MigrationExtensions
    {
        public static void ApplyMigration(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            using BaseContext dbContext = scope.ServiceProvider.GetRequiredService<BaseContext>();

            dbContext.Database.Migrate();
        }
    }
}