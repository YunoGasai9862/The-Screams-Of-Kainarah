using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
public class OpenWares : MonoBehaviour, IObserver<GameState>, INotify<bool>
{
    [SerializeField] GameObject MagicCircle;
    [SerializeField] GameObject WaresPanel;
    private GameState CurrentGameState { get; set; }

    //fix this too - why static!
    //use event driven approach
    public static bool Buying = false;
    // Update is called once per frame

    private void OnMouseDown()
    {
        if (CurrentGameState.Equals(GameState.DIALOGUE_TAKING_PLACE) && !SceneSingleton.GetInventoryManager().IsPouchOpen)
        {
            WaresPanel.SetActive(true);

            Buying = true;
        }

    }

    public void OnNotify(GameState data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        CurrentGameState = data;
    }

    public Task Notify(bool value)
    {
        Debug.Log("Here!");

        if (value)
        {
            MagicCircle.SetActive(true);
        }

        return Task.CompletedTask;
    }
}
