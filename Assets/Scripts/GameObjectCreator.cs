using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class GameObjectCreator : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private DialogueEntityScriptableObject dialogueScriptableObject;
    [SerializeField] private PlayerHittableItemsScriptableObject playerHittableItemsScriptableObject;
    [SerializeField] private EntitiesToReset entitiesToResetScriptableObject;
    [SerializeField] private CheckPoints checkpointsScriptableObject;

    public static DialogueEntityScriptableObject DialogueEntityScriptableObjectFetch => _instance.dialogueScriptableObject;
    public static PlayerHittableItemsScriptableObject PlayerHittableItemScriptableObjectFetch => _instance.playerHittableItemsScriptableObject;
    public static EntitiesToReset EntitiesToResetScriptableObjectFetch => _instance.entitiesToResetScriptableObject;
    public static CheckPoints CheckPointsScriptableObjectFetch => _instance.checkpointsScriptableObject;

    private static DialogueManager _dialogueManager { get; set; }
    private static InventoryOpenCloseManager _inventoryOpenCloseManager { get; set; }
    private static PlayerActionRelayer _playerHelperClassForOtherPurposes { get; set; }
    private static PlayerObserverListener _playerObserverListener { get; set; }
    private static EnemyObserverListener _enemyObserverListener { get; set; }
    private static EntitiesToResetActionListener _entitiesToResetActionListener { get; set; }
    private static CheckPointActionListener _checkpointActionListener { get; set; }
    private static SpawnPlayer _getSpawnPlayerScript { get; set; }
    private static CheckpointColliderListener _checkpointColliderListener { get; set; }

    private static GameObjectCreator _instance;
    private static IGameStateHandler[] _gameStateHandlerObjects { get; set; }//fill only once

    private void Awake()
    {
        if (_instance == null)
            _instance = this; //creating an instance (singleton pattern)
    }

    [System.Obsolete]
    private void Start()
    {
        _dialogueManager = FindFirstObjectByType<DialogueManager>();  //faster compared to FindObjectOfType
        _inventoryOpenCloseManager = FindFirstObjectByType<InventoryOpenCloseManager>();
        _playerHelperClassForOtherPurposes = FindFirstObjectByType<PlayerActionRelayer>();
        _playerObserverListener = FindFirstObjectByType<PlayerObserverListener>();
        _enemyObserverListener = FindFirstObjectByType<EnemyObserverListener>();
        _entitiesToResetActionListener = FindFirstObjectByType<EntitiesToResetActionListener>();
        _checkpointActionListener = FindFirstObjectByType<CheckPointActionListener>();
        _getSpawnPlayerScript = FindFirstObjectByType<SpawnPlayer>();
        _checkpointColliderListener = FindFirstObjectByType<CheckpointColliderListener>();
        _gameStateHandlerObjects = FindObjectsOfType<MonoBehaviour>().OfType<IGameStateHandler>().ToArray();
    }

    public static DialogueManager GetDialogueManager()
    {
        return _dialogueManager;
    }

    public static SpawnPlayer PlayerSpawn()
    {
        return _getSpawnPlayerScript;
    }
    public static InventoryOpenCloseManager GetInventoryOpenCloseManager()
    {
        return _inventoryOpenCloseManager;
    }
    public static async Task<IGameStateHandler[]> GameStateHandlerObjects(SemaphoreSlim semaphoreSlim, float delayInSeconds)
    {
        await Task.Delay(TimeSpan.FromSeconds(delayInSeconds));
        semaphoreSlim.Release();
        return _gameStateHandlerObjects;
    }
    public static void InsertIntoGameStateHandlerList(IGameStateHandler handler)
    {
        _gameStateHandlerObjects.Append(handler);
    }
    public static PlayerActionRelayer GetPlayerHelperClassObject()
    {
        return _playerHelperClassForOtherPurposes;
    }

    public static PlayerObserverListener GetPlayerObserverListenerObject()
    {
        return _playerObserverListener;
    }
    public static EnemyObserverListener GetEnemyOberverListenerObject()
    {
        return _enemyObserverListener;
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

}
