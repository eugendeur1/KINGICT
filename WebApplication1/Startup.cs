using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using WebApplication1.Services;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // ConfigureServices is used to add services to the DI container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add HttpClientFactory for making HTTP requests
            services.AddHttpClient();

            // Add controllers
            services.AddControllers();

            // Add Swagger/OpenAPI services (optional, for API documentation)
            services.AddSwaggerGen();

            // Add authentication with JWT Bearer token
            var key = Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false, // Modify as needed
                        ValidateAudience = false, // Modify as needed
                        ClockSkew = TimeSpan.Zero // This helps with clock skew when comparing token expiry
                    };
                });

            // Add authorization policies if needed
            services.AddAuthorization();

            // Configure CORS if needed
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins("https://example.com") // Update with your frontend URL
                        .AllowCredentials());
            });
        }

        // Configure is used to set up the middleware pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // Detailed error page for development
                app.UseSwagger(); // Swagger UI for API documentation
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }
            else
            {
                app.UseExceptionHandler("/error"); // Error handling for non-development environments
                app.UseHsts(); // HTTP Strict Transport Security
            }

            app.UseHttpsRedirection(); // Redirect HTTP to HTTPS

            app.UseRouting(); // Enable routing

            app.UseCors("CorsPolicy"); // Enable CORS

            app.UseAuthentication(); // Use authentication
            app.UseAuthorization(); // Use authorization

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // Map controllers
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!"); // Default endpoint response
                });
            });
        }
    }
}
