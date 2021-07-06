using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Tweetbook.Contracts.HealthChecks;

namespace Tweetbook.Extensions
{
    public static class ConfigureApplicationExtensions
    {
        /// <summary>
        /// Adding Health Checks | ASP.NET Core 5
        /// https://youtu.be/bdgtYbGYsK0?list=PLUOequmGnXxOgmSDWU7Tl6iQTsOtyjtwU
        /// </summary>
        /// <param name="app"></param>
        public static void UseHealthChecksConfig(this IApplicationBuilder app)
        {
            // Ativando o middlweare de Health Check
            //app.UseHealthChecks("/status");

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";

                    var response = new HealthCheckResponse
                    {
                        Status = report.Status.ToString(),
                        Checks = report.Entries.Select(x => new HealthCheck
                        {
                            Component = x.Key,
                            Status = x.Value.Status.ToString(),
                            Description = x.Value.Description,
                            ErrorMessage = x.Value.Exception?.Message
                        }),
                        Duration = report.TotalDuration
                    };

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                }
            });


            // Gera o endpoint que retornará os dados utilizados no dashboard /hc
            app.UseHealthChecks("/healthchecks-data-ui", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            // Ativa o dashboard para a visualização da situação de cada Health Check
            //app.UseHealthChecksUI();
            app.UseHealthChecksUI(options =>
            {
                options.UIPath = "/healthchecks-ui";
                options.ApiPath = "/health-ui-api";
            });


        }
    }
}
