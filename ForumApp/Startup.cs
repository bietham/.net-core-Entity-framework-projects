using AutoMapper;
using ForumApp.Seeder;
using ForumApp.Services;
using ForumApp.Store;
using ForumApp.Store.Roles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumApp
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
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
                options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new WebAppMapperProfile());
            });


            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            services.AddControllersWithViews();
            services.AddScoped<IForumSectionService, ForumSectionService>();
            services.AddScoped<ITopicService, TopicService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddTransient<IDbSeederService, DbSeederService>();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context, IDbSeederService seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            context.Database.Migrate();

            seeder.SeedAdminUser();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        private void InitRoles(IServiceCollection services)
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

                foreach (var r in Roles.AllRoles)
                {
                    if (roleManager.Roles.Any(x => x.Name == r))
                    {
                        continue;
                    }

                    var result = roleManager.CreateAsync(
                        new IdentityRole
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = r
                        }).Result;
                }
            }
        }
    }
}
