using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class UnityWebRequestMultimediaManager : IUnityWebRequestMultimedia
{
    public async Task<AudioClip> GetAudio(string path, string audioClipName, AudioType audioType)
    {
        try
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

                audioClip.name = audioClipName;

                return audioClip;
            }

        }catch (Exception ex)
        {
            Debug.Log($"Exception {ex}");
        }

        return null;

    }

    public async Task<TextAsset> GetTextAssetFile(string remoteURL)
    {
        //use it for accessing from firebase storage 

        try
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(remoteURL))
            {
                UnityWebRequestAsyncOperation webRequestAsyncOperation = uwr.SendWebRequest();

                while (!webRequestAsyncOperation.isDone)
                {
                    await Task.Yield();
                }

                string textFile = uwr.downloadHandler.text;

                return new TextAsset(textFile);
            }

        }catch(Exception ex){

            Debug.Log($"Exception {ex.ToString()}");
        }

        return null;

    }
}