using ChoETL;
using Newtonsoft.Json;
using System;
using System.IO;

namespace ForzaExport.Application.Helpers
{
    public class JsonToCsvHelper
    {
        private const string EXPORT_DIRECTORY = @"\FrozaExport\Exports";
        public string ForzaGetProductsResponseToCsv(string json)
        {
            Console.WriteLine("Exporting Forza products json to csv ...");
            string fileName = $"forza-products-{DateTime.Now.ToString("yyyyMMddhhmmss")}.csv";
            string exportPath =  @$"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{EXPORT_DIRECTORY}";
            Directory.CreateDirectory(exportPath);

            string filePath = Path.Combine(exportPath, fileName);

            using (var jsonTextReader = new JsonTextReader(new StringReader(json)))
            using (var choJSONReader = new ChoJSONReader(jsonTextReader))
            using (var choCSVWriter = new ChoCSVWriter(filePath).WithFirstLineHeader())
                choCSVWriter.Write(choJSONReader);

            Console.WriteLine("Exporting Forza products json to csv succeeded.");
            Console.WriteLine($"File was exported to: {filePath}");

            return filePath;
        }
    }
}
