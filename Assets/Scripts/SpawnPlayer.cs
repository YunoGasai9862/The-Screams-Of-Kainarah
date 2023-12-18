using NUnit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static CheckPoints;
using static GameObjectCreator;

public class SpawnPlayer : MonoBehaviour
{
    const float SHADERFADEMAXVALUE = 1;

    [Header("Prefab/Object To Spawn")]
    [SerializeField] GameObject Player;
    [SerializeField] Vector3 locationToSpawn;
    [SerializeField] float incrementValue;

    private Material playerMaterial;
    private float _increment = 0.0f;
    private SemaphoreSlim _semaphoreSlim;
    private CancellationTokenSource _cancellationTokenSource;
    private CancellationToken _cancellationToken;


    public CancellationToken GetCancellationToken { get => _cancellationToken; set => _cancellationToken = value; }

    private void Awake()
    {
        _semaphoreSlim= new SemaphoreSlim(1);
        _cancellationTokenSource= new CancellationTokenSource();
        _cancellationToken= _cancellationTokenSource.Token;
    }
    async void Start()
    {
        await SpawnEntity(Player, locationToSpawn);
        playerMaterial = Player.GetComponent<Renderer>().sharedMaterial; //we have to take it from the Renderer component                                                                     //direct material access is not allowed
        playerMaterial.SetFloat("_FadeIn", _increment);
    }

    async void Update()
    {
        try
        {
            await _semaphoreSlim.WaitAsync();

            if (_increment <= 1 && !GetCancellationToken.IsCancellationRequested)
            {
                _increment += incrementValue * Time.deltaTime;
                await FadeIn(playerMaterial, "_FadeIn", _increment, _cancellationTokenSource);
            }
        }
        finally
        {
            _semaphoreSlim.Release(); //to release the lock on the last false condition
        }
    }

    public async Task SpawnEntity(GameObject Player, Vector3 locationToSpawn)
    {
        await Task.Delay(TimeSpan.FromSeconds(0));
        Instantiate(Player, locationToSpawn, Player.transform.rotation);
    }
    
    public async Task ResetPlayerAttributes(GameObject Player, Vector3 locationToSpawn, Vector3 offsetValues, CancellationToken token)
    {
        GetCancellationToken = token;

        await Task.Delay(TimeSpan.FromSeconds(0));
        //Load Last CheckPoint here with json
        var animator= Player.GetComponent<Animator>();
        animator.Rebind(); //reset the death anim
        _increment = await ResetMaterialProperties(0, playerMaterial);
        Vector3 newSpawnLocation = locationToSpawn + offsetValues;
        Player.transform.position = newSpawnLocation;
        
    }

    public async Task<float> ResetMaterialProperties(float incrementValue, Material material)
    {
        await Task.Delay(TimeSpan.FromSeconds(0));
        material.SetFloat("_FadeIn", incrementValue);
        return incrementValue;
    }
    private async Task FadeIn(Material playerMaterial, string property, float increment, CancellationTokenSource token)
    {
        if (playerMaterial.GetFloat("_FadeIn")> SHADERFADEMAXVALUE)
        {
            cancelPendingCalls(token);
            return;
        }

        await Task.Delay(TimeSpan.FromSeconds(.2f));
        var value = playerMaterial.GetFloat(property);
        value += increment; //make the player appear in increments!
        playerMaterial.SetFloat(property, value);

    }

    private void cancelPendingCalls(CancellationTokenSource token)
    {
        token.Cancel();
    }

}
