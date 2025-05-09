using System.Threading.Tasks;
using UnityEngine;

public class AudioManager: MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    AudioTriggerEvent audioTriggerEvent;
    [SerializeField]
    MainThreadDispatcherEvent mainThreadDispatcherEvent;

    private UnityWebRequestMultimediaManager UnityWebRequestMultimediaManager { get; set; } = new UnityWebRequestMultimediaManager();

    private void Start()
    {
        audioTriggerEvent.AddListener(InvokeAudio);
    }

    public async void InvokeAudio(AudioPackage audioPackage)
    {
        CustomActions customActions = new CustomActions
        {
            Action = action => PlayAudio((AudioPackage)action),

            Parameter = audioPackage

        };

        await mainThreadDispatcherEvent.Invoke(customActions);
    }

    private async void PlayAudio(AudioPackage audioPackage)
    {
        audioSource.clip = await UnityWebRequestMultimediaManager.GetAudio(audioPackage.AudioPath, audioPackage.AudioName, audioPackage.AudioType);

        audioSource.Play();
    }
}