using Amazon.Polly;
using System;

[Serializable]
public class AWSPollyAudioPacket
{
    public string AudioPath { get; set; }
    public string AudioName { get; set; }
    public string DialogueText { get; set; }
    public VoiceId AudioVoiceId { get; set; }

    public OutputFormat OutputFormat { get; set; }

    public override string ToString()
    {
        return $"AudioPath: {AudioPath}, AudioName: {AudioName}, DialogueText: {DialogueText}, VoiceId: {AudioVoiceId} OutputFormat: {OutputFormat}";
    }

}