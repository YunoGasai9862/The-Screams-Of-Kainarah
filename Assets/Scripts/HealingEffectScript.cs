
using Assets.Scripts.BaseScene;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HealingEffectScript : MonoBehaviorScene
{
    private async void Start()
    {
        StartCoroutine((await (await GetBaseScene()).GetSceneUtilsAsync()).TuneDownIntensityToZero(GetComponent<Light2D>()));
    }
}
