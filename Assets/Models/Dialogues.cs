using Amazon.Polly;
using System;
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

    public List<INotify> DialogueSubscriberEntities
    {
        get 
        {
            if (DialogueSubscriberEntities.Count == 0)
            {
                DialogueSubscriberEntities = PrefillINotifyForDialogueSubscriberEntities();
            }

            return DialogueSubscriberEntities;
        }

        set
        {
            DialogueSubscriberEntities = value;
        }
    }

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

    public List<INotify> PrefillINotifyForDialogueSubscriberEntities()
    {
        List<INotify> entities = new List<INotify>();

        foreach(DialogueSubscriberEntity subscriberEntity in _dialogueSubscriberEntities)
        {
           INotify notify = subscriberEntity.Entity.GetComponent<INotify>();

           if (notify == null)
            {
                continue;
            }

            entities.Add(notify);
        }

        return entities;
    }
}

