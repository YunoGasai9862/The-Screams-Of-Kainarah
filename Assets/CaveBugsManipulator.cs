using System;
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
    [SerializeField] float particleInterpolationTime;
    [SerializeField] float delay;
    private MainModule mainModule;
    private Particle[] particles;
    private CancellationTokenSource cancellationTokenSource;
    private CancellationToken cancellationToken;
    private bool _taskRunning = true;


    private void Awake()
    {
        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;
    }


    private async void Start()
    {
        mainModule = _ps.main;
        particles = new Particle[mainModule.maxParticles]; //stores particles (updates their position as well)
        _ps.Play();
        //await channelGravity(-1f, 1f);

        _taskRunning = await travelTowardTarget(particles, delay);
  
    }

    async void Update()
    {
        int numberOfParticlesUpdated = _ps.GetParticles(particles); //updates the state of the particles.
   
        if (!cancellationToken.IsCancellationRequested && !_taskRunning)
        {
            _taskRunning = await travelTowardTarget(particles, delay);
        }
    }

    private void gravityModifier(float minRange, float maxRange)
    {
    
         float value = UnityEngine.Random.Range(minRange, maxRange);
         mainModule.gravityModifierMultiplier += value;
    }

    private async Task channelGravity(float minRange, float maxRange)
    {
        while (true)
        {
           gravityModifier(minRange, maxRange);

           await Task.Delay(TimeSpan.FromSeconds(2f));

        }
    }

    private async Task<bool> travelTowardTarget(Particle[] particles, float delay)
    {
        _taskRunning = true;

        for(int i= 0; i < particles.Length; i++)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));

            if (!cancellationToken.IsCancellationRequested)
            {
                while (Vector2.Distance(particles[i].position, _target.position)>1f)
                {
                    particles[i].position = Vector3.Lerp(particles[i].position, _target.position, particleInterpolationTime);

                    _ps.SetParticles(particles);
                }
            }
        }
        return false;
    }

    private void OnDisable()
    {
        cancellationTokenSource.Cancel();
    }


}
