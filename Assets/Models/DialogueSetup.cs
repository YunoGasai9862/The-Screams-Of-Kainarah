using Amazon.Polly;
using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class DialogueSetup
{
    [SerializeField]
    private string _entityName;
    [SerializeField]
    private string _voice;
    [SerializeField]
    private Dialogue[] _dialogues;
    [SerializeField]
    private List<DialogueSubscriberEntity> _dialogueSubscriberEntities;

    public string EntityName { get => _entityName; }
    public Dialogue[] Dialogues { get => _dialogues; set => _dialogues = value; }
    public string Voice { get => _voice; }

    public List<INotify<bool>> DialogueSubscriberEntities
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

    public List<INotify<bool>> PrefillINotifyForDialogueSubscriberEntities()
    {
        List<INotify<bool>> entities = new List<INotify<bool>>();

        foreach(DialogueSubscriberEntity subscriberEntity in _dialogueSubscriberEntities)
        {
           INotify<bool> notify = subscriberEntity.Entity.GetComponent<INotify<bool>>();

           if (notify == null)
            {
                continue;
            }

            entities.Add(notify);
        }

        return entities;
    }
}

