using Firebase.Extensions;
using Firebase.Storage;
using System;
using System.Threading.Tasks;
using UnityEngine;
public class FirebaseStorageManager : MonoBehaviour, IFirebaseStorage, ISubject<IObserver<FirebaseStorageManager>>
{

    [SerializeField]
    public FirebaseStorageManagerDelegator firebaseStorageManagerDelegator;

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

    public async Task<T> DownloadMedia<T>(FileType fileType, string fileName)
    {
        TaskCompletionSource<T> downloadMediaTCS = new TaskCompletionSource<T>();

        await SetMediaReference(fileName);

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
               T mediaDownloaded =  await RelayMediaRequest<T>(Convert.ToString(downloadResult.Result), fileType);

               downloadMediaTCS.SetResult((T)mediaDownloaded);
            }
        });

        return await downloadMediaTCS.Task;
    }

    public async Task<T> RelayMediaRequest<T>(string url, FileType fileType)
    {
        object media;
        TaskCompletionSource<T> mediaRelayerTCS = new TaskCompletionSource<T>();
        switch (fileType)
        {
            case FileType.AUDIO:
                break;
            case FileType.VIDEO:
                break;
            case FileType.IMAGE:
                break;
            case FileType.TEXT:
                media = await UnityWebRequestMultimediaManager.GetTextAssetFile(url);
                mediaRelayerTCS.SetResult((T)media);
                break;
            default:
                break;
        }

        return await mediaRelayerTCS.Task;
    }

    void ISubject<IObserver<FirebaseStorageManager>>.OnNotifySubject(IObserver<FirebaseStorageManager> data, params object[] optional)
    {
        firebaseStorageManagerDelegator.NotifyObserver(data, this);
    }
}