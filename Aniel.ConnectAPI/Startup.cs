using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Aniel.Hibernate.Entity;
using HibernatingRhinos.Profiler.Appender.NHibernate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Net.Http.Headers;
using NHibernate;
using Swashbuckle.AspNetCore.Swagger;

namespace Aniel.ConnectAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            this.HostingEnvironment = env;
            this.Configuration = configuration;
        }

        public IHostingEnvironment HostingEnvironment { get; private set; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options=> {
                options.RespectBrowserAcceptHeader = true;
                options.FormatterMappings.SetMediaTypeMappingForFormat("xml", MediaTypeHeaderValue.Parse("text/xml"));
                options.FormatterMappings.SetMediaTypeMappingForFormat("json", MediaTypeHeaderValue.Parse("application/json"));
            })
            //.AddXmlSerializerFormatters()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddCors(options =>
                        options.AddPolicy("AllowAll",
                        p => p.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()));

            services.AddApiVersioning(options => options.ReportApiVersions = true);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Integração API", Version = "v1" });
            });

            //services.Configure<IISOptions>(o =>
            //{
            //    o.ForwardClientCertificate = false;
            //});

            services.AddSingleton<ISessionFactory>((provider) => {

                //NHibernateProfiler.InitializeUsingConfiguration(String.Format(@"{0}\NHProf.config", Directory.GetCurrentDirectory()));

                var cfg = new NHibernate.Cfg.Configuration();

                cfg.Configure(String.Format(@"{0}\hibernate.cfg.xml", Directory.GetCurrentDirectory()));

                try
                {
                    cfg.AddAssembly(typeof(BaseClass).GetTypeInfo().Assembly);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Erro ao carregar assemblys Business ({0}). {1}",
                        ex.Message, ex.InnerException == null ? String.Empty : ex.InnerException.Message), ex);
                }

                return cfg.BuildSessionFactory();
            });

            services.AddScoped<ISession>((provider) => {
                var factory = provider.GetService<ISessionFactory>();
                var session = factory.OpenSession();
                session.FlushMode = FlushMode.Always;

                return session;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });
            app.UseCors("AllowAll");

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Integração API");
            });

            //var option = new RewriteOptions();
            //option.AddRedirect("^$", "swagger");
            //app.UseRewriter(option);

            //app.UseHttpsRedirection();
            app.UseMvc(routers => 
            {
                routers.MapRoute(
                    name: "DefaultApi",
                    template: "{controller=Values}/{id?}"
                );
            });
        }
    }
}
