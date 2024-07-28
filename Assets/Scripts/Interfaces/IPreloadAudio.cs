using System.Collections;
public interface IPreloadAudio<T>
{
    abstract IEnumerator PreloadAudio(T value);
}