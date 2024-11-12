namespace AppMonederoCommand.Services.Interfaces.AzureBlobStorage
{
    public interface IServAzureBlobStorage
    {

        /// <summary>
        /// This method uploads a file submitted with the request
        /// </summary>
        /// <param name="file">File for upload</param>
        /// <param name="container">Container where to submit the file</param>
        /// <param name="blobName">Blob name to update</param>
        /// <returns>File name saved in Blob contaienr</returns>
        Task<IMDResponse<string>> UploadAsync(string imageBase64, ContainerEnum container, string blobName = null);

        /// <summary>
        /// This method deleted a file with the specified filename
        /// </summary>
        /// <param name="container">Container where to delete the file</param>
        /// <param name="blobFilename">Filename</param>
        Task<IMDResponse<dynamic>> DeleteAsync(ContainerEnum container, string blobFilename);


        /// <summary>
        /// Lista los archivos almacenados
        /// </summary>
        /// <param name="container">Container where to delete the file</param>
        Task<IMDResponse<dynamic>> ListAsync(ContainerEnum container);

        Task<IMDResponse<dynamic>> DownloadAsync(ContainerEnum container, string nameImage);
    }
}
