using Azure.Data.Tables;
using CLDV2POE.Models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace CLDV2POE.Services
{
    public class TableService
    {
        //private readonly TableClient _tableClient;
        private readonly IConfiguration _configuration;

        public TableService(IConfiguration configuration)
        {
            //var connectionString = configuration["AzureStorage:ConnectionString"];
            //var serviceClient = new TableServiceClient(connectionString);
            //_tableClient = serviceClient.GetTableClient("CustomerProfiles");
            //_tableClient.CreateIfNotExists();
            _configuration = configuration;
        }

        public async Task InsertCustomerAsync(CustomerProfile profile)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var query = @"INSERT INTO CustomerTable (FirstName, SecondName, Email, PhoneNumber)
                          VALUES (@FirstName, @SecondName, @Email, @PhoneNumber)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FirstName", profile.FirstName);
                command.Parameters.AddWithValue("@SecondName", profile.LastName);
                command.Parameters.AddWithValue("@Email", profile.Email);
                command.Parameters.AddWithValue("@PhoneNumber", profile.PhoneNumber);

                connection.Open();
                await command.ExecuteNonQueryAsync();
            }
        }
        //public async Task AddEntityAsync(CustomerProfile profile)
        //{
        //    await _tableClient.AddEntityAsync(profile);
        //}
    }
}
