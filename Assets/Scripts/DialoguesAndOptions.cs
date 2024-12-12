using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using System.Linq;

[Asset(AssetType = Asset.PRELOADING_SCRIPTABLE_OBJECT)]
[CreateAssetMenu(fileName = "Dialogues And Options", menuName = "Dialogue And Options")]
[Serializable]
public class DialoguesAndOptions: EntityPreloadScriptableObject, IActiveNotifier, IMediatorNotificationListener
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

    public override async Task<Tuple<EntityType, dynamic>> EntityPreload(AssetReference assetReference, EntityType entityType, Preloader preloader)
    {
        UnityEngine.Object emptyObject = await preloader.PreloadAsset<DialoguesAndOptions>(assetReference, entityType);

        return new Tuple<EntityType, dynamic>(entityType, emptyObject);
    }

    public async Task NotifyAboutActivation()
    {
       await Mediator.NotifyManager(new NotifyPackage { EntityNameToNotify = "NotificationManager", NotifierEntity = new NotifierEntity { IsActive = true, Tag = this.name } });
    }

    public async Task MediatorNotificationListener()
    {
        await NotifyAboutActivation();
    }
}