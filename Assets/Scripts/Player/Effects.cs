using System.Threading;
using UnityEngine;

public class Effects: MonoBehaviour, IObserver<IEntityRenderer<Renderer>>
{
    private MaterialFader MaterialFader { get; set; } = new MaterialFader();

    public PlayerAttributesDelegator PlayerAttributesDelegator { get; set; }

    private void Awake()
    {
        PlayerAttributesDelegator = Helper.GetDelegator<PlayerAttributesDelegator>();

        StartCoroutine(PlayerAttributesDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            SubjectType = typeof(PlayerAttributesNotifier).ToString(),
        }, CancellationToken.None));
    }

    public void OnNotify(IEntityRenderer<Renderer> data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        MaterialFader.FadeFloat(new MaterialPropertyUpdate<float>()
        {
            Material = data.Renderer.sharedMaterial,
            PropertyName = "_FadeIn",
            Value = 1.0f
        }, 0.1f, 1);
    }
}