using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using ElectronicsShop.DAL.Context;
using ElectronicsShop.DAL.Entities;
using ElectronicsShop.DAL.ViewModels;
using ElectronicsShop.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ElectronicsShop.Web
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
            services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));
            services.Configure<CookiePolicyOptions>(Options =>
            {
                Options.CheckConsentNeeded = context => true;
                Options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddControllersWithViews().AddViewLocalization(opts => { opts.ResourcesPath = "Resources"; }).
                AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix).AddDataAnnotationsLocalization().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);
            services.AddDistributedMemoryCache();
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(1000);
                options.Cookie.IsEssential = true;
            });
            var connection = Configuration.GetConnectionString("DatabaseConnection");
            services.AddDbContext<ElectronicsShopContext>(option => option.UseSqlServer(connection));

            services.Configure<RequestLocalizationOptions>(Options =>
            {
                var culturesSupported = new[]
                {
                    new CultureInfo("en-US"),
                   new CultureInfo("ar-EG")


                };
                Options.DefaultRequestCulture = new RequestCulture("en-US");
                Options.SupportedCultures = culturesSupported;
                Options.SupportedUICultures = culturesSupported;
                //        Options.RequestCultureProviders = new List<IRequestCultureProvider>
                //{
                //    new QueryStringRequestCultureProvider(),
                //    new CookieRequestCultureProvider()
                //};

            });
            #region Identity
            services.AddIdentity<ApplicationUser, ApplicationRole>(opt =>
            {
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 د ج ح خ ه ع غ  ف ق ث ص ض ذ ط ك م ن ت ا ل ب ي س ش ظ ز و ة ى لا ر ؤ ء ئ-._@+";
            })
                .AddEntityFrameworkStores<ElectronicsShopContext>()
                .AddDefaultTokenProviders();
            #endregion
            // JWT Authentication 
            #region JWT
            var key = Encoding.UTF8.GetBytes("1234567890123456");
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero

                };
            }
            );
            #endregion
            #region Services
            // services.AddSingleton<SpecialtyService>();
            services.AddScoped<ProductService>();
            services.AddScoped<CategoryService>();
            services.AddTransient<UserService>();

            #endregion
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseRouting();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            var localizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(localizationOptions);
            app.UseSession();
            app.Use(async (context, next) =>
            {

                var JWToken = context.Request.Cookies["JWToken"];
                //var JWToken = context.Session.GetString("JWToken");
                if (!string.IsNullOrEmpty(JWToken))
                {
                    context.Request.Headers.Add("Authorization", "Bearer " + JWToken);
                }
                await next();
            });
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
