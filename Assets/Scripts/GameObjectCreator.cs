using UnityEngine;

public class GameObjectCreator : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private DialogueEntityScriptableObject dialogueScriptableObject;
    [SerializeField] private PlayerHittableItemsScriptableObject playerHittableItemsScriptableObject;
    [SerializeField] private EntitiesToReset entitiesToResetScriptableObject;

    public static DialogueEntityScriptableObject DialogueEntityScriptableObjectFetch => _instance.dialogueScriptableObject;
    public static PlayerHittableItemsScriptableObject PlayerHittableItemScriptableObjectFetch => _instance.playerHittableItemsScriptableObject;
    public static EntitiesToReset EntitiesToResetScriptableObjectFetch => _instance.entitiesToResetScriptableObject;

    private static DialogueManager _dialogueManager { get; set; }
    private static InventoryOpenCloseManager _inventoryOpenCloseManager { get; set;}
    private static PlayerHelperClassForOtherPurposes _playerHelperClassForOtherPurposes { get; set; }
    private static PlayerObserverListener _playerObserverListener { get; set; }
    private static EnemyObserverListener _enemyObserverListener { get; set; }

    private static GameObjectCreator _instance;

    private void Awake()
    {
        if(_instance == null)
         _instance = this; //creating an instance (singleton pattern)
        _dialogueManager = FindFirstObjectByType<DialogueManager>();  //faster compared to FindObjectOfType
        _inventoryOpenCloseManager = FindFirstObjectByType<InventoryOpenCloseManager>();
        _playerHelperClassForOtherPurposes = FindFirstObjectByType<PlayerHelperClassForOtherPurposes>();
        _playerObserverListener = FindFirstObjectByType<PlayerObserverListener>();
        _enemyObserverListener = FindFirstObjectByType<EnemyObserverListener>();
    }

    public static DialogueManager GetDialogueManager()
    {
        return _dialogueManager;
    }

    public static InventoryOpenCloseManager GetInventoryOpenCloseManager()
    {
        return _inventoryOpenCloseManager;
    }

    public static PlayerHelperClassForOtherPurposes GetPlayerHelperClassObject()
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

}
