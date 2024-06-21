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
using Microsoft.Extensions.Caching.Memory;

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
            // Dodaj HttpClientFactory za HTTP zahtjeve
            services.AddHttpClient();

            // Dodaj kontrolere
            services.AddControllers();

            // Dodaj Swagger/OpenAPI servise (opciono, za dokumentaciju API-ja)
            services.AddSwaggerGen();

            // Dodaj autentikaciju sa JWT Bearer tokenom
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
                        ValidateIssuer = false, // Promijenite po potrebi
                        ValidateAudience = false, // Promijenite po potrebi
                        ClockSkew = TimeSpan.Zero // Pomaže kod odstupanja sata prilikom usporedbe isteka tokena
                    };
                });

            // Dodaj autorizacijske politike ako su potrebne
            services.AddAuthorization();

            // Konfiguriraj CORS ako je potrebno
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins("https://example.com") // Ažurirajte sa svojom URL adresom frontend aplikacije
                        .AllowCredentials());
            });

            // Dodaj keširanje u memoriju
            services.AddMemoryCache();
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
