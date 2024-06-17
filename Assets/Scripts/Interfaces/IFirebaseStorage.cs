using Firebase.Storage;
using System;
using System.Threading.Tasks;

public interface IFirebaseStorage
{
    abstract void InitializeFirebaseStorage();

    abstract StorageReference GetStorageReference();

    abstract StorageReference GetMediaReference();

    abstract void SetFirebaseStorageLocation(string url);

    abstract Task<T> DownloadMedia<T>(FileType fileType, string fileName);

}