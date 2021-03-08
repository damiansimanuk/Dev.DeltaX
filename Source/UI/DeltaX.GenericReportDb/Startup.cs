using DeltaX.Database;
using DeltaX.GenericReportDb.Configuration;
using DeltaX.GenericReportDb.Repository;
using DeltaX.GenericReportDb.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Data;
using System.Text; 

namespace DeltaX.GenericReportDb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var conString = Configuration.GetConnectionString("UserRepository");
            services.AddSingleton(new DbConnectionFactory<SqliteConnection>(new[] { conString })); 
            services.AddTransient<IDbConnection>(services =>
            {
                return services.GetService<DbConnectionFactory<SqliteConnection>>().GetConnection();
            });
            services.AddTransient<UserRepository>();
            services.AddTransient<IUserService, UserService>(); 
            services.AddSingleton<CrudConfiguration>();
            services.AddSingleton<CrudServicePool>();

            services.AddCors(options =>
            {
                options.AddPolicy("DefaultCors", builder =>
                {
                    builder
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials()
                      .WithOrigins("http://127.0.0.1:8080", "http://127.0.0.1:5050", "https://127.0.0.1:5051");
                });
            });

            services.AddAuthentication(); 
            // services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
            services.AddControllers()
              //   .AddJsonOptions(options => options.JsonSerializerOptions )
              ;

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DeltaX.ApiRestLongPollingTest", Version = "v1" }); 
            });

            var secret = this.Configuration.GetValue<string>("UserService:Secret");
            var key = Encoding.ASCII.GetBytes(secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            CreateTable(app, env);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); 
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DeltaX.ApiRestLongPollingTest v1"));
            }

            app.UseHttpsRedirection();            
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseRouting();

            // global cors policy
            app.UseCors("DefaultCors");

            // serve wwwroot
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void CreateTable(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Create user schema
            var connection = app.ApplicationServices.GetService<IDbConnection>();
            var userTableCrator = new UserTableCrator(connection);
            userTableCrator.Start();

            // Create Administrator User
            var userRepository = app.ApplicationServices.GetService<UserRepository>();
            userRepository.CreateAdministrator();
        }
    }
}
