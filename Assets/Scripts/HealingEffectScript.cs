
using Assets.Scripts.Scene;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HealingEffectScript : Scene
{
    private async void Start()
    {
        StartCoroutine((await BaseScene.GetSceneUtilsAsync()).TuneDownIntensityToZero(GetComponent<Light2D>()));
    }
}
