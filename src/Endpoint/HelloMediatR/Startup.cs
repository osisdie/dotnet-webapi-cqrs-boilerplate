using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CoreFX.Abstractions.App_Start;
using CoreFX.Abstractions.Consts;
using CoreFX.Abstractions.Extensions;
using CoreFX.Abstractions.Logging;
using CoreFX.Auth.Interfaces;
using CoreFX.Auth.Services;
using CoreFX.DataAccess.Mapper.Extensions;
using CoreFX.Hosting.Extensions;
using CoreFX.Hosting.Middlewares;
using CoreFX.Notification.Extensions;
using FluentValidation.AspNetCore;
using Hello.MediatR.Domain.Contract.AuthServices.Login;
using Hello.MediatR.Domain.Contract.AuthServices.RefreshToken;
using Hello.MediatR.Domain.Contract.NotifiyServices;
using Hello.MediatR.Domain.DataAccess.DbContexts;
using Hello.MediatR.Domain.SDK.Services.AuthServices.Login;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Hello.MediatR.Endpoint
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            SdkRuntime.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Foundation
            services.AddLogging();
            services.AddOptions();
            services.AddHttpClient();
            services.AddHttpContextAccessor();

            string secret = Configuration.GetValue<string>("AuthConfig:JwtConfig:Secret");
            string issuer = Configuration.GetValue<string>("AuthConfig:JwtConfig:Issuer");
            string audience = Configuration.GetValue<string>("AuthConfig:JwtConfig:Audience");
            if (!string.IsNullOrEmpty(secret))
            {
                var secretKey = Encoding.ASCII.GetBytes(secret);
                services.AddAuthentication(auth =>
                {
                    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(token =>
                {
                    token.RequireHttpsMetadata = false;
                    token.SaveToken = true;
                    token.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        RequireExpirationTime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidateLifetime = true,
                        //LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken,
                        //                 TokenValidationParameters validationParameters) =>
                        //{
                        //    return notBefore <= DateTime.UtcNow && expires >= DateTime.UtcNow;
                        //}
                    };
                });
            }

            // CoreFX DI
            //services.AddMediatR(typeof(Program));
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly);
            });
            services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));
            services.AddScoped<ApiContext>();
            services.AddMappers<AutoMapper.IMapper>();
            //services.AddAutoMapper(cfg =>
            //{
            //    cfg.AllowNullCollections = true;
            //    cfg.AllowNullDestinationValues = true;
            //}, assemblies: new[]
            //{
            //    typeof(HelloLogin_Mapper).Assembly,
            //    typeof(Program).Assembly,
            //}.Distinct());

            services.AddSingleton<ISessionAdmin, SessionAdmin>();
            services.AddEmailService(options =>
            {
                Configuration.GetSection("NotifyConfig:EmailConfig")?.Bind(options);
            }, optional: true);
            services.AddReportService<LoginReportRecordDto>(options =>
            {
                Configuration.GetSection("NotifyConfig:ReportConfig")?.Bind(options);
            }, optional: true);

            // Domain DI
            //services.AddTransient<IAuthLogin_Service, AuthLogin_MockService>();
            services.AddTransient<IAuthLogin_Service, HelloLogin_Service>();

            // MVC
            services.AddHealthChecks();
            services.AddControllers()
            //.AddJsonOptions(options => {
            //    options.JsonSerializerOptions.IgnoreNullValues = true;
            //})
            .AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                opt.SerializerSettings.Converters.Add(new IsoDateTimeConverter());
            })
            .AddFluentValidation(opt =>
            {
                opt.RegisterValidatorsFromAssemblies(new[]
                {
                    typeof(HelloLogin_RequestValidator).Assembly,
                    typeof(AuthRefreshToken_RequestValidator).Assembly,
                }.Distinct());
            })
            .ConfigureApiBehaviorOptions(opt =>
            {
                opt.InvalidModelStateResponseFactory = c =>
                {
                    return c.ModelState.ToErrorJson400();
                };
            })
            .AddControllersAsServices();

            services.AddSwaggerGen(c =>
            {
                c.ResolveConflictingActions(descriptions =>
                {
                    return descriptions.First();
                });
                c.EnableAnnotations();
                c.SwaggerDoc("v202603", new OpenApiInfo { Title = "Hello-MediatR-NET v202603", Version = "202603" });
                c.SwaggerDoc("v202303", new OpenApiInfo { Title = "Hello-MediatR-NET v202303", Version = "202303" });
                c.SwaggerDoc("v202203", new OpenApiInfo { Title = "Hello-MediatR-NET v202203", Version = "202203" });
                c.SwaggerDoc("v202103", new OpenApiInfo { Title = "Hello-MediatR-NET v202103", Version = "202103" });

                c.DocumentFilter<RemoveDefaultApiVersionRouteDocumentFilter>();

                var xmlPath = Path.Combine(AppContext.BaseDirectory, "Swagger.xml");
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            SdkRuntime.SdkEnv = env.EnvironmentName;
            SdkRuntime.HostingEnv = env;
            LogMgr.LoggerFactory = loggerFactory;

            var envLog4netPath = Path.Combine(SvcConst.DefaultConfigFolder, SvcConst.DefaultLog4netConfigFile.AddingBeforeExtension(env.EnvironmentName));
            var defaultLog4netPath = Path.Combine(SvcConst.DefaultConfigFolder, SvcConst.DefaultLog4netConfigFile);
            if (File.Exists(envLog4netPath))
            {
                loggerFactory.AddLog4Net(envLog4netPath);
            }
            else if (File.Exists(defaultLog4netPath))
            {
                loggerFactory.AddLog4Net(defaultLog4netPath);
            }

            if (env.IsDevelopment() ||
                env.IsEnvironment(EnvConst.Testing) ||
                env.IsEnvironment(EnvConst.Debug))
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v202603/swagger.json", "Hello-MediatR-NET v202603");
                    c.SwaggerEndpoint("/swagger/v202303/swagger.json", "Hello-MediatR-NET v202303");
                    c.SwaggerEndpoint("/swagger/v202203/swagger.json", "Hello-MediatR-NET v202203");
                    c.SwaggerEndpoint("/swagger/v202103/swagger.json", "Hello-MediatR-NET v202103");
                });
            }

            app.UseRouting();

            app.UseJwtAuthorization();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRequestResponseLogging();
            app.UseExceptionHandlerMiddleware();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });
        }
    }
}
