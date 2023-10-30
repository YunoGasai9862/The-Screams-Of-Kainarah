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
    [SerializeField] float particleMovementSpeed;
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
        StartCoroutine(channelGravity(-1f, 1f));

    }

    async void Update()
    {

        int numberOfParticlesUpdated = _ps.GetParticles(particles); //updates the state of the particles.

        await travelTowardTarget(particles);

        //create release and come back effect. The particles once reached, can go back again for the time being

    }

    private void gravityModifier(float minRange, float maxRange)
    {
    
         float value = Random.Range(minRange, maxRange);
         mainModule.gravityModifierMultiplier += value;
    }

    private IEnumerator channelGravity(float minRange, float maxRange)
    {
        while (true)
        {
            gravityModifier(minRange, maxRange);

            yield return new WaitForSeconds(2f);
        }
    }

    private async Task<bool> travelTowardTarget(Particle[] particles)
    {
        for(int i= 0; i < particles.Length; i++)
        {
            await Task.Delay(0);

            if (!cancellationToken.IsCancellationRequested)
            {
                while (Vector2.Distance(particles[i].position, _target.position)>1f)
                {
                    particles[i].position = Vector2.MoveTowards(particles[i].position, _target.position, Time.deltaTime);
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
