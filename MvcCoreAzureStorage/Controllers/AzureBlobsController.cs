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
        public IActionResult Index()
        {
            List<string> containers = this.service.GetContainersAsync().Result;
            return View();
        }

        public async Task<IActionResult> CreateContainer()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateContainer(string containerName)
        {
            await this.service.CreateContainerAsync(containerName);
            return RedirectToAction("Index");
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
