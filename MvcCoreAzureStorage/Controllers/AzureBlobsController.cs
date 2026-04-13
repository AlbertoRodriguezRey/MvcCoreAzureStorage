using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using MvcCoreAzureStorage.Models;
using MvcCoreAzureStorage.Services;

namespace MvcCoreAzureStorage.Controllers
{
    public class AzureBlobsController : Controller
    {
        private ServiceStorageBlobs service;

        public AzureBlobsController(ServiceStorageBlobs service)
        {
            this.service = service;
        }
        public async Task<IActionResult> Index()
        {
            List<string> containers = await this.service.GetContainersAsync();
            return View(containers);
        }

        public IActionResult CreateContainer()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateContainer(string containerName)
        {
            if (string.IsNullOrWhiteSpace(containerName))
            {
                ViewData["MENSAJE"] = "Debe indicar un nombre de contenedor";
                return View();
            }

            await this.service.CreateContainerAsync(containerName);
            ViewData["MENSAJE"] = "Container creado OK";
            return View();
        }

        public async Task<IActionResult> DeleteContainer(string containerName)
        {
            await this.service.DeleteContainerAsync(containerName);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ListBlobs(string containerName)
        {
            List<BlobModel> models = await this.service.GetBlobsAsync(containerName);
            return View(models);
        }

        public async Task<IActionResult> GetBlobFile(string containerName, string blobName)
        {
            BlobDownloadInfo data = await this.service.GetBlobFileAsync(containerName, blobName);
            string contentType = data.ContentType ?? "application/octet-stream";
            return File(data.Content, contentType);
        }

        public async Task<IActionResult> DeleteBlob(string containerName, string blobName)
        {
            await this.service.DeleteBlobAsync(containerName, blobName);
            return RedirectToAction("ListBlobs", new { containerName = containerName });
        }

        public async Task<IActionResult> UploadBlob(string containerName)
        {
            ViewData["CONTAINER"] = containerName;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadBlob(string containerName, IFormFile file)
        {
            {
                string blobName = file.FileName;
                using (Stream stream = file.OpenReadStream())
                {
                    await this.service.UploadBlobAsync(containerName, blobName, stream);
                }
            }

            return RedirectToAction("ListBlobs", new { containerName = containerName });
        }
    }
}
