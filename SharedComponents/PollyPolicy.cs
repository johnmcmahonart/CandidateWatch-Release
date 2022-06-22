using System;
using System.Collections.Generic;
using System.Text;
using Polly;
using Polly.RateLimit;

namespace SharedComponents
{
    public class PollyPolicy
    {
        private static Polly.RateLimit.AsyncRateLimitPolicy _rateLimit => Policy.RateLimitAsync(10, TimeSpan.FromMinutes(1));
        private static Polly.Retry.AsyncRetryPolicy _retry = Policy.Handle<RateLimitRejectedException>()
            .WaitAndRetryAsync(3, sleepDurationProvider: s => TimeSpan.FromMinutes(1));
        public static Polly.Wrap.AsyncPolicyWrap GetDefault
        {
            get
            {
                return Policy.WrapAsync(_rateLimit, _retry);
            }
        }


        
    }
}