using UnityEngine;

public class ResetNotifier: MonoBehaviour
{
    [SerializeField]
    DialoguesAndOptions dialoguesAndOptions;

    private async void OnDisable()
    {
        await SceneSingleton.GetEntityListenerDelegator().ListenerDelegator(ResetNotifierSubjects.DialogueAndOptions, dialoguesAndOptions);
    }

}