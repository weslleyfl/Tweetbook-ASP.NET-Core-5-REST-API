using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tweetbook.Data;
using Tweetbook.HealthChecks;


namespace Tweetbook.Installers
{
    /// <summary>
    /// https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/master/README.md
    /// </summary>
    public class HealthChecksInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            // Verificando a disponibilidade dos bancos de dados
            // da aplicação através de Health Checks
            // e verificando a disponibilidade do Redis
            services
                .AddHealthChecks()
                .AddDbContextCheck<DataContext>()
                .AddCheck<RedisHealthCheck>("Redis");

            services
                .AddHealthChecksUI()
                .AddInMemoryStorage();

        }
    }
}
