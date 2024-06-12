using Firebase.Storage;

public interface IFirebaseStorage
{
    abstract void InitializeFirebaseStorage();

    abstract StorageReference GetReference(string url);

    abstract void DownloadMedia(StorageReference fileReference);
}