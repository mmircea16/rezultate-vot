using System;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using CSharpFunctionalExtensions;
using ElectionResults.Core.Models;
using Microsoft.Extensions.Logging;

namespace ElectionResults.Core.Repositories
{
    public class BucketRepository : IBucketRepository
    {
        private readonly IAmazonS3 _s3Client;
        private readonly ILogger _logger;

        public BucketRepository(IAmazonS3 s3Client, ILogger<BucketRepository> logger)
        {
            _s3Client = s3Client;
            _logger = logger;
        }

        public virtual async Task<bool> DoesS3BucketExist(string bucketName)
        {
            return await _s3Client.DoesS3BucketExistAsync(bucketName);
        }

        public async Task<Result<CreateBucketResponse>> CreateBucket(string bucketName)
        {
            try
            {
                var putBucketRequest = new PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true,
                    CannedACL = S3CannedACL.NoACL
                };
                var response = await _s3Client.PutBucketAsync(putBucketRequest);

                return Result.Ok(new CreateBucketResponse
                {
                    BucketName = bucketName,
                    RequestId = response.ResponseMetadata.RequestId
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to upload");
                throw;
            }
        }
    }
}
