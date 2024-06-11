using System.Threading.Tasks;
using UnityEngine;
public interface IUnityWebRequestMultimedia
{
    abstract Task<AudioClip> GetAudio(string path, AudioType audioType);

    abstract Task<TextAsset> GetTextAssetFile(string remoteURL);
}