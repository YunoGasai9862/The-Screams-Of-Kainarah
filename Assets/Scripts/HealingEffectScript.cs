
using Assets.Scripts.BaseScene;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HealingEffectScript : MonoBehaviorScene
{
    private async void Start()
    {
        StartCoroutine((await BaseScene.GetSceneUtilsAsync()).TuneDownIntensityToZero(GetComponent<Light2D>()));
    }
}
