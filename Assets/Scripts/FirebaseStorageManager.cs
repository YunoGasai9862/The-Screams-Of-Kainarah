using Firebase.Storage;

public class FirebaseStorageManager : IFirebaseStorage
{
    FirebaseStorage FirebaseStorage { get; set; }
    StorageReference StorageReference { get; set; }

    FirebaseStorageManager()
    {
       InitializeFirebaseStorage();
    }

    public void DownloadMedia(StorageReference fileReference)
    {
        throw new System.NotImplementedException();
    }

    public StorageReference GetReference(string url)
    {
        throw new System.NotImplementedException();
    }

    public void InitializeFirebaseStorage()
    {
        FirebaseStorage = FirebaseStorage.DefaultInstance;
    }
}