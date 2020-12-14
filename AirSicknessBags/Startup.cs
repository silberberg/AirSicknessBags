using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AirSicknessBags.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace AirSicknessBags
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
            string constr = Configuration.GetConnectionString("ConnectionStringHostGator");
            // I could do this without Pool, that is AddDbContext but it spins up a new connection every time without Pool
            services.AddDbContextPool<BagContext>(option => option.UseMySql(constr));
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<BagContext>();
            services.Configure<IdentityOptions>(option =>
           {
               option.Password.RequiredLength = 10;
           });
            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            // Allows us to upload files to server
            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images")));

            //services.AddRazorPages().AddRazorRuntimeCompilation();
            //services.AddRazorPages();
            services.AddMemoryCache();
            services.AddScoped<ISimpleCacheService, SimpleCacheService>();
            services.AddScoped<IGenericCacheService, ComplexCacheService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Home/Error");
                //// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
