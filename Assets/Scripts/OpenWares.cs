using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
public class OpenWares : MonoBehaviour, IObserver<GameStateConsumer>, INotify<bool>
{
    [SerializeField] GameObject MagicCircle;
    [SerializeField] GameObject WaresPanel;
    [SerializeField] GameStateEvent gameStateEvent;
    private GameStateConsumer CurrentGameState { get; set; }

    private void OnMouseDown()
    {
        if (CurrentGameState.Equals(GameStateConsumer.DIALOGUE_TAKING_PLACE) && !SceneSingleton.GetInventoryManager().IsPouchOpen)
        {
            WaresPanel.SetActive(true);

            gameStateEvent.Invoke(GameStateConsumer.SHOPPING);
        }
    }

    public void OnNotify(GameStateConsumer data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        CurrentGameState = data;

        if (CurrentGameState.Equals(GameStateConsumer.FREE_MOVEMENT))
        {
            WaresPanel.SetActive(false);
        }
    }

    public Task Notify(bool value)
    {
        if (value)
        {
            MagicCircle.SetActive(true);
        }

        return Task.CompletedTask;
    }
}
