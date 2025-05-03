using System.Threading.Tasks;
using UnityEngine;

public class DialogueAudioManager: MonoBehaviour
{

    [SerializeField]
    AudioSource AudioSource;
    private UnityWebRequestMultimediaManager UnityWebRequestMultimediaManager { get; set; } = new UnityWebRequestMultimediaManager();

    [SerializeField]
    MainThreadDispatcherEvent mainThreadDispatcherEvent;
    public async Task InvokeAIVoice(AWSPollyAudioPacket awsPollyAudioPacket)
    {
        CustomActions customActions = new CustomActions
        {
            Action = action => PlayAudio((AWSPollyAudioPacket)action),
            Parameter = awsPollyAudioPacket

        };

        await mainThreadDispatcherEvent.Invoke(customActions);
    }

    private async void PlayAudio(AWSPollyAudioPacket awsPollyAudioPacket)
    {
        AudioSource.clip = await UnityWebRequestMultimediaManager.GetAudio(awsPollyAudioPacket.AudioPath, awsPollyAudioPacket.AudioName, UnityEngine.AudioType.MPEG);

        AudioSource.Play();
    }
}