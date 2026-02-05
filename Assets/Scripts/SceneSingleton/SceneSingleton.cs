using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.Base;
using PlayerHittableItemsNS;
using System.Collections.Generic;
using UnityEngine;

[Subject(SubjectType = typeof(SceneSingleton), ContextType = typeof(SceneSingleton))]
public class SceneSingleton : MonoBehaviour, IRequest<SceneSingleton>
{
    [Header("Scriptable Objects")]
    [SerializeField] private DialoguesAndOptions dialogueAndOptions;
    [SerializeField] private PlayerHittableItemsScriptableObject playerHittableItemsScriptableObject;
    [SerializeField] private EntitiesToReset entitiesToResetScriptableObject;
    [SerializeField] private CheckPoints checkpointsScriptableObject;
    [SerializeField] private EventStringMapper eventStringMapperScriptableObject;

    private static InventoryManager _inventoryManager { get; set; }
    private static PlayerActionRelayer _playerHelperClassForOtherPurposes { get; set; }
    private static EntitiesToResetActionListener _entitiesToResetActionListener { get; set; }
    private static CheckPointActionListener _checkpointActionListener { get; set; }
    private static CheckpointColliderListener _checkpointColliderListener { get; set; }

    private static DialogueManager _dialogueManager { get; set; }

    private Delegator Delegator { get; set; }

    private static List<IGameStateHandler> _gameStateHandlerObjects { get; set; } = new List<IGameStateHandler>();

    private async void Start()
    {
        _inventoryManager = FindFirstObjectByType<InventoryManager>();
        _playerHelperClassForOtherPurposes = FindFirstObjectByType<PlayerActionRelayer>();
        _entitiesToResetActionListener = FindFirstObjectByType<EntitiesToResetActionListener>();
        _checkpointActionListener = FindFirstObjectByType<CheckPointActionListener>();
        _checkpointColliderListener = FindFirstObjectByType<CheckpointColliderListener>();
        _dialogueManager = FindFirstObjectByType<DialogueManager>();
    }

    public InventoryManager GetInventoryManager()
    {
        return _inventoryManager;
    }
    public List<IGameStateHandler> GameStateHandlerObjects()
    {
        return _gameStateHandlerObjects;
    }

    public void InsertIntoGameStateHandlerList(IGameStateHandler handler)
    {
        _gameStateHandlerObjects.Add(handler);
    }
    public PlayerActionRelayer GetPlayerHelperClassObject()
    {
        return _playerHelperClassForOtherPurposes;
    }

    public DialogueManager GetDialogueManager()
    {
        return _dialogueManager;
    }
    public EntitiesToResetActionListener GetEntitiesToResetListenerObject()
    {
        return _entitiesToResetActionListener;
    }
    public CheckPointActionListener GetCheckPointActionListenerObject()
    {
        return _checkpointActionListener;
    }
    public CheckpointColliderListener GetCheckPointColliderActionListenerObject()
    {
        return _checkpointColliderListener;
    }

    public IEnumerator<SceneSingleton> Request()
    {
        StartCoroutine(Delegator.NotifyObservers(new SubjectContext<SceneSingleton>() { Data = this, EntityType = typeof(SceneSingleton) }, this));

        yield return this;
    }
}
