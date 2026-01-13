using System.Threading;
using UnityEngine;

public class Effects: MonoBehaviour, IObserver<Player>
{
    private MaterialFader MaterialFader { get; set; } = new MaterialFader();

    public PlayerAttributesDelegator PlayerAttributesDelegator { get; set; }

    private async void Awake()
    {
        PlayerAttributesDelegator = await Helper.GetDelegator<PlayerAttributesDelegator>();

        StartCoroutine(PlayerAttributesDelegator.NotifySubject(this, new ObserverContext()
        {
            Instance = gameObject,
            SubjectType = typeof(PlayerAttributesNotifier),
        }, CancellationToken.None));
    }

    public void OnNotify(Player data, ObserverContext context, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        Debug.Log($"Material Fader: {MaterialFader} - data : {data.DefaultRendererValue.Renderer}");

        MaterialFader.FadeFloat(new MaterialPropertyUpdate<float>()
        {
            Material = data.DefaultRendererValue.Renderer.sharedMaterial,
            PropertyName = "_FadeIn",
            Value = 1.0f
        }, 0.1f, 1);
    }
}