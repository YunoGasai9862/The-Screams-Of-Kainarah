using System.Threading.Tasks;
using UnityEngine;

public interface IPreloadAudio<T>
{
    abstract Task PreloadAudio(T value);
}