using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql;
using Area.Models;
using Area.DAT;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Area
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
            services.AddMvc();
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddDbContext<AreaDbContext>(x => x.UseMySql("server=52.213.247.63;database=area;uid=dodo;pwd=ragnaros"));
            services.AddDbContext<AreaDbThreadContext>(x => x.UseMySql("server=52.213.247.63;database=area;uid=dodo;pwd=ragnaros"));
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
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            var DB = app.ApplicationServices.GetRequiredService<AreaDbContext>();
            DB.Database.EnsureCreated();
            var DBthread = app.ApplicationServices.GetRequiredService<AreaDbThreadContext>();
            DBthread.Database.EnsureCreated();
            Thread newThread = new Thread(runAreas);
            newThread.Start(DB);
        }

        public void runAreas(object Db)
        {
            try {
            AreaDbContext DB = (AreaDbContext)Db;
                while (true)
                {
                    foreach (var area in DB.areas.ToList())
                    {
                        IArea ar = AreaFactory.create(area, DB);
                        if (ar != null)
                            ar.run(DB);
                    }                
                    Thread.Sleep(10000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Message: " + e.Message);
                Console.WriteLine("Source: " + e.Source);
                System.Environment.Exit(0);
            }
        }
    }
}
