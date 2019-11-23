using System;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using CSharpFunctionalExtensions;
using ElectionResults.Core.Infrastructure;
using ElectionResults.Core.Models;

namespace ElectionResults.Core.Repositories
{
    public class BucketRepository : IBucketRepository
    {
        private readonly IAmazonS3 _s3Client;

        public BucketRepository(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
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
                Log.LogError(e, "Failed to upload");
                throw;
            }
        }
    }
}
