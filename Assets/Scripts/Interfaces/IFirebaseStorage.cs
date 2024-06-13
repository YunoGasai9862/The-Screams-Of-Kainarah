using Firebase.Storage;

public interface IFirebaseStorage
{
    abstract void InitializeFirebaseStorage();

    abstract StorageReference GetReference();

    abstract void SetFirebaseStorageLocation(string url);

    abstract StorageReference SelectMedia(StorageReference storageReference, FileType fileType, string fileName);

    abstract void DownloadMedia(StorageReference mediaReference, FileType fileType, string fileName);
}