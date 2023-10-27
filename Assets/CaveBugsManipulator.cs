using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class CaveBugsManipulator : MonoBehaviour
{
    [Header("Particle System")]
    [SerializeField] ParticleSystem _ps;
    [SerializeField] Transform _target;
    private MainModule mainModule;
    private Particle[] particles;
    private CancellationTokenSource cancellationTokenSource;
    private CancellationToken cancellationToken;

    private void Awake()
    {
        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;
    }


    private void Start()
    {
        mainModule = _ps.main;
        _ps.Play();
    }

    async void Update()
    {
        //await gravityModifier();
        particles = new Particle[mainModule.maxParticles]; //stores particles (updates their position as well)

       int numberOfParticlesUpdated = _ps.GetParticles(particles); //updates the state of the particles.

        await travelTowardTarget(particles);
    }

    private async Task gravityModifier()
    {
        await Task.Delay(1000);
        mainModule.gravityModifierMultiplier -= .001f;
    }

    private async Task travelTowardTarget(Particle[] particles)
    {
        for(int i= 0; i < particles.Length; i++)
        {
            await Task.Delay(500);
            if(!cancellationToken.IsCancellationRequested)
             Debug.Log(particles[i].position);
        }
    }

    private void OnDisable()
    {
        cancellationTokenSource.Cancel();
    }

}
