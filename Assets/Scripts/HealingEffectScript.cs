
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HealingEffectScript : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(SceneUtils.TuneDownIntensityToZero(GetComponent<Light2D>()));
    }
}
