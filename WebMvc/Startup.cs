using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebMvc
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //清空默认绑定的用户信息
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            //添加认证服务
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";                   //默认使用Cookies方案进行认证
                options.DefaultChallengeScheme = "oidc";        //默认认证失败时启用oidc方案
            }).AddCookie("Cookies")           //添加cookie认证方案
            .AddOpenIdConnect("oidc",options=> {

                options.SignInScheme = "Cookies";       //身份验证成功后使用Cookies方案来保存信息

                options.Authority = "http://localhost:57190";    //授权服务地址
                options.RequireHttpsMetadata = false;

                options.ClientId = "mvc_implicit";
                options.ResponseType = "id_token token";    //默认只返回id_token 这里添加上token(Access Token)
                options.SaveTokens = true;
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
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseAuthentication(); //启用认证

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
