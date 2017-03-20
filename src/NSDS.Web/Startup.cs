using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSDS.Data;
using NSDS.Core;
using NSDS.Core.Interfaces;
using NSDS.Core.Jobs;
using NSDS.Core.Services;
using System.Reflection;
using NSDS.Data.Services;
using NSDS.Web.Commands;

namespace NSDS.Web
{
    public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

			if (env.IsDevelopment())
			{
				// For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
				builder.AddUserSecrets<Startup>();
			}

			builder.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Add framework services.
			services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);

			services.AddMvc();

			// Add application services.
			services.AddTransient<IPoolStorage, PoolStorage>();
			services.AddTransient<IClientsStorage, ClientStorage>();
			services.AddTransient<IModuleStorage, ModuleStorage>();
			services.AddTransient<IPackageStorage, PackageStorage>();
			services.AddTransient<IDeploymentStorage, DeploymentStorage>();

			services.AddTransient<IDeploymentService, DeploymentService>();

			services.AddSingleton<IEventService>(new EventService());

			services.AddSingleton(typeof(JobRunner));

			services.AddSingleton(new ConnectionFactory().Add("http", uri => new HttpConnection(uri)));

			services.RegisterTypes(typeof(JobBase), Assembly.Load(new AssemblyName("NSDS.Core")));

			ApplicationDbContext.CommandTypes.Add(typeof(SshCommand));

			var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

			services.AddSingleton<ILoggerFactory>(loggerFactory);
			services.AddSingleton(loggerFactory.CreateLogger(string.Empty));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IServiceProvider services, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            //app.UseIdentity();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                //routes.MapRoute("api", "api/[controller]");
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

			using (var context = services.GetService<ApplicationDbContext>())
			{
				context.Seed();
			}

            services.GetService<JobRunner>().Start();
        }
    }
}
