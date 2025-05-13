using UnityEngine;

[ObserverSystem(SubjectType = typeof(GlobalGameState), ObserverType = typeof(I))]
public class GlobalGameState: MonoBehaviour
{

}