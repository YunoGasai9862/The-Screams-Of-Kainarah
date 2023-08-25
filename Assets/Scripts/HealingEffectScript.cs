
using UnityEngine;
using GlobalAccessAndGameHelper;
using UnityEngine.Rendering.Universal;

public class HealingEffectScript : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(HelperFunctions.TuneDownIntensityToZero(GetComponent<Light2D>()));
    }
}
