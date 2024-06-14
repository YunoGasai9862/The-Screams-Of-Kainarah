using Firebase.Storage;
using System.Threading.Tasks;

public interface IFirebaseStorage
{
    abstract void InitializeFirebaseStorage();

    abstract StorageReference GetReference();

    abstract void SetFirebaseStorageLocation(string url);

    abstract Task<StorageReference> GetMediaReference(StorageReference storageReference, string fileName);

    abstract Task DownloadMedia(StorageReference mediaReference, FileType fileType, string fileName);
}