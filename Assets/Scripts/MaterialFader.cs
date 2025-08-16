using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MaterialFader
{
    const float SHADERFADEMAXVALUE = 1;

    [Header("Prefab/Object To Spawn")]
    [SerializeField] GameObject Player;
    [SerializeField] Vector3 locationToSpawn;
    [SerializeField] float incrementValue;

    private Material playerMaterial;
    private float _increment = 0.0f;

    private void Awake()
    {
        _semaphoreSlim= new SemaphoreSlim(1);
        _cancellationTokenSource= new CancellationTokenSource();
        _cancellationToken= _cancellationTokenSource.Token;
    }
    async void Start()
    {
        playerMaterial = Player.GetComponent<Renderer>().sharedMaterial;
        playerMaterial.SetFloat("_FadeIn", _increment);
    }
   
    public async Task ResetAnimationAndMaterialProperties(GameObject Player, CancellationToken token)
    {
        GetCancellationToken = token;
        await Task.Delay(TimeSpan.FromSeconds(0));
        var animator= Player.GetComponent<Animator>();
        animator.Rebind(); //reset the death anim
        _increment = await ResetMaterialProperties(0, playerMaterial);
    }

    public async Task<float> ResetMaterialProperties(float incrementValue, Material material)
    {
        await Task.Delay(TimeSpan.FromSeconds(0));
        material.SetFloat("_FadeIn", incrementValue);
        return incrementValue;
    }

    //CONVERT TO A COROUTINE
    private async Task FadeIn(Material playerMaterial, string property, float threshold, float increment, CancellationTokenSource token)
    {
        if (playerMaterial.GetFloat(property) > threshold)
        {
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
