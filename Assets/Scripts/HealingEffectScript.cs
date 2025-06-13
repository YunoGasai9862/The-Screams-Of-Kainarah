
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HealingEffectScript : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(Helper.TuneDownIntensityToZero(GetComponent<Light2D>()));
    }
}
