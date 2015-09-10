using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
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
using Microsoft.Framework.Runtime;
using Microsoft.Owin.Security.Facebook;
using Newtonsoft.Json.Linq;
using Owin;
using Serilog;
using IdentityServer3;
using Loggly;
using Loggly.Config;
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
            services.Configure<AppSettings>(Configuration.GetConfigurationSection("AppSettings"));

            services.AddTransient<ItemsContext>();

            services.AddMvc();

            services.AddSignalR();
            
            // Uncomment the following line to add Web API services which makes it easier to port Web API 2 controllers.
            // You will also need to add the Microsoft.AspNet.Mvc.WebApiCompatShim package to the 'dependencies' section of project.json.
            // services.AddWebApiConventions();

            services.AddDataProtection();
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IApplicationEnvironment env, IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory)
        {
            ConfigureLogging(app, appSettings, loggerFactory);

            app.UseStaticFiles();


            app.Map("/core", core =>
            {
                var idsrvOptions = new IdentityServerOptions
                {
                    IssuerUri = "https://botanaroda.com.br",
                    SiteName = "Bota na Roda",

                    RequireSsl = false,
                    SigningCertificate = new X509Certificate2(env.ApplicationBasePath + "\\Identity\\idsrv3test.pfx", "idsrv3test"),
                    Factory = new IdentityServerServiceFactory()
                        {
                            UserService = new Registration<IUserService>(resolver => new UserService(new ItemsContext(appSettings))),
                            CorsPolicyService = new Registration<ICorsPolicyService>(new DefaultCorsPolicyService { AllowAll = true })
                        }
                        .UseInMemoryClients(Clients.Get())
                        .UseInMemoryScopes(Scopes.Get()),
                    

                    AuthenticationOptions = new AuthenticationOptions
                    {
                        IdentityProviders = ConfigureAdditionalIdentityProviders
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

            app.Map("/api", api =>
            {
                //Really? yet??
                JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

                api.UseOAuthBearerAuthentication(options =>
                {
                    options.Authority = appSettings.Options.IdSvrAuthority;
                    options.Audience = "https://botanaroda.com.br/resources";
                    options.AutomaticAuthentication = true;
                });

                api.UseMiddleware<RequiredScopesMiddleware>(new List<string> { Scopes.BotaNaRodaApiScope });

                api.UseMvc();

                app.UseSignalR();
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
                        string userInformationEndpoint = "https://graph.facebook.com/me?fields=name,email,picture&access_token=" + Uri.EscapeDataString(context.AccessToken);

                        HttpResponseMessage graphResponse = await new HttpClient().GetAsync(userInformationEndpoint);
                        graphResponse.EnsureSuccessStatusCode();
                        var text = await graphResponse.Content.ReadAsStringAsync();
                        JObject user = JObject.Parse(text);

                        foreach (var x in user)
                        {
                            var claimType = $"urn:facebook:{x.Key}";
                            string claimValue = x.Value.ToString();
                            if (!context.Identity.HasClaim(claimType, claimValue))
                                context.Identity.AddClaim(new Claim(claimType, claimValue, XmlSchemaString, "Facebook"));
                        }

                        //Parse facebook picture object into our custom avatar url claim
                        context.Identity.AddClaim(new Claim(Constants.ClaimTypes.Picture, (string)user["picture"]["data"]["url"]));
                    }
                }
            };
            app.UseFacebookAuthentication(fb);
        }

        const string XmlSchemaString = "http://www.w3.org/2001/XMLSchema#string";
    }
}
