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

    public static DialogueManager getDialogueManager()
    {
        return _dialogueManager;
    }

    public static InventoryOpenCloseManager getInventoryOpenCloseManager()
    {
        return _inventoryOpenCloseManager;
    }

    public static PlayerHelperClassForOtherPurposes getPlayerHelperClassObject()
    {
        return _playerHelperClassForOtherPurposes;
    }
}
