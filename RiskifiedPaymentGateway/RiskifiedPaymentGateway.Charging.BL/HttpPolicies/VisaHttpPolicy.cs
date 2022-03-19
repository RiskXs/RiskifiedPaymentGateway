using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RiskifiedPaymentGateway.Charging.BL.HttpPolicies
{
    public class VisaHttpPolicy
    {
        public const string Name = "VisaRetryPolicy";
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                                                                            retryAttempt)));
        }
    }
}
