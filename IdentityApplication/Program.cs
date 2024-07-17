using IdentityApplication.API.Data;
using IdentityApplication.API.Models;
using IdentityApplication.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityApplication
{
    public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(C =>
            {
                var SecuritySchema = new OpenApiSecurityScheme
                {
                    Name = "Authorizations",
                    Description = " Jwt Auth Bearer Schema",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme,

                    }
                };

                C.AddSecurityDefinition("Bearer", SecuritySchema);
                var ScurityRequirments = new OpenApiSecurityRequirement
                {
                    {
                        SecuritySchema , new [] {"Bearer"}
                    }
                };

                C.AddSecurityRequirement(ScurityRequirments);
            });



            builder.Services.AddDbContext<AppIdentityDbContext>(Opt =>
			{
				Opt.UseSqlServer(builder.Configuration.GetConnectionString("IdnetityDbContext"));
			});


            builder.Services.AddScoped<JwtServices>();
			builder.Services.AddScoped<EmailServices>();
			builder.Services.AddScoped<ContextSeedingData>();

			builder.Services.AddIdentityCore<AppUser>(opt =>
			{
				opt.Password.RequiredLength = 6;
				opt.Password.RequireNonAlphanumeric = false;
				opt.Password.RequireLowercase = false;
				opt.Password.RequireUppercase = false;
				opt.Password.RequireDigit = false;


				opt.SignIn.RequireConfirmedAccount = true;
			})
				.AddRoles<IdentityRole>()
				.AddRoleManager<RoleManager<IdentityRole>>()
				.AddEntityFrameworkStores<AppIdentityDbContext>()
				.AddSignInManager<SignInManager<AppUser>>()
				.AddUserManager<UserManager<AppUser>>()
				.AddDefaultTokenProviders();


			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(opt =>
				{
					opt.TokenValidationParameters = new TokenValidationParameters
					{

						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:key"])),
						ValidateIssuer = true,
						ValidIssuer = builder.Configuration["Token:ValidIssuer"],
						ValidateAudience = true,
						ValidAudience = builder.Configuration["Token:ValidAudiance"],
						ValidateLifetime = true,
					};
				});

			builder.Services.AddCors( opt =>
			{
				opt.AddPolicy("CotsOrigin", policy =>
				{
					policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
				});
			});

			builder.Services.Configure<ApiBehaviorOptions>(opt =>
			{
				opt.InvalidModelStateResponseFactory = actionContent =>
				{
					var errors = actionContent.ModelState.Where(e => e.Value.Errors.Count > 0)
														.SelectMany(e => e.Value.Errors)
														.Select(e => e.ErrorMessage).ToArray();


					var objectToReturn = new { Errors = errors };


					return new BadRequestObjectResult(objectToReturn);
				};
			});






			var app = builder.Build();
			using var scoped = app.Services.CreateScope();
			var servicesProvader = scoped.ServiceProvider;
			var LoogerFactory = servicesProvader.GetRequiredService<ILoggerFactory>();



			try
			{
				var dbcontext = servicesProvader.GetRequiredService<AppIdentityDbContext>();
				await dbcontext.Database.MigrateAsync();

				var initSeedContext = servicesProvader.GetRequiredService<ContextSeedingData>();
				await initSeedContext.InitializeContextAsync();


            }
			catch (Exception ex)
			{
				var loger = LoogerFactory.CreateLogger<Program>();
				loger.LogError(ex, "An Error Occure in migration or seeding data");

			}
			// Configure the HTTP request pipeline.
			app.UseCors("CotsOrigin");
			
				
	
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
