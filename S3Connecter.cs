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
        TOKYO, // ����
        SEOUL, // �\�E��
        OSAKA // ���
    }

    [SerializeField, Header("�o�P�b�g��")]
    string _bucketName;

    [SerializeField, Header("�A�N�Z�X�L�[")]
    string _accessKey;

    [SerializeField, Header("�V�[�N���b�g�L�[")]
    string _secretKey;

    [SerializeField, Header("���[�W����")]
    RegionAPNorth _regionEndpoint;

    [SerializeField, Header("�擾����t�@�C����")]
    string _downloadFileName;

    [SerializeField, Header("�_�E�����[�h�p�X")]
    string _downloadPass = @"C:\temp\";

    [SerializeField, Header("���O��\������e�L�X�g")]
    Text _logtext;


    /// <summary>
    /// S3��̃e�L�X�g���擾����B
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
    /// S3���Json�t�@�C�����擾���A�f�V���A���C�Y����B
    /// </summary>
    public async Task<RequestType> JsonDeserialize<RequestType>()
    {
        AmazonS3Client s3Client = GetAmazonS3Client();
        GetObjectResponse getObjectResponse = await s3Client.GetObjectAsync(_bucketName, _downloadFileName);

        RequestType deserializedInstance;
        using (StreamReader reader = new StreamReader(getObjectResponse.ResponseStream))
        {
            // Json�f�V���A���C�Y
            JsonSerializer serializer = new JsonSerializer();
            deserializedInstance = (RequestType)serializer.Deserialize(reader, typeof(RequestType));
        }
        return deserializedInstance;
    }

    /// <summary>
    /// �t�@�C�����_�E�����[�h���A�w��̃f�B���N�g���Ɋi�[����B
    /// </summary>
    public void DownloadFile()
    {
        AmazonS3Client s3Client = GetAmazonS3Client();
        using (TransferUtility fileTransferUtility = new TransferUtility(s3Client))
        {
            fileTransferUtility.Download(_downloadPass + _downloadFileName, _bucketName, _downloadFileName);
            _logtext.text = "�_�E�����[�h�ɐ������܂���";
        }
    }

    /// <summary>
    /// S3�N���C�A���g�̎擾
    /// </summary>
    private AmazonS3Client GetAmazonS3Client()
    {
        return new AmazonS3Client(_accessKey, _secretKey, GetRegionInstance(_regionEndpoint));
    }

    /// <summary>
    /// ���[�W�����C���X�^���X�擾
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
