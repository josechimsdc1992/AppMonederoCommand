using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

namespace AppMonederoCommand.Utils
{
    public static class GenerateCSV
    {
        public static byte[] GenerarArchivoCSV<T>(List<T> elementos)
        {
            // Configuración de CsvHelper
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture);

            // Crear un memorystream para almacenar el archivo CSV
            using (var csvMemoryStream = new MemoryStream())
            using (var writer = new StreamWriter(csvMemoryStream, Encoding.GetEncoding("ISO-8859-1")))
            using (var csv = new CsvWriter(writer, csvConfig))
            {

                csv.WriteRecords(elementos); // Escribir los elementos en el archivo CSV
                csv.NextRecord();
                writer.Flush();

                // Devolver el archivo CSV como arreglo de bytes
                csvMemoryStream.Seek(0, SeekOrigin.Begin);
                return csvMemoryStream.ToArray();
            }
        }
        public static byte[] GenerarArchivoCSV<T>(T elemento)
        {
            // Configuración de CsvHelper
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture);

            // Crear un memorystream para almacenar el archivo CSV
            using (var csvMemoryStream = new MemoryStream())
            using (var writer = new StreamWriter(csvMemoryStream))
            using (var csv = new CsvWriter(writer, csvConfig))
            {
                List<T> listaElemento = new List<T> { elemento };
                csv.WriteRecords(listaElemento); // Escribir los elementos en el archivo CSV
                writer.Flush();

                // Devolver el archivo CSV como arreglo de bytes
                csvMemoryStream.Seek(0, SeekOrigin.Begin);
                return csvMemoryStream.ToArray();
            }
        }
    }
}
