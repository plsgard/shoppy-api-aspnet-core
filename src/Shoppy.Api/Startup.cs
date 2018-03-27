using System;
using System.IO;
using System.Linq;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Shoppy.Api.Authentication;
using Shoppy.Api.Configurations;
using Shoppy.Api.Models.Configuration;
using Shoppy.Application.Items;
using Shoppy.Application.Lists;
using Shoppy.Application.Session;
using Shoppy.Application.Users;
using Shoppy.Core.Data;
using Shoppy.Core.Roles;
using Shoppy.Core.Session;
using Shoppy.Core.Users;
using Shoppy.Data;
using Shoppy.Data.Repositories;
using Swashbuckle.AspNetCore.Swagger;

namespace Shoppy.Api
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
            ConfigureDatabase(services);
            ConfigureSecurity(services);
            ConfigureInjection(services);
            ConfigurationApi(services);

            services.AddMvc();

            services.AddAutoMapper();

            services.AddSwaggerGen(options =>
            {
                var provider = services.BuildServiceProvider()
                    .GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, new Info
                    {
                        Title = $"Shoppy API {description.GroupName}",
                        Version = description.ApiVersion.ToString(),
                        Description = "A simple API for Shoppy App." + (description.IsDeprecated ? " /WARNING/ This API version has been deprecated." : string.Empty),
                        Contact = new Contact
                        {
                            Email = "hello@plsgd.com",
                            Name = "Pier-Lionel Sgard",
                            Url = "http://plsgd.com"
                        }
                    });
                }

                options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                options.OperationFilter<SecurityRequirementsOperationFilter>();
                options.DocumentFilter<LowercaseDocumentFilter>();

                options.DescribeAllEnumsAsStrings();

                // Set the comments path for the Swagger JSON and UI.
                var basePath = AppContext.BaseDirectory;
                foreach (var file in Directory.GetFiles(basePath, "*.xml"))
                    options.IncludeXmlComments(file);
            });
        }

        private void ConfigurationApi(IServiceCollection services)
        {
            services.AddMvcCore().AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
            });
            services.AddCors();
        }

        private void ConfigureInjection(IServiceCollection services)
        {
            services.AddSingleton<IJwtFactory, JwtFactory>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddTransient<IItemAppService, ItemAppService>();
            services.AddTransient<IListAppService, ListAppService>();
            services.AddTransient<IUserAppService, UserAppService>();
            services.AddTransient<IAppSession, AppSession>();
        }

        private void ConfigureSecurity(IServiceCollection services)
        {
            // jwt wire up
            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection("Auth:Token").Get<TokenAuthenticationOptions>();

            // Configure TokenAuthenticationOptions
            SecurityKey signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAppSettingOptions.Key));
            services.Configure<TokenAuthenticationOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions.Issuer;
                options.Audience = jwtAppSettingOptions.Audience;
                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(jwtBearerOptions =>
                {
                    jwtBearerOptions.ClaimsIssuer = jwtAppSettingOptions.Issuer;
                    jwtBearerOptions.TokenValidationParameters = tokenValidationParameters;
                    jwtBearerOptions.SaveToken = true;
                });

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy(AppConsts.Policies.Users.Manager, policy =>
            //    {
            //        policy.RequireClaim(AppConsts.Policies.Claims.UsersRights, AppConsts.Policies.ManageActions.All.ToString());
            //    });
            //    options.AddPolicy(AppConsts.Policies.Accounts.Register, policy => policy.RequireClaim(AppConsts.Policies.Claims.AccountsRights, AppConsts.Policies.ManageActions.All.ToString(), AppConsts.Policies.ManageActions.Create.ToString()));
            //});
        }

        private void ConfigureDatabase(IServiceCollection services)
        {
            services.AddDbContext<ShoppyContext>(options =>
            {
                var connectionString = Configuration["ConnectionStrings:ShoppyContext"];
                if (!string.IsNullOrWhiteSpace(connectionString))
                    options.UseSqlServer(connectionString);
                else
                    options.UseInMemoryDatabase("shoppy");
            });

            services.AddIdentity<User, Role>(options =>
                {
                    options.User = new UserOptions
                    {
                        RequireUniqueEmail = true
                    };
                    options.Password = new PasswordOptions
                    {
                        RequiredLength = User.MinPasswordLength,
                        RequireNonAlphanumeric = false,
                        RequireUppercase = false,
                        RequireDigit = false
                    };
                    options.Lockout = new LockoutOptions
                    {
                        MaxFailedAccessAttempts = 5,
                        AllowedForNewUsers = true,
                        DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15)
                    };
                    options.SignIn = new SignInOptions
                    {
                        RequireConfirmedEmail = false // true : see later
                    };
                })
                .AddEntityFrameworkStores<ShoppyContext>()
                .AddDefaultTokenProviders();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName);
                }
            });

            app.UseCors(builder =>
                builder.WithOrigins(
                    // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                    Configuration["App:CorsOrigins"]
                        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(o => o.TrimEnd('/'))
                        .ToArray()
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
            );

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
