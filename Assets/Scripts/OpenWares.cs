using UnityEngine;
public class OpenWares : MonoBehaviour
{
    [SerializeField] GameObject MagicCircle;
    [SerializeField] Conversations checkingDialogue;
    [SerializeField] GameObject WaresPanel;

    public static bool Buying = false;
    // Update is called once per frame
    void Update()
    {
        if (Conversations.MultipleDialogues[checkingDialogue.wizardPlayerConvo])
        {
            MagicCircle.SetActive(true);
        }
    }
    private void OnMouseDown()
    {
        if (SceneSingleton.IsDialogueTakingPlace && !SceneSingleton.GetInventoryManager().IsPouchOpen)
        {
            WaresPanel.SetActive(true);
            Buying = true;
        }

    }
}
