using ForzaExport.Application.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ForzaExport.Application.Services
{
    public class ForzaHttpService
    {
        private readonly HttpClient httpClient;

        public ForzaHttpService(ForzaOptions forzaOptions)
        {
            httpClient = new HttpClient()
            {
                BaseAddress = new Uri(forzaOptions.BaseUrl),
            };
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", forzaOptions.ApiKey);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<string> GetProducts()
        {
            try
            {
                Console.WriteLine("Getting products from Forza API ...");
                HttpResponseMessage response = await httpClient.GetAsync("products");
                response.EnsureSuccessStatusCode();
                Console.WriteLine("Getting products from Forza API succeeded.");
                return await response.Content.ReadAsStringAsync();
  
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException ?? e);
                throw;
            }
        }
    }
}