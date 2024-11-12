using AppMonederoCommand.Services.Interfaces.AzureBlobStorage;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AppMonederoCommand.Services.AzureBlobStorage
{
    public class ServAzureBlobStorage : IServAzureBlobStorage
    {
        private readonly string _azureStorageConnectionString;
        private readonly ILogger<ServGenerico> _logger;

        public ServAzureBlobStorage(ILogger<ServGenerico> logger)
        {
            this._azureStorageConnectionString = Environment.GetEnvironmentVariable("AZUREBLOBSTORAGE_KEY") ?? string.Empty;
            _logger = logger;
        }


        public async Task<IMDResponse<dynamic>> DeleteAsync(ContainerEnum container, string blobFilename)
        {
            IMDResponse<dynamic> response = new IMDResponse<dynamic>();

            string metodo = nameof(this.DeleteAsync);
            _logger.LogInformation(IMDSerializer.Serialize(67823463533161, $"Inicia {metodo}(ContainerEnum container, string blobFilename)", container, blobFilename));

            try
            {
                var containerName = Enum.GetName(typeof(ContainerEnum), container).ToLower();
                var blobContainerClient = new BlobContainerClient(this._azureStorageConnectionString, containerName);
                var blobClient = blobContainerClient.GetBlobClient(blobFilename);
                Azure.Response borrado = await blobClient.DeleteAsync();

                response.SetSuccess(borrado);
            }
            catch (RequestFailedException ex)
            {

                if (ex.Status == 404)
                {
                    response.SetNoContent();
                }
                else
                {
                    response.ErrorCode = 67823463533938;
                    response.SetError(ex);
                    _logger.LogError(IMDSerializer.Serialize(67823463533938, $"Error en {metodo}(ContainerEnum container, string blobFilename): {ex.Message}", container, blobFilename, ex, response));
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823463533938;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823463533938, $"Error en {metodo}(ContainerEnum container, string blobFilename): {ex.Message}", container, blobFilename, ex, response));
            }
            return response;
        }


        public async Task<IMDResponse<string>> UploadAsync(string imageBase64, ContainerEnum container, string blobName)
        {
            IMDResponse<string> response = new IMDResponse<string>();

            string metodo = nameof(this.UploadAsync);
            _logger.LogInformation(IMDSerializer.Serialize(67823463530053, $"Inicia {metodo}(IFormFile file, ContainerEnum container, string blobName)", container, blobName));

            try
            {
                //SearchOption optiene el contentType
                if (string.IsNullOrEmpty(imageBase64))
                {
                    response.SetError("No hay una imagen a subir");
                    return response;
                }

                string[] base64Data = imageBase64.Split(',');
                string contentType = string.Empty;
                if (base64Data.Length == 2)
                {
                    contentType = base64Data[0].Split(':')[1].Split(';')[0];
                }


                var containerName = Enum.GetName(typeof(ContainerEnum), container).ToLower();

                var blobContainerClient = new BlobContainerClient(this._azureStorageConnectionString, containerName);

                // Get a reference to the blob just uploaded from the API in a container from configuration settings
                if (string.IsNullOrEmpty(blobName))
                {
                    response.SetError("El archivo no tiene un nombre definido para ser almacenado en el Storage");
                    return response;
                }
                var blobClient = blobContainerClient.GetBlobClient(blobName);

                var blobHttpHeader = new BlobHttpHeaders { ContentType = contentType };

                string base64Image = base64Data[1];

                byte[] imageBytes = Convert.FromBase64String(base64Image);
                // Open a stream for the file we want to upload
                await using (MemoryStream imageStream = new MemoryStream(imageBytes))
                {
                    // Upload the file async
                    await blobClient.UploadAsync(imageStream, new BlobUploadOptions { HttpHeaders = blobHttpHeader });


                }
                response.SetSuccess(blobName);  
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823463530830;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823463530830, $"Error en {metodo}(IFormFile file, ContainerEnum container, string blobName): {ex.Message}", container, blobName, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<dynamic>> DownloadAsync(ContainerEnum container, string nameImage)
        {
            IMDResponse<dynamic> response = new IMDResponse<dynamic>();

            string metodo = nameof(this.DownloadAsync);
            _logger.LogInformation(IMDSerializer.Serialize(67823463545593, $"Inicia {metodo}(ContainerEnum container, string nameImage)", container, nameImage));

            try
            {
                var containerName = Enum.GetName(typeof(ContainerEnum), container).ToLower();

                BlobContainerClient blobContainerClient = new BlobContainerClient(this._azureStorageConnectionString, containerName);

                BlobClient blobClient = blobContainerClient.GetBlobClient(nameImage);

                if (!await blobClient.ExistsAsync())
                {
                    throw new InvalidOperationException($"El blob {containerName} no existe en el contenedor.");
                }

                Stream imageStream = await blobClient.OpenReadAsync();

                string mimeType = blobClient.GetProperties().Value.ContentType;

                // Convertimos el stream a un array de bytes
                byte[] blobContent;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await imageStream.CopyToAsync(memoryStream);
                    blobContent = memoryStream.ToArray();
                }

                // Convertimos el contenido del Blob a Base64
                string base64Content = Convert.ToBase64String(blobContent);

                // Creamos una cadena Base64 con el tipo de archivo
                string base64StringWithType = $"data:{mimeType};base64,{base64Content}";

                response.SetSuccess(base64StringWithType);
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823463546370;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823463546370, $"Error en {metodo}(ContainerEnum container, string nameImage): {ex.Message}", container, nameImage, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<dynamic>> ListAsync(ContainerEnum container)
        {
            IMDResponse<dynamic> response = new IMDResponse<dynamic>();

            string metodo = nameof(this.ListAsync);
            _logger.LogInformation(IMDSerializer.Serialize(67823463531607, $"Inicia {metodo}(ContainerEnum container)", container));

            try
            {
                var containerName = Enum.GetName(typeof(ContainerEnum), container).ToLower();
                var blobContainerClient = new BlobContainerClient(this._azureStorageConnectionString, containerName);

                List<object> blobList = new List<object>();


                await foreach (BlobItem blobItem in blobContainerClient.GetBlobsAsync())
                {
                    blobList.Add(new
                    {
                        name = blobItem.Name,
                        properties = blobItem.Properties.ContentLength
                    });
                }
                response.SetSuccess(blobList);
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823463532384;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823463532384, $"Error en {metodo}(ContainerEnum container): {ex.Message}", container, ex, response));
            }
            return response;
        }


    }
}
