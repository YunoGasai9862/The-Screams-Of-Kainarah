using Amazon.Polly;
using NUnit.Framework;
using System;
using UnityEngine;


[Serializable]
public class Dialogues
{
    [SerializeField]
    private string _entityName;
    [SerializeField]
    private string _voice;
    [SerializeField]
    private string _audioPath;

    private VoiceId _voiceId;

    public string EntityName { get => _entityName; }

    public string AudioPath { get => _audioPath; set => _audioPath = value; }

    public string Voice { get => _voice; }
    //wont be visible in the UI
    public VoiceId VoiceID { get => _voiceId; set => _voiceId = value; }
    //call always when an entity is added to the scriptble object
    [SerializeField]
    [TextArea(3, 10)]
    private string[] _sentences;

    public string[] Sentences { get => _sentences; }


    public void ParseVoiceId()
    {
        if (Enum.TryParse(typeof(VoiceId), Voice, out object finalVoiceId))
        {
            VoiceID = (VoiceId)finalVoiceId;
        }
        else
        {
            Debug.Log($"Failed Parsing: - defaulting to Emma");
            VoiceID = VoiceId.Emma;
        }
    }
}

