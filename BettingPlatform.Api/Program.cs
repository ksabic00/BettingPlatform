using BettingPlatform.Api.Middleware;
using BettingPlatform.Application;
using BettingPlatform.Infrastructure;
using BettingPlatform.Infrastructure.Persistence;
using BettingPlatform.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BettingPlatform.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((ctx, lc) =>
                lc.ReadFrom.Configuration(ctx.Configuration));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddTransient<ExceptionHandlingMiddleware>();

            builder.Services.AddCors(opt =>
             {
                 opt.AddPolicy("ng", p => p
                     .WithOrigins("http://localhost:4200")
                     .AllowAnyHeader()
                     .AllowAnyMethod());
             });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var logger = scope.ServiceProvider
                                  .GetRequiredService<ILoggerFactory>()
                                  .CreateLogger("InitialDataSeeder");

                await db.Database.MigrateAsync();
                await InitialDataSeeder.SeedAsync(db, logger);
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSerilogRequestLogging();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseCors("ng"); 

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
