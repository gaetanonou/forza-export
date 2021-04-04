using ForzaExport.Application.Helpers;
using ForzaExport.Application.Options;
using ForzaExport.Application.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ForzaExport
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.Title = "Forza Export";
                #region CONFIGURATION
                string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                .AddJsonFile($"appsettings.{environment}.json", optional: true);

                IConfiguration configuration = configurationBuilder
                                .Build();

                ForzaOptions forzaOptions = configuration.GetSection(nameof(ForzaOptions)).Get<ForzaOptions>();
                if (string.IsNullOrWhiteSpace(forzaOptions.ApiKey))
                {
                    string apiKey = RequestApiKey();

                    AppSettingsHelpers.AddOrUpdateAppSetting("ForzaOptions:ApiKey", apiKey);
                    configuration = configurationBuilder.Build();
                    forzaOptions = configuration.GetSection(nameof(ForzaOptions)).Get<ForzaOptions>();
                }

                #endregion

                ForzaHttpService forzaHttpService = new ForzaHttpService(forzaOptions);
                string getProductsResopnse = await forzaHttpService.GetProducts();

                JsonToCsvHelper jsonToCsvHelper = new JsonToCsvHelper();
                string exportPath = jsonToCsvHelper.ForzaGetProductsResponseToCsv(getProductsResopnse);

                Process.Start("explorer.exe", exportPath);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.InnerException ?? e);
                Console.Read();
            }
          
        }

        private static string RequestApiKey()
        {
            Console.WriteLine("No API Key was found for the Forza API.");
            Console.WriteLine("Enter API Key:");
            string apiKey = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                Console.WriteLine("API Key cannot be empty. Please try again.");
                return RequestApiKey();
            }

            return apiKey;
        }
    }
}
