using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Data;
using BotaNaRoda.WebApi.Identity;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using Microsoft.Framework.Runtime;
using Microsoft.Owin.Security.Facebook;
using Owin;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Logging;
using Thinktecture.IdentityServer.Core.Services;

namespace BotaNaRoda.WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            // Setup configuration sources.
            var builder = new ConfigurationBuilder(appEnv.ApplicationBasePath)
                .AddEnvironmentVariables()
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
            // Uncomment the following line to add Web API services which makes it easier to port Web API 2 controllers.
            // You will also need to add the Microsoft.AspNet.Mvc.WebApiCompatShim package to the 'dependencies' section of project.json.
            // services.AddWebApiConventions();

            services.AddDataProtection();
            services.AddLogging();
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IApplicationEnvironment env, IOptions<AppSettings> appSettings)
        {
            LogProvider.SetCurrentLogProvider(new DiagnosticsTraceLogProvider());

            //app.UseStaticFiles();
            app.UseMvc();

            app.Map("/core", core =>
            {
                var factory = InMemoryFactory.Create(
                                clients: Clients.Get(),
                                scopes: Scopes.Get());
                factory.UserService = new Registration<IUserService>(resolver => new UserService(new ItemsContext(appSettings)));

                var idsrvOptions = new IdentityServerOptions
                {
                    IssuerUri = "https://botanaroda.com.br",
                    SiteName = "Bota na Roda",

                    RequireSsl = false,
                    SigningCertificate = new X509Certificate2(env.ApplicationBasePath + "\\Identity\\idsrv3test.pfx", "idsrv3test"),
                    Factory = factory,
                    CorsPolicy = CorsPolicy.AllowAll,

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
        }


        public static void ConfigureAdditionalIdentityProviders(IAppBuilder app, string signInAsType)
        {
            var fb = new FacebookAuthenticationOptions
            {
                AuthenticationType = "Facebook",
                SignInAsAuthenticationType = signInAsType,
                AppId = "450077978528041",
                AppSecret = "662a479402ffad82d300a1c6d87c6d8f"
            };
            app.UseFacebookAuthentication(fb);
        }
    }
}
