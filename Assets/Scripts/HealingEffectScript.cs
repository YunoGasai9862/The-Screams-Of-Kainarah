
using Assets.Scripts.Scene;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HealingEffectScript : Scene
{
    private void Start()
    {
        StartCoroutine(SceneUtils.TuneDownIntensityToZero(GetComponent<Light2D>()));
    }
}
