using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            /*
                Toda vez que referenciarmos a nossa classe do mongoDB, precisaremos de uma 
                instância Singleton na injeção de dependencias, pois o mongo vai mappear 
                a nossa entidade para criar a collection, e assim receber os documents
                da melhor forma.

                Se fosse Scoped, ou Transient, ele iria mapear isso toda hora, ia ficar meio perdido.
            */
            services.AddSingleton<Data.MongoDB>();
            // Vamos ter as nossas famigeradas Controllers
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Redireciona para HTTPS se bater no http
            // app.UseHttpsRedirection();
            // Vamos usar rotas
            app.UseRouting();
            // Vamos mapear os endpoints nas controllers
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
