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
    private bool _taskRunning = false;

    private void Awake()
    {
        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;
    }


    private void Start()
    {
        mainModule = _ps.main;
        particles = new Particle[mainModule.maxParticles]; //stores particles (updates their position as well)
        _ps.Play();

    }

    async void Update()
    {
        //await gravityModifier();

        int numberOfParticlesUpdated = _ps.GetParticles(particles); //updates the state of the particles.

        if (numberOfParticlesUpdated > 0 && !_taskRunning) //we dont have the particles
        {
          // _taskRunning = true;
          //_taskRunning = await travelTowardTarget(particles);

        }

        for (int i = 0; i < particles.Length; i++)
        {
            await Task.Delay(10);
            if (!cancellationToken.IsCancellationRequested)
            {

              particles[i].position = Vector2.MoveTowards(particles[i].position, _target.position, Time.deltaTime);
             _ps.SetParticles(particles);

            }
        }


    }

    private async Task gravityModifier()
    {
        await Task.Delay(1000);
        mainModule.gravityModifierMultiplier -= .001f;
    }

    private async Task<bool> travelTowardTarget(Particle[] particles)
    {
        for(int i= 0; i < particles.Length; i++)
        {
            await Task.Delay(10);
            if (!cancellationToken.IsCancellationRequested)
            {
                particles[i].position = Vector2.MoveTowards(particles[i].position, _target.position, Time.deltaTime);
            }
        }
        return false;
    }

    private void OnDisable()
    {
        cancellationTokenSource.Cancel();
    }

}
