using UnityEngine;

public class GameObjectCreator : MonoBehaviour
{
    private static DialogueManager _dialogueManager;
    private static InventoryOpenCloseManager _inventoryOpenCloseManager;
    private static PlayerHelperClassForOtherPurposes _playerHelperClassForOtherPurposes;
    private void Awake()
    { 
        _dialogueManager = FindFirstObjectByType<DialogueManager>();  //faster compared to FindObjectOfType
        _inventoryOpenCloseManager = FindFirstObjectByType<InventoryOpenCloseManager>();
        _playerHelperClassForOtherPurposes = FindFirstObjectByType<PlayerHelperClassForOtherPurposes>();
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
}
