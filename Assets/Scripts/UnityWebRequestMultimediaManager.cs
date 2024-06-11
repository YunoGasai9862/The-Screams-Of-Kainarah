using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class UnityWebRequestMultimediaManager : IUnityWebRequestMultimedia
{
    public async Task<AudioClip> GetAudio(string path, AudioType audioType)
    {
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, audioType))
        {

            UnityWebRequestAsyncOperation webRequestAsyncOperation = uwr.SendWebRequest();

            //await until the file is yield
            while (!webRequestAsyncOperation.isDone)
            {
                await Task.Yield();
            }

            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(uwr);

            return audioClip;
        }
    }

    public Task<TextAsset> GetTextAssetFile(string remoteURL)
    {
        //use it for accessing from firebase storage 
        throw new System.NotImplementedException();
    }
}