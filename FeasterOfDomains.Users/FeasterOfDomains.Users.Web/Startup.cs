using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FeasterOfDomains.Users.Infrastructure;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FeasterOfDomains.Users.Web
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

            services.AddDbContext<FeasterDbContext>(options => 
                options.UseSqlServer(Configuration["ConnectionString"], sql => sql.MigrationsAssembly(typeof(FeasterDbContext).Assembly.GetName().Name)));

            services.AddScoped<IUserStore<FeasterUser>, UserStore<FeasterUser, IdentityRole, FeasterDbContext>>();
            services.AddScoped<IUserValidator<FeasterUser>, UserValidator<FeasterUser>>();
            services.AddScoped<IPasswordValidator<FeasterUser>, PasswordValidator<FeasterUser>>();
            services.AddScoped<UserManager<FeasterUser>, AspNetUserManager<FeasterUser>>();
            services.AddScoped<RoleManager<IdentityRole>, AspNetRoleManager<IdentityRole>>();
            services.AddScoped<IRoleStore<IdentityRole>, RoleStore<IdentityRole, FeasterDbContext>>();

            services.AddIdentity<FeasterUser, IdentityRole>()
                .AddEntityFrameworkStores<FeasterDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
                options.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
                options.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject;
                options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 15;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+\\";
            });

            /* 
            services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
                .AddInMemoryClients(AccountOptions.Clients)
                .AddAspNetIdentity<FeasterUser>();*/

            services.AddAuthentication();


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
            MigrateAllContexts(serviceProvider);
        }

        private void MigrateAllContexts(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetService<FeasterDbContext>().Database.Migrate();
                RegisterDefaultRoles(scope.ServiceProvider.GetService<RoleManager<IdentityRole>>());
                RegisterDefaultUsers(scope.ServiceProvider.GetService<UserManager<FeasterUser>>());
            }
        }

        private void RegisterDefaultRoles(RoleManager<IdentityRole> roleManager)
        {
            RegisterRole(roleManager, "Front office");
            RegisterRole(roleManager, "Finance");
            RegisterRole(roleManager, "Risk");
        }

        private void RegisterRole(RoleManager<IdentityRole> roleManager, string name)
        {
            if(roleManager.FindByNameAsync(name).Result == null)
            {
                var role = new IdentityRole
                {
                    Name = name
                };
                var result = roleManager.CreateAsync(role).Result;
                if(!result.Succeeded)
                {
                    throw new IdentityException<IdentityRole>(name, result.Errors);
                }
            }
        }

        private void RegisterDefaultUsers(UserManager<FeasterUser> userManager)
        {
            // https://www.fantasynamegenerators.com/imp-names.php
            RegisterUser(userManager, "Goltox", "Front office");
            RegisterUser(userManager, "Qegmok", "Finance");
            RegisterUser(userManager, "Nagla", "Risk");
        }

        private void RegisterUser(UserManager<FeasterUser> userManager, string name, string role)
        {
            if(userManager.FindByNameAsync(name).Result == null)
            {
                var user = new FeasterUser
                {
                    UserName = name
                };
                var result = userManager.CreateAsync(user, "MySecurePassword").Result;
                if(result.Succeeded)
                {
                    user = userManager.FindByNameAsync(name).Result;
                    result = userManager.AddToRoleAsync(user, role).Result;
                }
                // If either the create or the granting of a role failed
                if(!result.Succeeded)
                {
                    throw new IdentityException<FeasterUser>(name, result.Errors);
                }
            }
        }
    }
}
