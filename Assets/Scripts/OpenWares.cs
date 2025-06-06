using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
public class OpenWares : MonoBehaviour, IObserver<GameState>, INotify<bool>
{
    [SerializeField] GameObject MagicCircle;
    [SerializeField] GameObject WaresPanel;
    [SerializeField] GameStateEvent gameStateEvent;
    private GameState CurrentGameState { get; set; }

    private void OnMouseDown()
    {
        if (CurrentGameState.Equals(GameState.DIALOGUE_TAKING_PLACE) && !SceneSingleton.GetInventoryManager().IsPouchOpen)
        {
            WaresPanel.SetActive(true);

            gameStateEvent.Invoke(GameState.SHOPPING);
        }
    }

    public void OnNotify(GameState data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        CurrentGameState = data;

        if (CurrentGameState.Equals(GameState.FREE_MOVEMENT))
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
