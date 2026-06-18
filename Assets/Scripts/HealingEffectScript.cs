
using Assets.Scripts.Scene;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HealingEffectScript : MonoBehaviorScene
{
    private void Start()
    {
        StartCoroutine(SceneUtils.TuneDownIntensityToZero(GetComponent<Light2D>()));
    }
}
