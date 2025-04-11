using UnityEngine;

[ObserverSystem(SubjectType = typeof(PlayerAttributesNotifier), ObserverType = typeof(CandleLightPackageGenerator))]

public class PlayerAttributesNotifier: MonoBehaviour
{

}