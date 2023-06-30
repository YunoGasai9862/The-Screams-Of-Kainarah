using UnityEngine;

public class OpenWares : MonoBehaviour
{
    [SerializeField] GameObject MagicCircle;
    [SerializeField] Interactable checkingDialogue;
    [SerializeField] GameObject WaresPanel;
    public static bool Buying = false;

    // Update is called once per frame
    void Update()
    {
        if (Interactable.MultipleDialogues[checkingDialogue.WizardPlayerConvo])
        {
            MagicCircle.SetActive(true);
        }


    }
    private void OnMouseDown()
    {
        if (SingletonForObjects.getDialogueManager().getIsOpen() && !SingletonForObjects.getInventoryOpenCloseManager().isOpenInventory)
        {
            WaresPanel.SetActive(true);
            Buying = true;
        }

    }
}
