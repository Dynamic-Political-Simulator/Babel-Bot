using BabelDatabase;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
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

            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.WithOrigins("https://discordplaysstellaris.com")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }));

            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // add context
            services.AddDbContext<BabelContext>();

            //services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            //{
            //	builder.AllowAnyOrigin()
            //		.AllowAnyMethod()
            //		.AllowAnyHeader()
            //		.SetIsOriginAllowed(origin => true)
            //		.WithOrigins("https://localhost:3000");
            //}));





            // add services
            services.AddScoped<DiscordUserService>();
            services.AddScoped<EconPopsimServices>();
            services.AddScoped<SimulationService>();

            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None; // Shoutout to chrome for working well :*
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration.GetValue<string>("Jwt:Issuer"),
                    ValidAudience = Configuration.GetValue<string>("Jwt:Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("Jwt:EncryptionKey")))
                };
            })
            .AddOAuth("Discord", options =>
            {
                options.AuthorizationEndpoint = "https://discord.com/api/oauth2/authorize";
                options.Scope.Add("identify");

                options.CallbackPath = new PathString("/auth/oauthCallback");

                options.ClientId = Configuration.GetValue<string>("Discord:ClientId");
                options.ClientSecret = Configuration.GetValue<string>("Discord:ClientSecret");

                options.TokenEndpoint = "https://discord.com/api/oauth2/token";

                options.UserInformationEndpoint = "https://discord.com/api/users/@me";

                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "username");

                options.AccessDeniedPath = "/api/DiscordAuthFailed";

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                        var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();

                        var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;

                        context.RunClaimActions(user);
                    }
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseCors(x => x
            //	.WithOrigins("http://localhost:3000")
            //	.AllowAnyMethod()
            //	.AllowAnyHeader()
            //	.AllowCredentials());

            app.UseCors("MyPolicy");

            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCookiePolicy();

            app.UseAuthorization();



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
