using UnityEngine;

public class GameObjectCreator : MonoBehaviour
{
    private static DialogueManager _dialogueManager;
    private static InventoryOpenCloseManager _inventoryOpenCloseManager;
    private static PlayerHelperClassForOtherPurposes _playerHelperClassForOtherPurposes;
    private void Awake()
    {
        _dialogueManager = FindObjectOfType<DialogueManager>();
        _inventoryOpenCloseManager = FindObjectOfType<InventoryOpenCloseManager>();
        _playerHelperClassForOtherPurposes = FindObjectOfType<PlayerHelperClassForOtherPurposes>();
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
