using System;
using System.Collections.Generic;
using System.Text;
using Polly;
using Polly.RateLimit;

namespace SharedComponents
{
    public static class PollyPolicy
    {
        private static Polly.RateLimit.AsyncRateLimitPolicy _rateLimit = Policy.RateLimitAsync(10, TimeSpan.FromSeconds(30), 10);
        private static Polly.Retry.AsyncRetryPolicy _retry = Policy.Handle<RateLimitRejectedException>().WaitAndRetryAsync(3, sleepDurationProvider: s => TimeSpan.FromSeconds(35), onRetry: (exception, sleepDuration) => { Console.WriteLine("Retry Triggered"); });
        public static Polly.Wrap.AsyncPolicyWrap GetDefault
        {
            get
            {
                return Policy.WrapAsync(_retry, _rateLimit); //policy order matters here!! rate limit policy before retry will NOT work
            }
        }

        
        
    }
}