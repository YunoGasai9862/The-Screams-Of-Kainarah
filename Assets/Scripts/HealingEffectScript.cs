
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HealingEffectScript : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(HelperFunctions.TuneDownIntensityToZero(GetComponent<Light2D>()));
    }
}
