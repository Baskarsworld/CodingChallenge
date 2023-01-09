using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace CoffeeMachine.Infrastructure
{
    public static class HttpRequestMessageFactory
    {
        public static HttpRequestMessage Create(HttpMethod method, string requestUri, object content = null)
        {
            var message = new HttpRequestMessage(method, requestUri);
            var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() };

            if(content != null)
            {
                message.Content = new StringContent(
                    JsonConvert.SerializeObject(content, jsonSerializerSettings),
                    Encoding.UTF8,
                    "application/json");
            }

            return message;
        }
    }
}
