using EnemyHittable;
using PlayerHittableItemsNS;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SceneSingleton : MonoBehaviour, ISubjectAsync<IObserverAsync<SceneSingleton>>
{
    [Header("Scriptable Objects")]
    [SerializeField] private DialoguesAndOptions dialogueAndOptions;
    [SerializeField] private PlayerHittableItemsScriptableObject playerHittableItemsScriptableObject;
    [SerializeField] private EntitiesToReset entitiesToResetScriptableObject;
    [SerializeField] private CheckPoints checkpointsScriptableObject;
    [SerializeField] private EnemyHittableObjects enemyHittableObject;
    [SerializeField] private EventStringMapper eventStringMapperScriptableObject;

    [Header("Events")]
    [SerializeField] private DialogueTakingPlaceEvent dialogueTakingPlaceEvent;
    [SerializeField] private EntityPoolManagerEvent entityPoolManagerEvent;

    [Header("Delegators")]
    [SerializeField] private SceneSingletonDelegator sceneSingletonDelegator;

    public static DialoguesAndOptions DialogueAndOptions => _instance.dialogueAndOptions;
    public static PlayerHittableItemsScriptableObject PlayerHittableItems => _instance.playerHittableItemsScriptableObject;
    public static EntitiesToReset EntitiesToReset => _instance.entitiesToResetScriptableObject;
    public static CheckPoints CheckPoints => _instance.checkpointsScriptableObject;
    public static EnemyHittableObjects EnemyHittableObjects => _instance.enemyHittableObject;
    public static EventStringMapper EventStringMapper => _instance.eventStringMapperScriptableObject;

    private static InventoryManager _inventoryManager { get; set; }
    private static PlayerActionRelayer _playerHelperClassForOtherPurposes { get; set; }
    private static EntityListenerDelegator _entityListenerDelegator { get; set; }
    private static EnemyObserverListener _enemyObserverListener { get; set; }
    private static EntitiesToResetActionListener _entitiesToResetActionListener { get; set; }
    private static CheckPointActionListener _checkpointActionListener { get; set; }
    private static SpawnPlayer _getSpawnPlayerScript { get; set; }
    private static CheckpointColliderListener _checkpointColliderListener { get; set; }

    private static DialogueManager _dialogueManager { get; set; }

    private static SceneSingleton _instance;
    private static List<IGameStateHandler> _gameStateHandlerObjects { get; set; }//fill only once
    public static bool IsDialogueTakingPlace { get; set; }
    public EntityPoolManager EntityPoolManager { get; set; }

    private void OnEnable()
    {
        sceneSingletonDelegator.Subject.SetSubject(this);
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this; //creating an instance (singleton pattern)
    }

    private void Start()
    {
        _inventoryManager = FindFirstObjectByType<InventoryManager>();
        _playerHelperClassForOtherPurposes = FindFirstObjectByType<PlayerActionRelayer>();
        _entityListenerDelegator = FindFirstObjectByType<EntityListenerDelegator>();
        _enemyObserverListener = FindFirstObjectByType<EnemyObserverListener>();
        _entitiesToResetActionListener = FindFirstObjectByType<EntitiesToResetActionListener>();
        _checkpointActionListener = FindFirstObjectByType<CheckPointActionListener>();
        _getSpawnPlayerScript = FindFirstObjectByType<SpawnPlayer>();
        _checkpointColliderListener = FindFirstObjectByType<CheckpointColliderListener>();
        _dialogueManager = FindFirstObjectByType<DialogueManager>();
        _gameStateHandlerObjects= new List<IGameStateHandler>();

        //events
        dialogueTakingPlaceEvent.AddListener(DialougeTakingPlace);
        entityPoolManagerEvent.AddListener(EntityPoolManagerEvent);

    }

    public static SpawnPlayer PlayerSpawn()
    {
        return _getSpawnPlayerScript;
    }
    public static InventoryManager GetInventoryManager()
    {
        return _inventoryManager;
    }
    public static List<IGameStateHandler> GameStateHandlerObjects()
    {
        return _gameStateHandlerObjects;
    }

    public static void InsertIntoGameStateHandlerList(IGameStateHandler handler)
    {
        _gameStateHandlerObjects.Add(handler);
    }
    public static PlayerActionRelayer GetPlayerHelperClassObject()
    {
        return _playerHelperClassForOtherPurposes;
    }

    public static EntityListenerDelegator GetEntityListenerDelegator()
    {
        return _entityListenerDelegator;
    }
    public static EnemyObserverListener GetEnemyOberverListenerObject()
    {
        return _enemyObserverListener;
    }

    public static DialogueManager GetDialogueManager()
    {
        return _dialogueManager;
    }
    public static EntitiesToResetActionListener GetEntitiesToResetListenerObject()
    {
        return _entitiesToResetActionListener;
    }
    public static CheckPointActionListener GetCheckPointActionListenerObject()
    {
        return _checkpointActionListener;
    }
    public static CheckpointColliderListener GetCheckPointColliderActionListenerObject()
    {
        return _checkpointColliderListener;
    }
    private void DialougeTakingPlace(bool isTakingPlace)
    {
        IsDialogueTakingPlace = isTakingPlace;
    }

    private void EntityPoolManagerEvent(EntityPoolManager entityPoolManager)
    {
        EntityPoolManager = entityPoolManager;
    }

    public async Task OnNotifySubject(IObserverAsync<SceneSingleton> data, params object[] optional)
    {
       await sceneSingletonDelegator.NotifyObserver(data, this);
    }
}
