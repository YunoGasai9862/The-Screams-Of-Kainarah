using System;
using Amazon.Polly;
using UnityEngine;
[Serializable]
public class DialogueOptions
{
    public bool dialogueConcluded;
    public bool multipleDialogues;
    public string voice;
    //wont be visible in the UI
    public VoiceId voiceId;
    //call always when an entity is added to the scriptble object
    public void ParseVoiceId()
    {
         if(Enum.TryParse(typeof(VoiceId), voice, out object finalVoiceId))
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