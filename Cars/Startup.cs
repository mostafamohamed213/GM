using Cars.Models;
using Cars.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Cars.Service.Notification;
using Hangfire;
using Cars.Service.HangfireAuth;
using Cars.Service.Hangfire;
using Hangfire.MemoryStorage;
using Cars.Service.Email;

namespace Cars
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
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                   new CultureInfo("en-US"),
                   new CultureInfo("ar-EG"),

                };

                options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMemoryStorage()
                );

            // Add the processing server as IHostedService
            services.AddHangfireServer();

            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddTransient<PricingService, PricingService>();
            services.AddTransient<OrderServices, OrderServices>();
            services.AddTransient<BranchService, BranchService>();
            services.AddTransient<UserBranchService, UserBranchService>();
            services.AddTransient<LaborService, LaborService>();
            services.AddTransient<SystemIssuesServices, SystemIssuesServices>();
            services.AddTransient<OrderLineUsedService, OrderLineUsedService>();
            services.AddTransient<QuotationService, QuotationService>();
            services.AddTransient<FinanceService, FinanceService>();
            services.AddTransient<OrderDetailsService, OrderDetailsService>();
            services.AddTransient<WorkflowOrderDetailsLogsService, WorkflowOrderDetailsLogsService>();
            services.AddTransient<VendorLocationService, VendorLocationService>();
            services.AddTransient<RunnerService, RunnerService>();
            services.AddTransient<PurchasingService, PurchasingService>();
            services.AddTransient<AllOrderLinesService, AllOrderLinesService>();
            services.AddTransient<InventoryService, InventoryService>();
            services.AddTransient<RunnerOrdersService, RunnerOrdersService>();
            services.AddTransient<Cars.Service.NotificationService, Cars.Service.NotificationService>();
            services.AddTransient<Cars.Service.NotificationUserService, Cars.Service.NotificationUserService>();
            services.AddTransient<DeliveryService, DeliveryService>();
            services.AddTransient<UserService, UserService>();
            services.AddTransient<INotificationService, Cars.Service.Hangfire.NotificationService>();
            services.AddTransient<IEmailService,EmailService>();
            services.AddTransient<TeamBranchService, TeamBranchService>();


            services.AddSession();

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.Zero;
            });


            services.AddControllersWithViews();

            //SignalR
            services.AddSignalR();
            services.AddSingleton<IUserConnectionManager, UserConnectionManager>();

            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddDbContext<CarsContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("Cars")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;

            })
                .AddEntityFrameworkStores<CarsContext>()
                .AddDefaultTokenProviders();

            services.AddHttpContextAccessor();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Accounts/login";
                options.LogoutPath = $"/Accounts/logout";
                options.AccessDeniedPath = $"/Accounts/accessDenied";
                //  options.ExpireTimeSpan = TimeSpan.FromMinutes(3);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IRecurringJobManager recurringJobManager, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Hangfire background job every 30 min
            app.UseHangfireDashboard("/hangfire", new DashboardOptions()
            {
                Authorization = new[] { new CustomAuthorizeFilter() }
            });
            recurringJobManager.AddOrUpdate("Run every 30 min",
            () => serviceProvider.GetService<INotificationService>().SendNotificationsAsync(),
                 "*/30 * * * *");

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting();
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<NotificationHubService>("/NotificationHub");
            });
        }
    }
}
