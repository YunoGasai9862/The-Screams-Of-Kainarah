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

    public string EntityName { get => _entityName; }
    public TextAudioPath[] TextAudioPath { get => _textAudioPath; set => _textAudioPath = value; }
    public string Voice { get => _voice; }

    public VoiceId VoiceID { get => ParseVoiceId();}

    public VoiceId ParseVoiceId()
    {
        try
        {
            return VoiceId.FindValue(Voice);

        }catch(Exception ex)
        {
            Debug.Log($"Failed Parsing {ex.Message}: - defaulting to Emma");
        }

        return VoiceId.Emma;
    }
}

