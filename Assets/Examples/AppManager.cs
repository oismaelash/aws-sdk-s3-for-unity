using Amazon.S3.Model;
using UnityEngine;
using UnityEngine.UI;

namespace AWSSDK.Examples
{
    public class AppManager : MonoBehaviour
    {
        [Header("Infos")]
        [SerializeField] private string S3BucketName;
        [Tooltip("FileName with Extesion. E.G file.txt")] [SerializeField] private string fileNameOnBucket;
        [Tooltip("Path and FileName with Extesion. E.G Documents/file.txt")] [SerializeField] private string pathFileUpload;

        [Header("Buttons")]
        [SerializeField] private Button buttonListFilesBucket;
        [SerializeField] private Button buttonGetFileBucket;
        [SerializeField] private Button buttonUploadFileBucket;
        [SerializeField] private Button buttonDeleteFileBucket;
        [SerializeField] private Text resultTextOperation;

        void Start()
        {
            buttonListFilesBucket.onClick.AddListener(() => { ListObjectsBucket(); });
            buttonGetFileBucket.onClick.AddListener(() => { GetObjectBucket(); });
            buttonUploadFileBucket.onClick.AddListener(() => { UploadObjectForBucket(pathFileUpload, S3BucketName, fileNameOnBucket); });
            buttonDeleteFileBucket.onClick.AddListener(() => { DeleteObjectOnBucket(); });

            S3Manager.Instance.OnResultGetObject += GetObjectBucket;
        }

        private void ListObjectsBucket()
        {
            resultTextOperation.text = "Fetching all the Objects from " + S3BucketName;

            S3Manager.Instance.ListObjectsBucket(S3BucketName, (result, error) =>
            {
                resultTextOperation.text += "\n";
                if (string.IsNullOrEmpty(error))
                {
                    resultTextOperation.text += "Got Response \nPrinting now \n";
                    result.S3Objects.ForEach((o) =>
                    {
                        resultTextOperation.text += string.Format("File: {0}\n", o.Key);
                    });
                }
                else
                {
                    print("Get Error:: " + error);
                    resultTextOperation.text += "Got Exception \n";
                }
            });
        }

        private void GetObjectBucket(GetObjectResponse resultFinal = null, string errorFinal = null)
        {
            resultTextOperation.text = string.Format("fetching {0} from bucket {1}", fileNameOnBucket, S3BucketName);

            if(errorFinal != null)
            {
                resultTextOperation.text += "\n";
                resultTextOperation.text += "Get Data Error";
                print("Get Error:: " + errorFinal);
                return;
            }

            S3Manager.Instance.GetObjectBucket(S3BucketName, fileNameOnBucket, (result, error) =>
            {
                if (string.IsNullOrEmpty(error))
                {
                    resultTextOperation.text += "\nGet Data Complete.";
                }
                else
                {
                    resultTextOperation.text += "\n";
                    resultTextOperation.text += "Get Data Error";
                    print("Get Error:: " + error);
                }
            });

        }

        private void UploadObjectForBucket(string pathFile, string S3BucketName, string fileNameOnBucket)
        {
            resultTextOperation.text = "Retrieving the file";
            resultTextOperation.text += "\nCreating request object";
            resultTextOperation.text += "\nMaking HTTP post call";

            S3Manager.Instance.UploadObjectForBucket(pathFile, S3BucketName, fileNameOnBucket, (result, error) =>
            {
                if(string.IsNullOrEmpty(error))
                {
                    resultTextOperation.text += "\nUpload Success";
                }
                else
                {
                    resultTextOperation.text += "\nUpload Failed";
                    Debug.LogError("Get Error:: " + error);
                }
            });
        }

        private void DeleteObjectOnBucket()
        {
            resultTextOperation.text = string.Format("deleting {0} from bucket {1}\n", fileNameOnBucket, S3BucketName);

            S3Manager.Instance.DeleteObjectOnBucket(fileNameOnBucket, S3BucketName, (result, error) =>
            {
                if (string.IsNullOrEmpty(error))
                {
                    resultTextOperation.text += "\nFile Deleted Success";
                }
                else
                {
                    resultTextOperation.text += "\nFile Deleted Failed";
                    print("Get Error:: " + error);
                }
            });
        }
    }
}