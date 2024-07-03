using Amazon.Polly;
using System;
using UnityEngine;


[Serializable]
public class Dialogues
{
    public string entityName;

    public string voice;
    //wont be visible in the UI
    public VoiceId voiceId;
    //call always when an entity is added to the scriptble object

    [TextArea(3, 10)]
    public string[] sentences;
    public void ParseVoiceId()
    {
        if (Enum.TryParse(typeof(VoiceId), voice, out object finalVoiceId))
        {
            voiceId = (VoiceId)finalVoiceId;
        }
        else
        {
            Debug.Log($"Failed Parsing: - defaulting to Emma");
            voiceId = VoiceId.Emma;
        }
    }
}
