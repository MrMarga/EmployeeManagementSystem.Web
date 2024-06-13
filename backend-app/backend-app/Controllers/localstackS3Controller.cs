using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace backend_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalstackS3Controller : ControllerBase
    {
        private readonly IAmazonS3 _s3Client;
        private readonly ILogger<LocalstackS3Controller> _logger;

        public LocalstackS3Controller(IConfiguration configuration, ILogger<LocalstackS3Controller> logger)
        {
            _logger = logger;

            var awsOptions = configuration.GetAWSOptions("localstack");
            var s3Config = new AmazonS3Config
            {
                ServiceURL = configuration["localstack:ServiceURL"],
                ForcePathStyle = true // Necessary for Localstack
            };
            _s3Client = new AmazonS3Client(
                configuration["localstack:AccessKey"],
                configuration["localstack:SecretKey"],
                s3Config);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBucketAsync(string bucketName)
        {
            try
            {
                var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
                if (bucketExists)
                {
                    return BadRequest($"Bucket {bucketName} already exists.");
                }

                var putBucketRequest = new PutBucketRequest
                {
                    BucketName = bucketName
                };

                var response = await _s3Client.PutBucketAsync(putBucketRequest);

                _logger.LogInformation($"Bucket {bucketName} created with status code {response.HttpStatusCode}.");

                return Ok($"Bucket {bucketName} created.");
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(ex, $"Error creating bucket: {ex.Message}");
                return BadRequest($"Error creating bucket: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred: {ex.Message}");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllBucketsAsync()
        {
            try
            {
                var response = await _s3Client.ListBucketsAsync();
                var bucketNames = response.Buckets.Select(b => b.BucketName);
                return Ok(bucketNames);
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(ex, $"Error listing buckets: {ex.Message}");
                return BadRequest($"Error listing buckets: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred: {ex.Message}");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteBucketAsync(string bucketName)
        {
            try
            {
                var deleteBucketRequest = new DeleteBucketRequest
                {
                    BucketName = bucketName
                };

                await _s3Client.DeleteBucketAsync(deleteBucketRequest);
                _logger.LogInformation($"Bucket {bucketName} deleted.");

                return NoContent();
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(ex, $"Error deleting bucket: {ex.Message}");
                return BadRequest($"Error deleting bucket: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred: {ex.Message}");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
