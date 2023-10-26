using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CaveBugsManipulator : MonoBehaviour
{
    [Header("Particle System")]
    [SerializeField] ParticleSystem _ps;

    private void Start()
    {
        _ps.Play();
    }

    [System.Obsolete]
    async void Update()
    {
        await gravityModifier();
    }

    [System.Obsolete]
    private async Task<bool> gravityModifier()
    {
        _ps.gravityModifier -= .1f;
        await Task.Delay(1000);
        return true;
        
    }
}
