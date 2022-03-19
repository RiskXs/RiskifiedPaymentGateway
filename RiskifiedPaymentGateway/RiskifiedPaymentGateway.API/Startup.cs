using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using RiskifiedPaymentGateway.API.Services;
using RiskifiedPaymentGateway.API.Validators;
using RiskifiedPaymentGateway.Charging.BL;
using RiskifiedPaymentGateway.Charging.BL.CreditCardChargers;
using RiskifiedPaymentGateway.Charging.BL.HttpPolicies;
using RiskifiedPaymentGateway.Charging.DAL;
using RiskifiedPaymentGateway.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RiskifiedPaymentGateway.API
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
            services.AddScoped<IChargeValidator, ChargeValidator>();
            services.AddScoped<IChargingService, ChargingService>();
            // Core
            services.AddScoped<ICreditCardCompanyValidator, CreditCardCompany>();
            //Charging
            services.AddOptions<VisaChargingSettings>().BindConfiguration("Visa:Charging");
            services.AddOptions<MasterCardChargingSettings>().BindConfiguration("MasterCard:Charging");
            services.AddScoped<VisaCreditCardCharger>();
            services.AddScoped<MasterCardCreditCardCharger>();
            services.AddScoped<ICreditCardChargerFactory, CreditCardChargerFactory>();
            services.AddScoped<IChargingManager, ChargingManager>();
            services.AddSingleton<IChargeStatusRepository, InMemoryChargeStatusRepository>();

            services.AddHttpClient(MasterCardHttpRetryPolicy.Name)
                        .AddPolicyHandler(MasterCardHttpRetryPolicy.GetRetryPolicy(3));
            services.AddHttpClient(VisaHttpPolicy.Name)
                        .AddPolicyHandler(MasterCardHttpRetryPolicy.GetRetryPolicy(3));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RiskifiedPaymentGateway.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RiskifiedPaymentGateway.API v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
