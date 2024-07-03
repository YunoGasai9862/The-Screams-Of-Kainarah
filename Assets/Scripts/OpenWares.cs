using UnityEngine;
public class OpenWares : MonoBehaviour
{
    [SerializeField] GameObject MagicCircle;
    [SerializeField] GameObject WaresPanel;
    [SerializeField] DialogueConcludedEvent dialogeConcludedEvent;

    public static bool Buying = false;
    // Update is called once per frame
    void Update()
    {
        //update this logic later - maybe use an event to notify the subscribed objects that the dialogue has concluded - entities to object mapping
       // if (Conversations.MultipleDialogues[checkingDialogue.wizardPlayerConvo].dialogueConcluded)
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
