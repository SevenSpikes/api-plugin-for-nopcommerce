using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs.NewsLetterSubscriptions
{
    public class NewsLetterSubscriptionsRootObject
    {
        public NewsLetterSubscriptionsRootObject()
        {
            NewsLetterSubscriptions = new List<NewsLetterSubscriptionDto>();
        }

        [JsonProperty("news_letter_subscriptions")]
        public IList<NewsLetterSubscriptionDto> NewsLetterSubscriptions { get; set; }
        
    }
}