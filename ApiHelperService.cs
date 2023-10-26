using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace zidy.FreshBooks.core;

public class ApiHelperService
{
    public ApiHelperService() { }
    public static async Task<T?> GetAsync<T>(string baseUrl, Dictionary<string, string> headers)
    {
        using (HttpClient client = new HttpClient())
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            using (HttpResponseMessage res = await client.GetAsync(baseUrl))
            {
                res.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                using (HttpContent content = res.Content)
                {
                    var response = await content.ReadAsStringAsync();
                    var a = content.ReadAsStringAsync().Result;
                    if (TryParseJson<T>(response))
                    {
                        return JsonConvert.DeserializeObject<T>(response);
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }
        }
    }

    public static async Task<T?> PostAsync<T>(string baseUrl, object body, Dictionary<string, string> headers)
    {
        using (HttpClient client = new HttpClient())
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            var requestBodyString = JsonConvert.SerializeObject(body);
            var httpContent = new StringContent(requestBodyString, Encoding.UTF8, "application/json");
            using (HttpResponseMessage res = await client.PostAsync(baseUrl, httpContent))
            {
                res.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                using (HttpContent content = res.Content)
                {
                    var response = await content.ReadAsStringAsync();
                    var a = content.ReadAsStringAsync().Result;
                    if (TryParseJson<T>(response))
                    {
                        return JsonConvert.DeserializeObject<T>(response);
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }
        }

    }

    public static async Task<T?> DeleteAsync<T>(string baseUrl, Dictionary<string, string> headers)
    {
        using (HttpClient client = new HttpClient())
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            using (HttpResponseMessage res = await client.DeleteAsync(baseUrl))
            {
                res.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                using (HttpContent content = res.Content)
                {
                    var response = await content.ReadAsStringAsync();
                    if (TryParseJson<T>(response))
                    {
                        return JsonConvert.DeserializeObject<T>(response);
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }
        }

    }


    private static bool TryParseJson<T>(string jsonString)
    {
        try
        {
            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => { args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Error
            };
            JsonConvert.DeserializeObject<T>(jsonString);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}