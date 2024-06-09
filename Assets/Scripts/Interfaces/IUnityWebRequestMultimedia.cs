using System.Threading.Tasks;
using UnityEngine;
public interface IUnityWebRequestMultimedia
{
    abstract Task<AudioClip> GetAudio(string path, AudioType audioType);
}