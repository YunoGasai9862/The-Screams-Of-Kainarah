using Amazon.Polly;
using System;

[Serializable]
public class AWSPollyAudioPacket
{
    public string AudioName { get; set; }
    public string DialogueText { get; set; }
    public VoiceId AudioVoiceId { get; set; }
}