using Firebase.Extensions;
using Firebase.Storage;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseStorageManager : IFirebaseStorage
{

    FirebaseStorage FirebaseStorage { get; set; }
    StorageReference MediaStorageReference { get; set; }

    UnityWebRequestMultimediaManager UnityWebRequestMultimediaManager { get; set; }

    string FirebaseStorageLocationURL { get; set; }
    
    FirebaseStorageManager()
    {
        InitializeFirebaseStorage();

        UnityWebRequestMultimediaManager = new UnityWebRequestMultimediaManager();
    }

    public Task<StorageReference> GetMediaReference(StorageReference storageReference, string fileName)
    {
        if(string.IsNullOrEmpty(FirebaseStorageLocationURL))
        {
            throw new ApplicationException($"URL is empty, please set Firebase Location first");
        }

        return Task.FromResult(storageReference.Child(fileName));
    }

    public StorageReference GetReference()
    {
        return MediaStorageReference;
    }

    public void InitializeFirebaseStorage()
    {
        FirebaseStorage = FirebaseStorage.DefaultInstance;
    }

    public void SetFirebaseStorageLocation(string url)
    {
        try
        {
            FirebaseStorageLocationURL = url;
            MediaStorageReference = FirebaseStorage.GetReferenceFromUrl(FirebaseStorageLocationURL);

        }catch(Exception  ex)
        {
            Debug.LogException(ex); 
            throw ex;
        }
    }

    public async Task DownloadMedia(StorageReference mediaReference, FileType fileType, string fileName)
    {
        StorageReference fileReference = await GetMediaReference(mediaReference, fileName);

        if(fileReference == null)
        {
            throw new ApplicationException($"File Reference is null. Check the remote location");
        }

        //once url is here, it continues with the next action
        await fileReference.GetDownloadUrlAsync().ContinueWith(async task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                //task.result contains the URL
                Debug.Log(task.Result.ToString());

                TextAsset textAsset = await UnityWebRequestMultimediaManager.GetTextAssetFile(task.Result.ToString());

            }
        });

    }

    public Task SelectFileType(Uri url, FileType fileType)
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
                return GetMedia<TextAsset>(url);
            default:
                break;
        }

        return null;
    }

    public Task<T> GetMedia<T>(Uri url)
    {
        return null;
    }  



}