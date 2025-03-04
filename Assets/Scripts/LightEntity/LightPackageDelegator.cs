using UnityEngine;
public class LightPackageDelegator: BaseDelegator<LightPackage>
{
    private void OnEnable()
    {
        Subject = new Subject<IObserver<LightPackage>>();
    }
}