using Firebase.Storage;
using System;
using UnityEngine;

public class FirebaseStorageManager : IFirebaseStorage
{

    FirebaseStorage FirebaseStorage { get; set; }
    StorageReference StorageReference { get; set; }

    string FirebaseStorageLocationURL { get; set; }

    FirebaseStorageManager()
    {
       InitializeFirebaseStorage();
    }

    public StorageReference SelectMedia(StorageReference storageReference, FileType fileType, string fileName)
    {
        if(string.IsNullOrEmpty(FirebaseStorageLocationURL))
        {
            throw new ApplicationException($"URL is empty, please set Firebase Location first");
        }

        StorageReference mediaReference;

        try
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
                    mediaReference = storageReference.Child(fileName);
                break;
                default:
                    break;
            }
        }catch(Exception ex)
        {
            Debug.LogException(ex);
            throw ex;
        }

        return null;
    }

    public StorageReference GetReference()
    {
        return StorageReference;
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
            StorageReference = FirebaseStorage.GetReferenceFromUrl(FirebaseStorageLocationURL);

        }catch(Exception  ex)
        {
            Debug.LogException(ex); 
            throw ex;
        }
    }

    public void DownloadMedia(StorageReference mediaReference, FileType fileType, string fileName)
    {
        throw new NotImplementedException();
    }

}