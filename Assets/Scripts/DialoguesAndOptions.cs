using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using System.Linq;

[Asset(AssetType = Asset.SCRIPTABLE_OBJECT, AddressLabel = "DialoguesAndOptions")]
[CreateAssetMenu(fileName = "Dialogues And Options", menuName = "Dialogue And Options")]
[Serializable]
public class DialoguesAndOptions: ScriptableObject, IActiveNotifier, IMediatorNotificationListener
{
    private IMediator Mediator { get; set; }

    [Serializable]
    public class DialogueSystem
    {
        [SerializeField]
        private List<Dialogues> _dialogues;
        [SerializeField]
        private DialogueOptions _dialogueOptions;
        [SerializeField]
        private DialogueTriggeringEntity _dialogueTriggeringEntity;

        public List<Dialogues> Dialogues { get => _dialogues; }
        public DialogueOptions DialogueOptions { get => _dialogueOptions;  }
        public DialogueTriggeringEntity DialogueTriggeringEntity { get => _dialogueTriggeringEntity; }
    }

    public List<DialogueSystem> exchange;

    public async Task NotifyAboutActivation()
    {
       await Mediator.NotifyManager(new NotifyPackage { EntityNameToNotify = "NotificationManager", NotifierEntity = new NotifierEntity { IsActive = true, Tag = this.name } });
    }

    public async Task MediatorNotificationListener()
    {
        await NotifyAboutActivation();
    }
}