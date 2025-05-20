using Amazon.Polly;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField]
    private List<DialogueSubscriberEntity> _dialogueSubscriberEntities;

    public string EntityName { get => _entityName; }
    public TextAudioPath[] TextAudioPath { get => _textAudioPath; set => _textAudioPath = value; }
    public string Voice { get => _voice; }

    //need to do something better here so it doesn't get called again and again - check length maybe
    public List<INotify> DialogueSubscriberEntities { get => DialogueSubscriberEntities; set => GetEntitiesToBeNotified(); }

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

    public List<INotify> GetEntitiesToBeNotified()
    {
        if (DialogueSubscriberEntities.Count > 0)
        {
            return DialogueSubscriberEntities;
        }

        List<INotify> entities = new List<INotify>();

        foreach(DialogueSubscriberEntity subscriberEntity in _dialogueSubscriberEntities)
        {
            //find the component and assign
        }

        return entities;
    }
}

