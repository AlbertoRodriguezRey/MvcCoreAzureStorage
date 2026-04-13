using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MvcCoreAzureStorage.Models;

namespace MvcCoreAzureStorage.Services
{
    public class ServiceStorageBlobs
    {
        private BlobServiceClient client;

        public ServiceStorageBlobs(BlobServiceClient client)
        {
            this.client = client;
        }

        public async Task<List<string>> GetContainersAsync()
        {
            List<string> containers = new List<string>();
            await foreach (BlobContainerItem container in this.client.GetBlobContainersAsync())
            {
                containers.Add(container.Name);
            }
            return containers;
        }

        //METODO PARA CREAR UN CONTAINER
        public async Task CreateContainerAsync(string containerName)
        {
            await this.client.CreateBlobContainerAsync(containerName.ToLower(), PublicAccessType.Blob);
        }

        //METODO PARA ELIMINAR UN CONTAINER
        public async Task DeleteContainerAsync(string containerName)
        {
            await this.client.DeleteBlobContainerAsync(containerName);
        }

        //LISTADO DE FICHEROS DENTRO DE UN CONTAINER
        public async Task<List<BlobModel>> GetBlobsAsync(string containerName)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            List<BlobModel> blobs = new List<BlobModel>();
            await foreach (BlobItem item in containerClient.GetBlobsAsync())
            {
                BlobClient blobClient = containerClient.GetBlobClient(item.Name);
                BlobModel blob = new BlobModel();
                blob.Nombre = item.Name;
                blob.Container = containerName;
                blob.Url = blobClient.Uri.ToString();
                blobs.Add(blob);
            }
            return blobs;
        }

        //METODO PARA ELIMINAR UN FICHERO
        public async Task DeleteBlobAsync(string containerName, string blobName)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteAsync();
        }
        //METODO PARA SUBIR UN FICHERO
        public async Task UploadBlobAsync(string containerName, string blobName, Stream stream)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            await containerClient.UploadBlobAsync(blobName, stream);
        }
    }
}
