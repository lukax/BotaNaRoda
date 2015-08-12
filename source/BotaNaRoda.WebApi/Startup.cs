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
using Microsoft.Framework.Runtime;
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
                .AddJsonFile("config.json")
                .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // This reads the configuration keys from the secret store.
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                //builder.AddUserSecrets();
            }
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by a runtime.
        // Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            // maps the AppSettings configuration key to an instance of the configuration class
            services.Configure<AppSettings>(Configuration.GetConfigurationSection("AppSettings"));

            services.AddSingleton<ItemsContext>();

            services.AddMvc();
            // Uncomment the following line to add Web API services which makes it easier to port Web API 2 controllers.
            // You will also need to add the Microsoft.AspNet.Mvc.WebApiCompatShim package to the 'dependencies' section of project.json.
            // services.AddWebApiConventions();

            services.AddDataProtection();
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IApplicationEnvironment env)
        {
            LogProvider.SetCurrentLogProvider(new DiagnosticsTraceLogProvider());

            //app.UseStaticFiles();
            app.UseMvc();

            app.Map("/core", core =>
            {
                var factory = InMemoryFactory.Create(
                                clients: Clients.Get(),
                                scopes: Scopes.Get());
                factory.UserService = new Registration<IUserService>(resolver => new UserService());

                var idsrvOptions = new IdentityServerOptions
                {
                    IssuerUri = "https://botanaroda.com.br",
                    Factory = factory,
                    RequireSsl = false,
                    SigningCertificate = new X509Certificate2(env.ApplicationBasePath + "\\Identity\\idsrv3test.pfx", "idsrv3test"),

                    CorsPolicy = CorsPolicy.AllowAll,

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
    }
}
