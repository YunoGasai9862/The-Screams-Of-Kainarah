using EnemyHittable;
using PlayerHittableItemsNS;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SceneSingleton : MonoBehaviour, ISubject<IObserver<SceneSingleton>>
{
    [Header("Scriptable Objects")]
    [SerializeField] private DialoguesAndOptions dialogueAndOptions;
    [SerializeField] private PlayerHittableItemsScriptableObject playerHittableItemsScriptableObject;
    [SerializeField] private EntitiesToReset entitiesToResetScriptableObject;
    [SerializeField] private CheckPoints checkpointsScriptableObject;
    [SerializeField] private EnemyHittableObjects enemyHittableObject;
    [SerializeField] private EventStringMapper eventStringMapperScriptableObject;

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

    private static List<IGameStateHandler> _gameStateHandlerObjects { get; set; } = new List<IGameStateHandler>();

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

        //delegators
        sceneSingletonDelegator.Subject.SetSubject(this);

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

    public void OnNotifySubject(IObserver<SceneSingleton> data, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StartCoroutine(sceneSingletonDelegator.NotifyObserver(data, this, notificationContext, cancellationToken, semaphoreSlim));
    }
}
