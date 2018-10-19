using System;
using System.Collections;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Test
{
    public class Tests : MonoBehaviour
    {
        [Header("For Get Image")]
        [SerializeField] private RawImage rawImageShowImage;
        [SerializeField] private InputField inputFieldPathImage;

        [Header("For Get Video")]
        [SerializeField] private RawImage rawImageShowVideo;
        [SerializeField] private InputField inputFieldPathVideo;
        [SerializeField] private VideoPlayer videoPlayer;

        [Header("For Upload and Download")]
        [SerializeField] private string pathInput = "/Users/ismaelnascimento/Downloads/book.mp4";
        [SerializeField] private string pathOutput = "/Users/ismaelnascimento/Documents/workspace/My Hero Academia - Official Trailer 1.mp4";
        [SerializeField] private string urlInput = "https://s3.amazonaws.com/tests-shopperum/My Hero Academia - Official Trailer 1.mp4";
        private string byteTemp;
        private string bytesTemp;

        public void OnViewImageSelected()
        {
            Texture2D textureImage = new Texture2D(2, 2);
            textureImage.LoadImage(File.ReadAllBytes(inputFieldPathImage.text));
            rawImageShowImage.texture = textureImage;
        }

        public void OnViewVideoSelected()
        {
            videoPlayer.url = inputFieldPathVideo.text;
            videoPlayer.Play();
        }

        [ContextMenu("TestGetBytesVideo")]
        private void TestGetBytesVideo()
        {
            StartCoroutine(TestGetBytesVideo_Coroutine());
        }

        private IEnumerator TestGetBytesVideo_Coroutine()
        {
            using (UnityWebRequest request = UnityWebRequest.Get("https://s3.amazonaws.com/tests-shopperum/My+Hero+Academia+-+Official+Trailer+1.mp4"))
            {
                print("Start download...");
                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.Log(request.error);
                }
                else
                {
                    // Show results as text
                    Debug.Log("text:: " + request.downloadHandler.text);
                    Debug.Log("data:: " + request.downloadHandler.data);

                    using (StreamWriter sw = new StreamWriter("/Users/ismaelnascimento/Documents/workspace/test1.mp4"))
                    {
                        sw.WriteLine(request.downloadHandler.text);
                    }

                    using (StreamWriter sw = new StreamWriter("/Users/ismaelnascimento/Documents/workspace/test2.mp4"))
                    {
                        sw.WriteLine(request.downloadHandler.data);
                    }

                    print("Finish download...");
                }
            }



        }
    
        [ContextMenu("GetBytes")]
        private void FileToBytes()
        {
            Byte[] bytes = File.ReadAllBytes(pathInput);
            String file = Convert.ToBase64String(bytes);
            byteTemp = file;

            var howManyBytes = byteTemp.Length * sizeof(Char);
            print("size variable byteTemp:: " + howManyBytes);
            print("file bytes size::" + file.Length);
            print("file bytes::" + file);
        }

        [ContextMenu("DownloadBytes")]
        private void BytesToFile()
        {
            Byte[] bytes = Convert.FromBase64String(byteTemp);
            File.WriteAllBytes(pathOutput, bytes);

            print("download bytes::");
        }

        // NameTemp
        // NameOriginal
        // StringBase64
        // FormartFile

        [ContextMenu("GetDownloadUrlBytes")]
        private void UrlToFile()
        {
            print("Start download file");
            // Get Bytes file ( url )
            var webClient = new WebClient();
            byte[] imageBytes = webClient.DownloadData(urlInput);

            // Convert bytes to file
            File.WriteAllBytes(pathOutput, imageBytes);
            print("Convert Url to File");
        }

        // App -> Api Gateway -> Lambda -> S3 = UploadFile
        // S3(All Files, path + name) -> Sdk AWS Mobile(Get name files on bucket) -> App = DownloadFile


    }
}