using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ConsultaAcademica.Service;
using ConsultaAcademica.Model;
using ConsultaAcademica.Service.Contract;

namespace ConsultaAcademica.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private Tuple<string, string, string, string> GetApiConfigs()
        {
            var baseurl = Configuration["ConsultaAcademicaApiUrl"];
            var username = Configuration["ConsultaAcademicaApiUsername"];
            var password = Configuration["ConsultaAcademicaApiPassword"];
            var season = Configuration["ConsultaAcademicaApiPeriodoLetivoAtual"];

            return new Tuple<string, string, string, string>(baseurl, username, password, season);
        }

        public void FillAppConfig()
        {
            var apiConfigs = GetApiConfigs();
            AppConfig.ApiUrl = apiConfigs.Item1;
            AppConfig.ApiUsername = apiConfigs.Item2;
            AppConfig.ApiPassword = apiConfigs.Item3;
            AppConfig.ApiCurrentSemester = apiConfigs.Item4;

            AutenticacaoService service = new AutenticacaoService(AppConfig.ApiUrl);
            service.BaseUrl = AppConfig.ApiUrl;

            Autenticacao autenticacao = (Autenticacao)service.Autenticar(AppConfig.ApiUsername, AppConfig.ApiPassword);
            AppConfig.Token = autenticacao.Token;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            FillAppConfig();
            services.AddMvc();            

            services.AddTransient<IConsultaAcademicaService, ConsultaAcademicaService>((s) =>
            {
                var consultaAcademicaService = new ConsultaAcademicaService(AppConfig.ApiUrl)
                {
                    Token = AppConfig.Token
                };

                return consultaAcademicaService;
            });

            services.AddTransient<ISincroniaService, SincroniaService>((s) =>
            {
                var sincroninaService = new SincroniaService(AppConfig.ApiUrl)
                {
                    Token = AppConfig.Token
                };

                return sincroninaService;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
