using CLDV2POE.Models;
using CLDV2POE.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;


namespace CLDV2POE.Controllers
{
    public class HomeController : Controller
    {
        //private readonly BlobService _blobService;
        //private readonly TableService _tableService;
        //private readonly QueueService _queueService;
        //private readonly FileService _fileService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly TableService _customerService; // Inject CustomerService
        private readonly BlobService _blobService; // Inject BlobService

        //public HomeController(BlobService blobService, TableService tableService, QueueService queueService, FileService fileService)
        //{
        //    _blobService = blobService;
        //    _tableService = tableService;
        //    _queueService = queueService;
        //    _fileService = fileService;
        //}

        public HomeController(IHttpClientFactory httpClientFactory, ILogger<HomeController> logger, IConfiguration configuration, TableService customerService, BlobService blobService)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;
            _customerService = customerService;
            _blobService = blobService;
        }
        public IActionResult Index()
        {
            //return View();
            var model = new CustomerProfile();
            return View(model);
        }

        
            [HttpGet]
            public IActionResult UploadImage()
            {
                return View();
            }
        

        [HttpPost]
        public async Task<IActionResult> UploadBlob(IFormFile imageFile)
        {
            if (imageFile != null)
            {
                try
                {
                    // Call Azure function to upload the blob
                    using var httpClient = _httpClientFactory.CreateClient();
                    using var stream = imageFile.OpenReadStream();
                    var content = new StreamContent(stream);
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imageFile.ContentType);

                    var baseUrl = _configuration["AzureFunctions:UploadBlob"];
                    string url = $"{baseUrl}&blobName={imageFile.FileName}";
                    var response = await httpClient.PostAsync(url, content); if (response.IsSuccessStatusCode)
                    {
                        // Convert image to byte array for SQL insertion
                        using (var memoryStream = new MemoryStream())
                        {
                            await imageFile.CopyToAsync(memoryStream);
                            var imageData = memoryStream.ToArray();

                            // Insert image data into SQL BlobTable
                            await _blobService.InsertBlobAsync(imageData);
                        }

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        _logger.LogError($"Error submitting image: {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Exception occurred while submitting image: {ex.Message}");
                }
            }
            else
            {
                _logger.LogError("No image file provided.");
            }

            return View("Index");
        }
        //{
        //    if (file != null)
        //    {
        //        using var stream = file.OpenReadStream();
        //        await _blobService.UploadBlobAsync("product-images", file.FileName, stream);
        //    }
        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        public async Task<IActionResult> StoreTableInfo(CustomerProfile profile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Call Azure function to store data in Azure Table
                    using var httpClient = _httpClientFactory.CreateClient();
                    var baseUrl = _configuration["AzureFunctions:StoreTableInfo"];
                    var requestUri = $"{baseUrl}&tableName=CustomerProfiles&partitionKey={profile.PartitionKey}&rowKey={profile.RowKey}&firstName={profile.FirstName}&lastName={profile.LastName}&phoneNumber={profile.PhoneNumber}&Email={profile.Email}";

                    var response = await httpClient.PostAsync(requestUri, null);

                    if (response.IsSuccessStatusCode)
                    {
                        // Insert customer data into SQL database
                        await _customerService.InsertCustomerAsync(profile);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        _logger.LogError($"Error submitting client info: {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Exception occurred while submitting client info: {ex.Message}");
                }
            }

            return View("Index", profile);
        }

        //{
        //    if (ModelState.IsValid)
        //    {
        //        await _tableService.AddEntityAsync(profile);
        //    }
        //    return RedirectToAction("Index");
        //}

        //[HttpPost]
        //public async Task<IActionResult> ProcessOrder(string orderId)
        //{
        //    await _queueService.SendMessageAsync("order-processing", $"Processing order {orderId}");
        //    return RedirectToAction("Index");
        //}

        //[HttpPost]
        //public async Task<IActionResult> UploadContract(IFormFile file)
        //{
        //    if (file != null)
        //    {
        //        using var stream = file.OpenReadStream();
        //        await _fileService.UploadFileAsync("contracts-logs", file.FileName, stream);
        //    }
        //    return RedirectToAction("Index");
        //}
    }
}
