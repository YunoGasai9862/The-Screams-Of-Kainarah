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

    private void Awake()
    {
        _semaphoreSlim= new SemaphoreSlim(1);
        _cancellationTokenSource= new CancellationTokenSource();
        _cancellationToken= _cancellationTokenSource.Token;
    }
    async void Start()
    {
        await spawnPlayer(Player, locationToSpawn);
        playerMaterial = Player.GetComponent<Renderer>().sharedMaterial; //we have to take it from the Renderer component                                                                     //direct material access is not allowed
        playerMaterial.SetFloat("_FadeIn", _increment);
    }

    async void Update()
    {
        await _semaphoreSlim.WaitAsync();

        if (_increment <= 1 && !_cancellationToken.IsCancellationRequested)
        {
            _increment += incrementValue * Time.deltaTime;
            await FadeIn(playerMaterial, "_FadeIn", _increment, _semaphoreSlim, _cancellationTokenSource);
        }
    }

    public async Task spawnPlayer(GameObject Player, Vector3 locationToSpawn)
    {
        await Task.Delay(TimeSpan.FromSeconds(0));
        Instantiate(Player, locationToSpawn, Player.transform.rotation);
    }

    private async Task FadeIn(Material playerMaterial, string property, float increment, SemaphoreSlim locker, CancellationTokenSource token)
    {
        if (playerMaterial.GetFloat("_FadeIn")> SHADERFADEMAXVALUE)
        {
            cancelPendingCalls(token);
            locker.Release();
            return;
        }

        await Task.Delay(TimeSpan.FromSeconds(.2f));
        var value = playerMaterial.GetFloat(property);
        value += increment; //make the player appear in increments!
        playerMaterial.SetFloat(property, value);
        locker.Release();

    }

    private void cancelPendingCalls(CancellationTokenSource token)
    {
        token.Cancel();
    }

}
