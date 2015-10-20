using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using BotaNaRoda.WebApi.Data;
using BotaNaRoda.WebApi.Identity;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using IdentityServer3.Core.Logging;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Cors.Core;
using Microsoft.AspNet.Hosting;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using Microsoft.Owin.Security.Facebook;
using Newtonsoft.Json.Linq;
using Owin;
using Serilog;
using IdentityServer3;
using Loggly;
using Loggly.Config;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Logging;
using Constants = IdentityServer3.Core.Constants;

namespace BotaNaRoda.WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            // Setup configuration sources.
            var builder = new ConfigurationBuilder(appEnv.ApplicationBasePath)
                .AddJsonFile("config.json")
                .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        // This method gets called by a runtime.
        // Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            // maps the AppSettings configuration key to an instance of the configuration class
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            //------ CONTAINER STUFF
            services.AddSingleton<ItemsContext>();
            services.AddSingleton<NotificationService>();
            //-------

            services.AddMvc();
            services.AddSignalR();
            services.AddDataProtection();
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IApplicationEnvironment env, IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory)
        {
            ConfigureLogging(app, appSettings, loggerFactory);

            app.UseStaticFiles();

            app.Map("/core", core =>
            {
                var userService = new Registration<IUserService>(resolver => new UserService(new ItemsContext(appSettings)));
                var fbCustomGrant = new Registration<ICustomGrantValidator>(resolver => new FacebookCustomGrantValidator(new ItemsContext(appSettings)));

                var idSvrMongoDbSettings = IdentityServer3.MongoDb.StoreSettings.DefaultSettings();
                idSvrMongoDbSettings.ConnectionString = appSettings.Options.BotaNaRodaConnectionString;
                idSvrMongoDbSettings.Database = appSettings.Options.BotaNaRodaDatabaseName;

                var idSvrMongoDbFactory = new IdentityServer3.MongoDb.ServiceFactory(userService, idSvrMongoDbSettings)
                {
                    CorsPolicyService = new Registration<ICorsPolicyService>(new DefaultCorsPolicyService {AllowAll = true})
                };
                idSvrMongoDbFactory.UseInMemoryClients(Clients.Get());
                idSvrMongoDbFactory.UseInMemoryScopes(Scopes.Get());
                idSvrMongoDbFactory.CustomGrantValidators.Add(fbCustomGrant);

                var idsrvOptions = new IdentityServerOptions
                {
                    IssuerUri = "https://botanaroda.com.br",
                    SiteName = "Bota na Roda",
                    EnableWelcomePage  = false,
                    RequireSsl = false,
                    SigningCertificate = new X509Certificate2(env.ApplicationBasePath + "\\Identity\\idsrv3test.pfx", "idsrv3test"),
                    Factory = idSvrMongoDbFactory,
                    
                    AuthenticationOptions = new AuthenticationOptions
                    {
                        IdentityProviders = ConfigureAdditionalIdentityProviders,
                        EnableLocalLogin = false
                    },

                    EventsOptions = new EventsOptions
                    {
                        RaiseSuccessEvents = true,
                        RaiseErrorEvents = true,
                        RaiseFailureEvents = true,
                        RaiseInformationEvents = true
                    }
                };

                core.UseIdentityServer(idsrvOptions);
            });

            app.Map("/api", theApi =>
            {
                //Really? yet??
                JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

                theApi.UseOAuthBearerAuthentication(options =>
                {
                    options.Authority = appSettings.Options.IdSvrAuthority;
                    options.Audience = appSettings.Options.IdSvrAudience;
                    options.AutomaticAuthentication = true;
                });

                theApi.UseMiddleware<RequiredScopesMiddleware>(new List<string> { Scopes.BotaNaRodaApiScope });

                theApi.UseMvc();
                theApi.UseSignalR();
                theApi.UseStaticFiles();
            });
        }

        public static void ConfigureLogging(IApplicationBuilder app, IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory)
        {
            LogglyConfig.Instance.ApplicationName = appSettings.Options.AppName;
            LogglyConfig.Instance.CustomerToken = appSettings.Options.LogglyCustomerToken;
            LogglyConfig.Instance.Transport = new TransportConfiguration
            {
                EndpointHostname = "logs-01.loggly.com",
                EndpointPort = 443,
                LogTransport = LogTransport.Https
            };
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Trace(outputTemplate: "{Timestamp:HH:MM} [{Level}] ({Name:l}){NewLine} {Message}{NewLine}{Exception}")
                .WriteTo.Loggly()
                .CreateLogger();

            loggerFactory.AddSerilog();
        }

        public static void ConfigureAdditionalIdentityProviders(IAppBuilder app, string signInAsType)
        {
            var fb = new FacebookAuthenticationOptions
            {
                AuthenticationType = "Facebook",
                SignInAsAuthenticationType = signInAsType,
                AppId = "450077978528041",
                AppSecret = "662a479402ffad82d300a1c6d87c6d8f",
                Scope = { "public_profile", "email" },
                Provider = new FacebookAuthenticationProvider
                {
                    OnAuthenticated = async context =>
                    {
                        var fbClaims = await FacebookUtil.GetClaimsAsync(context.AccessToken);
                        foreach (var claim in fbClaims)
                        {
                            if (!context.Identity.HasClaim(claim.Type, claim.Value))
                                context.Identity.AddClaim(claim);
                        }
                    }
                }
            };
            app.UseFacebookAuthentication(fb);
        }

    }
}
