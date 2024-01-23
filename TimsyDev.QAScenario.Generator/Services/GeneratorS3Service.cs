using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TimsyDev.QAScenario.Generator.Services
{
    public interface IGeneratorS3Service
    {
        Task<bool> TestDownloadObjectFromBucketAsync();

        Task<bool> DownloadObjectFromBucketAsync(string bucketName, string objectName, string filePath);
        Task<string> ReturnS3FileEncoded(string bucketName, string objectName);
        Task<StreamContent> ReturnS3FileMS(string bucketName, string objectName);
    }

    public class GeneratorS3Service: IGeneratorS3Service
    {
        private readonly IAmazonS3 _client;
        public GeneratorS3Service(IAmazonS3 client)
        {
            _client = client;
        }


        public async Task<bool> TestDownloadObjectFromBucketAsync()
        {
            var bucketName = "qa-scenario.generator-files-by-extension";
            var objectName = "PDF.pdf";
            var filePath = Directory.GetCurrentDirectory();
            return await DownloadObjectFromBucketAsync(bucketName, objectName, filePath);
        }

        public async Task<bool> DownloadObjectFromBucketAsync(
            string bucketName,
            string objectName,
            string filePath)
        {
            // Create a GetObject request
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = objectName,
            };

            // Issue request and remember to dispose of the response
            using GetObjectResponse response = await _client.GetObjectAsync(request);

            try
            {
                // Save object to local file
                Console.WriteLine($"File Output: {filePath}\\{objectName}");
                await response.WriteResponseStreamToFileAsync($"{filePath}\\{objectName}", true, CancellationToken.None);
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error saving {objectName}: {ex.Message}");
                return false;
            }
        }

        public async Task<string> ReturnS3FileEncoded(string bucketName, string objectName)
        {
            // Create a GetObject request
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = objectName,
            };

            // Issue request and remember to dispose of the response
            using GetObjectResponse response = await _client.GetObjectAsync(request);
            using var ms = new MemoryStream();
            try
            {
                await response.ResponseStream.CopyToAsync(ms);
                var encoded = Convert.ToBase64String(ms.ToArray());
                //await response.WriteResponseStreamToFileAsync($"{filePath}\\{objectName}", true, CancellationToken.None);
                return encoded;
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error getting {objectName}: {ex.Message}");
                throw;
            }
        }

        public async Task<StreamContent> ReturnS3FileMS(string bucketName, string objectName)
        {
            // Create a GetObject request
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = objectName,
            };

            // Issue request and remember to dispose of the response
            using GetObjectResponse response = await _client.GetObjectAsync(request);
            using var ms = new MemoryStream();
            try
            {
                await response.ResponseStream.CopyToAsync(ms);
                var buffer = ms.ToArray();
                return new StreamContent(new MemoryStream(buffer));

            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error getting {objectName}: {ex.Message}");
                throw;
            }
        }
    }
}
