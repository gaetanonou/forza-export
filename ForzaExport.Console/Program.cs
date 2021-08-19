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
                if (string.IsNullOrWhiteSpace(forzaOptions.Username) || string.IsNullOrWhiteSpace(forzaOptions.Password))
                {
                    (string username, string password) = RequestUsernamePassword();

                    AppSettingsHelpers.AddOrUpdateAppSetting("ForzaOptions:Username", username);
                    AppSettingsHelpers.AddOrUpdateAppSetting("ForzaOptions:Password", password);

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

        private static (string, string) RequestUsernamePassword(string username = null, string password = null)
        {
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                Console.WriteLine("No username or password was found for the Forza API.");
        
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Enter username:");
                username = Console.ReadLine();
                if(string.IsNullOrWhiteSpace(username)) 
                {
                    Console.WriteLine("Username cannot be empty. Please try again.");
                    return RequestUsernamePassword(username);
                }
              
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Enter password:");
                password = Console.ReadLine();
                if(string.IsNullOrWhiteSpace(password)) 
                {
                    Console.WriteLine("Password cannot be empty. Please try again.");
                    return RequestUsernamePassword(username);
                }
              
            }

            return (username, password);
        }
    }
}
