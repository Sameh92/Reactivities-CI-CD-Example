using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Activities;
using Persistence;
using FluentValidation;
using FluentValidation.AspNetCore;
using Application.Interfaces;
using Infrastructure.Security;
using Infrastructure.Photos;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            IConfiguration config)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddDbContext<DataContext>(opt =>
            {
                //opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
                //opt.UseNpgsql(config.GetConnectionString("DefaultConnection"));
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                string connStr;

                // Depending on if in development or production, use either FlyIO
                // connection string, or development connection string from env var.
                if (env == "Development")
                {
                    // Use connection string from file.
                    connStr = config.GetConnectionString("DefaultConnection");
                }
                else
                {
                    // Use connection string provided at runtime by Flyio.
                    var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

                    // Parse connection URL to connection string for Npgsql
                    connUrl = connUrl.Replace("postgres://", string.Empty);
                    var pgUserPass = connUrl.Split("@")[0];
                    var pgHostPortDb = connUrl.Split("@")[1];
                    var pgHostPort = pgHostPortDb.Split("/")[0];
                    var pgDb = pgHostPortDb.Split("/")[1];
                    var pgUser = pgUserPass.Split(":")[0];
                    var pgPass = pgUserPass.Split(":")[1];
                    var pgHost = pgHostPort.Split(":")[0];
                    var pgPort = pgHostPort.Split(":")[1];

                    connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};";
                }

                // Whether the connection string came from the local development configuration file
                // or from the environment variable from FlyIO, use it to set up your DbContext.
                opt.UseNpgsql(connStr);
            });
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    // policy.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:3000");
                    policy
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials()
                      .WithOrigins("http://localhost:3000");
                });
            });
            // register Mediator
            /*
             when our application starts app this  service gets registered and then it takes a look

            inside the assembly where this class is located and it's going to register all of our mediator handlers

            so it knows where to send the notifications or activities that we're getting mediator to take care of.
            */
            services.AddMediatR(typeof(List.Handler));


            // add autoMapper
            /*
             we're going to tell it to use the assembly to locate all of the mapping profiles that we're

            using inside our project.

            Same deal as mediator.

            When our application starts.

            Auto mapper is going to be registered as a service.

            It's going to take a look inside this assembly that contains mapping profiles and register all of our

            mapping profiles.

            So it can be used when it comes across them inside our code here.
            */
            services.AddAutoMapper(typeof(MappingProfiles).Assembly);
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<Create>();
            services.AddHttpContextAccessor();
            services.AddScoped<IUserAccessor, UserAccessor>();
            services.AddScoped<IPhotoAccessor, PhotoAccessor>();
            services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));
            services.AddSignalR();
            return services;

        }
    }
}