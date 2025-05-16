using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[ObserverSystem(SubjectType = typeof(GlobalGameState), ObserverType = typeof(OpenWares))]
public class OpenWares : MonoBehaviour, IGameStateListener, IObserver<GameState>
{
    [SerializeField] GameObject MagicCircle;
    [SerializeField] GameObject WaresPanel;
    [SerializeField] DialogueConcludedEvent dialogeConcludedEvent;

    private GameState CurrentGameState { get; set; }

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
        if (CurrentGameState.Equals(GameState.DIALOGUE_TAKING_PLACE) && !SceneSingleton.GetInventoryManager().IsPouchOpen)
        {
            WaresPanel.SetActive(true);
            Buying = true;
        }

    }

    public Task Ping(GameState gameState)
    {
        CurrentGameState = gameState;

        return Task.CompletedTask;
    }

    public void OnNotify(GameState data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        throw new System.NotImplementedException();
    }
}
