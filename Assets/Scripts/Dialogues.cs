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
    private TextAudioPath[] _textAudioPath;
    private VoiceId _voiceId;

    public string EntityName { get => _entityName; }
    public TextAudioPath[] TextAudioPath { get => _textAudioPath; set => _textAudioPath = value; }
    public string Voice { get => _voice; }
    public VoiceId VoiceID { get => _voiceId; set => _voiceId = value; }

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

