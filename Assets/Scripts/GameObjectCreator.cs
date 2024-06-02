using EnemyHittable;
using PlayerHittableItemsNS;
using System.Collections.Generic;
using UnityEngine;

public class SceneSingleton : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private DialogueEntityScriptableObject dialogueScriptableObject;
    [SerializeField] private PlayerHittableItemsScriptableObject playerHittableItemsScriptableObject;
    [SerializeField] private EntitiesToReset entitiesToResetScriptableObject;
    [SerializeField] private CheckPoints checkpointsScriptableObject;
    [SerializeField] private EnemyHittableObjects enemyHittableObject;
    [SerializeField] private EventStringMapper eventStringMapperScriptableObject;

    public static DialogueEntityScriptableObject DialogueEntityScriptableObjectFetch => _instance.dialogueScriptableObject;
    public static PlayerHittableItemsScriptableObject PlayerHittableItemScriptableObjectFetch => _instance.playerHittableItemsScriptableObject;
    public static EntitiesToReset EntitiesToResetScriptableObjectFetch => _instance.entitiesToResetScriptableObject;
    public static CheckPoints CheckPointsScriptableObjectFetch => _instance.checkpointsScriptableObject;
    public static EnemyHittableObjects EnemyHittableObjects => _instance.enemyHittableObject;
    public static EventStringMapper EventStringMapperScriptableObject => _instance.eventStringMapperScriptableObject;

    private static DialogueManager _dialogueManager { get; set; }
    private static InventoryManager _inventoryManager { get; set; }
    private static PlayerActionRelayer _playerHelperClassForOtherPurposes { get; set; }
    private static PlayerObserverListener _playerObserverListener { get; set; }
    private static EnemyObserverListener _enemyObserverListener { get; set; }
    private static EntitiesToResetActionListener _entitiesToResetActionListener { get; set; }
    private static CheckPointActionListener _checkpointActionListener { get; set; }
    private static SpawnPlayer _getSpawnPlayerScript { get; set; }
    private static CheckpointColliderListener _checkpointColliderListener { get; set; }

    private static SceneSingleton _instance;
    private static List<IGameStateHandler> _gameStateHandlerObjects { get; set; }//fill only once


    

    private void Awake()
    {
        if (_instance == null)
            _instance = this; //creating an instance (singleton pattern)
    }

    private void Start()
    {
        _dialogueManager = FindFirstObjectByType<DialogueManager>();  //faster compared to FindObjectOfType
        _inventoryManager = FindFirstObjectByType<InventoryManager>();
        _playerHelperClassForOtherPurposes = FindFirstObjectByType<PlayerActionRelayer>();
        _playerObserverListener = FindFirstObjectByType<PlayerObserverListener>();
        _enemyObserverListener = FindFirstObjectByType<EnemyObserverListener>();
        _entitiesToResetActionListener = FindFirstObjectByType<EntitiesToResetActionListener>();
        _checkpointActionListener = FindFirstObjectByType<CheckPointActionListener>();
        _getSpawnPlayerScript = FindFirstObjectByType<SpawnPlayer>();
        _checkpointColliderListener = FindFirstObjectByType<CheckpointColliderListener>();
        _gameStateHandlerObjects= new List<IGameStateHandler>();
    }
    public static DialogueManager GetDialogueManager()
    {
        return _dialogueManager;
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
