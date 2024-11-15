﻿using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;


namespace CLDV2POE.Services
{

    //public class BlobService
    //{
    //    private readonly HttpClient _httpClient;
    //    private readonly string _uploadBlobUrl;

    //    public BlobService(HttpClient httpClient, IConfiguration configuration)
    //    {
    //        _httpClient = httpClient;
    //        _uploadBlobUrl = configuration["AzureFunctionApp:UploadBlobUrl"]; // URL for the function app
    //    }

    //    public async Task UploadBlobAsync(string containerName, string blobName, Stream content)
    //    {
    //        var requestContent = new MultipartFormDataContent();
    //        requestContent.Add(new StreamContent(content), "file", blobName);

    //        var response = await _httpClient.PostAsync($"{_uploadBlobUrl}?container={containerName}&blob={blobName}", requestContent);
    //        response.EnsureSuccessStatusCode();
    //    }
    //}


    //public class BlobService
    //{
    //    private readonly BlobServiceClient _blobServiceClient;

    //    public BlobService(IConfiguration configuration)
    //    {
    //        _blobServiceClient = new BlobServiceClient(configuration["AzureStorage:ConnectionString"]);
    //    }

    //    public async Task UploadBlobAsync(string containerName, string blobName, Stream content)
    //    {
    //        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
    //        await containerClient.CreateIfNotExistsAsync();
    //        var blobClient = containerClient.GetBlobClient(blobName);
    //        await blobClient.UploadAsync(content, true);
    //    }
    //}

    public class BlobService
    {
        private readonly IConfiguration _configuration;

        public BlobService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task InsertBlobAsync(byte[] imageData)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var query = @"INSERT INTO BlobTable (BlobImage) VALUES (@BlobImage)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@BlobImage", imageData);

                connection.Open();
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}

