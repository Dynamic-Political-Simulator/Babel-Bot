using BabelDatabase;
using Discord.WebSocket;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOPR.Services;

namespace WOPR
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
			services.AddControllers();

			var configBuilder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

			var configuration = configBuilder.Build();

			var section = configuration.GetSection("WoprConfig");

			services.Configure<WoprConfig>(section);

			// add context
			services.AddDbContext<BabelContext>();

			services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
			{
				builder.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader();
			}));

			var appSettings = section.Get<WoprConfig>();

			var key = Encoding.ASCII.GetBytes(appSettings.JwtSecret);

			// add services
			services.AddScoped<DiscordBotService>();
			services.AddScoped<DiscordUserAuthenticationTokenService>();
			services.AddScoped<DiscordUserService>();

			services.AddAuthentication(a =>
			{
				a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(j =>
			{
				j.Events = new JwtBearerEvents()
				{
					OnTokenValidated = context =>
					{
						var userService = context.HttpContext.RequestServices.GetRequiredService<DiscordUserService>();
						var userId = context.Principal.Identity.Name;

						var user = userService.GetUserById(userId);

						if (user == null)
						{
							context.Fail("Unauthorised.");
						}

						return Task.CompletedTask;
					}
				};

				j.RequireHttpsMetadata = false;
				j.SaveToken = true;
				j.TokenValidationParameters = new TokenValidationParameters()
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = false,
					ValidateAudience = false
				};
			});


		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseAuthentication();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
