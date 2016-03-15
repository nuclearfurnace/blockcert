using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Entity;
using BlockCert.Api.Database;
using Swashbuckle.SwaggerGen;

namespace BlockCert.Api
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			// Set up configuration sources.
			var builder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; set; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Add framework services.
			services.AddMvc();

			var dbConfig = Configuration.GetSection("Database");
			var connectionString = dbConfig.GetSection("ConnectionString").Value;

			services.AddEntityFramework()
				.AddNpgsql()
				.AddDbContext<BlockCertContext>(options => options.UseNpgsql(connectionString));

			services.AddSwaggerGen();
			services.ConfigureSwaggerDocument(options => {
				options.SingleApiVersion(new Info {
					Version = "v1",
					Title = "BlockCert",
					Description = "An API for the BlockCert verification service.",
					TermsOfService = ""
				});
			});

			services.ConfigureSwaggerSchema(options => {
				options.DescribeAllEnumsAsStrings = true;
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			app.UseIISPlatformHandler();

			app.UseDefaultFiles();
			app.UseStaticFiles();

			app.UseMvc();

			app.UseSwaggerGen();
			app.UseSwaggerUi();
		}

		// Entry point for the application.
		public static void Main(string[] args) => Microsoft.AspNet.Hosting.WebApplication.Run<Startup>(args);
	}
}