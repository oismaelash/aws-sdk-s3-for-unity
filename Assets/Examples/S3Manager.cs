//
// Copyright 2014-2015 Amazon.com, 
// Inc. or its affiliates. All Rights Reserved.
// 
// Licensed under the AWS Mobile SDK For Unity 
// Sample Application License Agreement (the "License"). 
// You may not use this file except in compliance with the 
// License. A copy of the License is located 
// in the "license" file accompanying this file. This file is 
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, express or implied. See the License 
// for the specific language governing permissions and 
// limitations under the License.
//

using UnityEngine;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using System.IO;
using System.Collections.Generic;
using Amazon.CognitoIdentity;
using Amazon;
using System;

namespace AWSSDK.Examples
{
    public class S3Manager : MonoBehaviour
    {
        #region VARIABLES

        public static S3Manager Instance { get; set; }

        [Header("AWS Setup")]
        [SerializeField] private string identityPoolId;
        [SerializeField] private string cognitoIdentityRegion = RegionEndpoint.USEast1.SystemName;
        [SerializeField] private string s3Region = RegionEndpoint.USEast1.SystemName;

        // Variables privates
        private int timeoutGetObject = 5; // seconds
        private string resultTimeout = "";
        public Action<GetObjectResponse, string> OnResultGetObject;
        private IAmazonS3 s3Client;
        private AWSCredentials credentials;

        // Propertys
        private RegionEndpoint CognitoIdentityRegion
        {
            get { return RegionEndpoint.GetBySystemName(cognitoIdentityRegion); }
        }
        private RegionEndpoint S3Region
        {
            get { return RegionEndpoint.GetBySystemName(s3Region); }
        }
        private AWSCredentials Credentials
        {
            get
            {
                if (credentials == null)
                    credentials = new CognitoAWSCredentials(identityPoolId, CognitoIdentityRegion);
                return credentials;
            }
        }
        private IAmazonS3 Client
        {
            get
            {
                if (s3Client == null)
                {
                    s3Client = new AmazonS3Client(Credentials, S3Region);
                }
                //test comment
                return s3Client;
            }
        }

        #endregion

        #region METHODS MONOBEHAVIOUR

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            UnityInitializer.AttachToGameObject(this.gameObject);
            AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
        }


        #endregion

        #region METHODS AWS SDK S3

        /// <summary>
        /// Get Objects from S3 Bucket
        /// </summary>
        public void ListObjectsBucket(string nameBucket, Action<ListObjectsResponse, string> result)
        {
            var request = new ListObjectsRequest()
            {
                BucketName = nameBucket
            };

            Client.ListObjectsAsync(request, (responseObject) =>
            {
                if (responseObject.Exception == null)
                    result?.Invoke(responseObject.Response, "");
                else
                    result?.Invoke(null, responseObject.Exception.ToString());
            });
        }

        /// <summary>
        /// Get Object from S3 Bucket
        /// </summary>
        public void GetObjectBucket(string S3BucketName, string fileNameOnBucket, Action<GetObjectResponse, string> result)
        {
            resultTimeout = "";
            Invoke("ResultTimeoutGetObjectBucket", timeoutGetObject);

            var request = new GetObjectRequest
            {
                BucketName = S3BucketName,
                Key = fileNameOnBucket
            };

            Client.GetObjectAsync(request, (responseObj) =>
            {
                var response = responseObj.Response;

                if (response.ResponseStream != null)
                {
                    result?.Invoke(responseObj.Response, "");
                    resultTimeout = "success";
                }
                else
                    result?.Invoke(null, responseObj.Exception.ToString());
            });
        }

        /// <summary>
        /// Post Object to S3 Bucket. 
        /// </summary>
        public void UploadObjectForBucket(string pathFile, string S3BucketName, string fileNameOnBucket, Action<PostObjectResponse, string> result)
        {
            if (!File.Exists(pathFile))
            {
                result?.Invoke(null, "FileNotFoundException: Could not find file " + pathFile);
                return;
            }

            var stream = new FileStream(pathFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            var request = new PostObjectRequest()
            {
                Bucket = S3BucketName,
                Key = fileNameOnBucket,
                InputStream = stream,
                CannedACL = S3CannedACL.Private,
                Region = S3Region
            };

            Client.PostObjectAsync(request, (responseObj) =>
            {
                if (responseObj.Exception == null)
                    result?.Invoke(responseObj.Response, "");
                else
                    result?.Invoke(null, responseObj.Exception.ToString());
            });
        }

        /// <summary>
        /// Delete Objects in S3 Bucket
        /// </summary>
        public void DeleteObjectOnBucket(string fileNameOnBucket, string S3BucketName, Action<DeleteObjectsResponse, string> result)
        {
            List<KeyVersion> objects = new List<KeyVersion>();
            objects.Add(new KeyVersion()
            {
                Key = fileNameOnBucket
            });

            var request = new DeleteObjectsRequest()
            {
                BucketName = S3BucketName,
                Objects = objects
            };

            Client.DeleteObjectsAsync(request, (responseObj) =>
            {
                if (responseObj.Exception == null)
                    result?.Invoke(responseObj.Response, "");
                else
                    result?.Invoke(null, responseObj.Exception.ToString());
            });
        }

        #endregion 

        #region METHODS UTILS

        private void ResultTimeoutGetObjectBucket()
        {
            if(string.IsNullOrEmpty(resultTimeout))
            {
                OnResultGetObject?.Invoke(null, "Timeout GetObject");
            }
        }

        #endregion
    }
}