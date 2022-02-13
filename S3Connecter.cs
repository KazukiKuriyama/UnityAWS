using UnityEngine;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using UnityEngine.UI;
using Amazon.S3.Transfer;
using Newtonsoft.Json;
using System.Threading.Tasks;

public class S3Connecter : MonoBehaviour
{
    enum RegionAPNorth
    {
        TOKYO, // 東京
        SEOUL, // ソウル
        OSAKA // 大阪
    }

    [SerializeField, Header("バケット名")]
    string _bucketName;

    [SerializeField, Header("アクセスキー")]
    string _accessKey;

    [SerializeField, Header("シークレットキー")]
    string _secretKey;

    [SerializeField, Header("リージョン")]
    RegionAPNorth _regionEndpoint;

    [SerializeField, Header("取得するファイル名")]
    string _downloadFileName;

    [SerializeField, Header("ダウンロードパス")]
    string _downloadPass = @"C:\temp\";

    [SerializeField, Header("ログを表示するテキスト")]
    Text _logtext;


    /// <summary>
    /// S3上のテキストを取得する。
    /// </summary>
    public async void DispTextFile()
    {
        AmazonS3Client s3Client = GetAmazonS3Client();
        GetObjectResponse getObjectResponse = await s3Client.GetObjectAsync(_bucketName, _downloadFileName);
        using (StreamReader reader = new StreamReader(getObjectResponse.ResponseStream))
        {
            _logtext.text = await reader.ReadToEndAsync();
        }
    }
    /// <summary>
    /// S3上のJsonファイルを取得し、デシリアライズする。
    /// </summary>
    public async Task<RequestType> JsonDeserialize<RequestType>()
    {
        AmazonS3Client s3Client = GetAmazonS3Client();
        GetObjectResponse getObjectResponse = await s3Client.GetObjectAsync(_bucketName, _downloadFileName);

        RequestType deserializedInstance;
        using (StreamReader reader = new StreamReader(getObjectResponse.ResponseStream))
        {
            // Jsonデシリアライズ
            JsonSerializer serializer = new JsonSerializer();
            deserializedInstance = (RequestType)serializer.Deserialize(reader, typeof(RequestType));
        }
        return deserializedInstance;
    }

    /// <summary>
    /// ファイルをダウンロードし、指定のディレクトリに格納する。
    /// </summary>
    public void DownloadFile()
    {
        AmazonS3Client s3Client = GetAmazonS3Client();
        using (TransferUtility fileTransferUtility = new TransferUtility(s3Client))
        {
            fileTransferUtility.Download(_downloadPass + _downloadFileName, _bucketName, _downloadFileName);
            _logtext.text = "ダウンロードに成功しました";
        }
    }

    /// <summary>
    /// S3クライアントの取得
    /// </summary>
    private AmazonS3Client GetAmazonS3Client()
    {
        return new AmazonS3Client(_accessKey, _secretKey, GetRegionInstance(_regionEndpoint));
    }

    /// <summary>
    /// リージョンインスタンス取得
    /// </summary>
    private Amazon.RegionEndpoint GetRegionInstance(RegionAPNorth region)
    {
        Amazon.RegionEndpoint retRegion = region switch
        {
            RegionAPNorth.TOKYO => Amazon.RegionEndpoint.APNortheast1,
            RegionAPNorth.SEOUL => Amazon.RegionEndpoint.APNortheast2,
            RegionAPNorth.OSAKA => Amazon.RegionEndpoint.APNortheast3,
            _ => null

        };
        return retRegion;
    }
}
