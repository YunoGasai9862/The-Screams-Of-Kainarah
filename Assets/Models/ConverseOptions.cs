using Amazon.Polly;
using System;

[Serializable]
public class ConverseOptions
{
    public bool shouldConverse;
    public VoiceId voiceId;
}