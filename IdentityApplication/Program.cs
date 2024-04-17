using IdentityApplication.API.Data;
using IdentityApplication.API.Models;
using IdentityApplication.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
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
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddDbContext<AppIdentityDbContext>(Opt =>
			{
					Opt.UseSqlServer(builder.Configuration.GetConnectionString("IdnetityDbContext"));
			});

			builder.Services.AddScoped<JwtServices>();

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
						IssuerSigningKey = new SymmetricSecurityKey (Encoding.UTF8.GetBytes(builder.Configuration["Token:key"])),
						ValidateIssuer = true , 
						ValidIssuer = builder.Configuration["Token:ValidIssuer"],
						ValidateAudience = true,
						ValidAudience = builder.Configuration["Token:ValidAudiance"],
						ValidateLifetime =true,
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
			}
			catch (Exception ex)
			{
				var loger = LoogerFactory.CreateLogger<Program>();
				loger.LogError(ex, "An Error Occure");
			
			}
			// Configure the HTTP request pipeline.
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
