using UnityEngine;
public class LightEntityDelegator: BaseDelegator<LightEntity>
{
    private void OnEnable()
    {
        Subject = new Subject<IObserver<LightEntity>>();
    }
}