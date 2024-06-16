using Firebase.Extensions;
using Firebase.Storage;
using System;
using System.Threading.Tasks;
using UnityEngine;
public class FirebaseStorageManager : MonoBehaviour, IFirebaseStorage
{

    FirebaseStorage FirebaseStorage { get; set; }
    StorageReference MediaStorageReference { get; set; }

    StorageReference StorageReference { get; set; }

    UnityWebRequestMultimediaManager UnityWebRequestMultimediaManager { get; set; }

    string FirebaseStorageLocationURL { get; set; }

    private void Start()
    {
        InitializeFirebaseStorage();

        UnityWebRequestMultimediaManager = new UnityWebRequestMultimediaManager();
    }

    public Task SetMediaReference(string fileName)
    {
        if(string.IsNullOrEmpty(FirebaseStorageLocationURL) || StorageReference == null)
        {
            throw new ApplicationException($"URL is empty, please set Firebase Location first");
        }

        MediaStorageReference = StorageReference.Child(fileName);

        return Task.CompletedTask;
    }

    public StorageReference GetMediaReference()
    {
        return MediaStorageReference;
    }

    public StorageReference GetStorageReference()
    {
        return StorageReference;
    }

    public void InitializeFirebaseStorage()
    {
        FirebaseStorage = FirebaseStorage.DefaultInstance;

        Debug.Log(FirebaseStorage);
    }

    public void SetFirebaseStorageLocation(string url)
    {
        try
        {
            FirebaseStorageLocationURL = url;
            StorageReference = FirebaseStorage.GetReferenceFromUrl(FirebaseStorageLocationURL);

        }catch(Exception  ex)
        {
            Debug.LogException(ex); 
            throw ex;
        }
    }

    public async Task DownloadMedia(FileType fileType, string fileName)
    {
        await SetMediaReference(fileName);

        Debug.Log(MediaStorageReference);

        if (MediaStorageReference == null)
        {
            throw new ApplicationException($"File Reference is null. Check the remote location");
        }

        //once url is here, it continues with the next action
        await MediaStorageReference.GetDownloadUrlAsync().ContinueWithOnMainThread(async downloadResult =>
        {
            //why main thread? Because UnityWebRequest requires us to be on the main thread
            if (!downloadResult.IsFaulted && !downloadResult.IsCanceled)
            {
                await RelayMediaRequest(Convert.ToString(downloadResult.Result), fileType);
            }
        });
    }

    public async Task RelayMediaRequest(string url, FileType fileType)
    {
        switch (fileType)
        {
            case FileType.AUDIO:
                break;
            case FileType.VIDEO:
                break;
            case FileType.IMAGE:
                break;
            case FileType.TEXT:
                Debug.Log(url);
                TextAsset asset = await UnityWebRequestMultimediaManager.GetTextAssetFile(url);
                Debug.Log(asset);
                break;
            default:
                break;
        }

    }
}