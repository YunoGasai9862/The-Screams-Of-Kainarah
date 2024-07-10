using UnityEngine;

public class ResetNotifier: MonoBehaviour
{
    [SerializeField]
    DialoguesAndOptions dialoguesAndOptions;

    private async void OnDisable()
    {
        Debug.Log("HERE ON DISABLE");
        await SceneSingleton.GetEntityListenerDelegator().ListenerDelegator(ResetNotifierSubjects.DialogueAndOptions, dialoguesAndOptions);
    }

}