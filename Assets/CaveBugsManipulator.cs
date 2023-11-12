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
    [SerializeField] string _targetTag;


    [Header("Customizable Behavior")]
    [SerializeField] float particleLerpTiming;
    [SerializeField] float taskDelay;
    [SerializeField] float minGravity;
    [SerializeField] float maxGravity;
    [SerializeField] int minParticleDistanceFromTarget;
    [SerializeField] int maxParticleDistanceFromTarget;


    private MainModule mainModule;
    private Particle[] particles;
    private CancellationTokenSource cancellationTokenSource;
    private CancellationToken cancellationToken;
    private bool _taskRunning = false;
    private GameObject _target;

    //getters and setters
    private bool getTaskRunning {  get =>  _taskRunning; set => _taskRunning = value; }

    private void Awake()
    {
         _target = GameObject.FindWithTag(_targetTag);
        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;
    }


    private async void Start()
    {
        mainModule = _ps.main;
        particles = new Particle[mainModule.maxParticles]; //stores particles (updates their position as well)
        _ps.Play();
        await channelGravity(minGravity, maxGravity);

        getTaskRunning = await travelTowardTarget(particles, taskDelay, minParticleDistanceFromTarget, maxParticleDistanceFromTarget);  //a decent range
  
    }

    async void Update()
    {
        if(_target!=null)
        {
            int numberOfParticlesUpdated = _ps.GetParticles(particles); //updates the state of the particles.

            if (!getTaskRunning && !cancellationToken.IsCancellationRequested)
            {
                getTaskRunning = await travelTowardTarget(particles, taskDelay, minParticleDistanceFromTarget, maxParticleDistanceFromTarget);
            }
        }else
        {
            _target = GameObject.FindWithTag(_targetTag);
        }
    }

    private void gravityModifier(float minRange, float maxRange)
    {
    
         float value = UnityEngine.Random.Range(minRange, maxRange);

         mainModule.gravityModifierMultiplier = value;
    }

    private async Task channelGravity(float minRange, float maxRange)
    {
        while (true)
        {
           gravityModifier(minRange, maxRange);

           await Task.Delay(TimeSpan.FromSeconds(2f));

        }
    }

    private Vector3 randomPosition(Vector3 position, float min, float max)
    {
        float posX = UnityEngine.Random.Range(min, max);
        float posY = UnityEngine.Random.Range(min, max);
        return new Vector3(position.x + posX, position.y + posY);
    }


    private async Task<bool> travelTowardTarget(Particle[] particles, float taskDelay, int randomMin, int randomMax)
    {
        getTaskRunning = true;

        for(int i= 0; i < particles.Length; i++)
        {
            await Task.Delay(TimeSpan.FromSeconds(taskDelay));

            Vector3 target = randomPosition(_target.transform.position, randomMin, randomMax);

            if (!cancellationToken.IsCancellationRequested)
            {
                while (Vector2.Distance(particles[i].position, _target.transform.position) > randomMax + 1)
                {
                    _ps.GetParticles(particles); //updates the state of the particles.
                    particles[i].position = Vector3.Lerp(particles[i].position, target, particleLerpTiming);
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
