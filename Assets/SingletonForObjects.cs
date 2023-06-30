using UnityEngine;

public class SingletonForObjects : MonoBehaviour
{
    private static DialogueManager _dialogueManager;
    private static InventoryOpenCloseManager _inventoryOpenCloseManager;

    private void Awake()
    {
        _dialogueManager = FindObjectOfType<DialogueManager>();
        _inventoryOpenCloseManager = FindObjectOfType<InventoryOpenCloseManager>();
    }

    public static DialogueManager getDialogueManager()
    {
        return _dialogueManager;
    }

    public static InventoryOpenCloseManager getInventoryOpenCloseManager()
    {
        return _inventoryOpenCloseManager;
    }
}
