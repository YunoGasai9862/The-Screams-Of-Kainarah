using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    [Header("Prefab/Object To Spawn")]
    [SerializeField] GameObject Player;
    [SerializeField] Vector3 locationToSpawn;

    private Material playerMaterial;

    async void Start()
    {
        await spawnPlayer(Player, locationToSpawn);
        playerMaterial = Player.GetComponent<Renderer>().sharedMaterial; //we have to take it from the Renderer component
        //direct material access is not allowed
        await FadeIn(playerMaterial, "_FadeIn");

    }

    private async Task spawnPlayer(GameObject Player, Vector3 locationToSpawn)
    {
        await Task.Delay(TimeSpan.FromSeconds(0));
        Instantiate(Player, locationToSpawn, Player.transform.rotation);
    }

    private async Task FadeIn(Material playerMaterial, string property)
    {
        await Task.Delay(TimeSpan.FromSeconds(.5f));
        var value = playerMaterial.GetFloat(property);
        value += .3f; //make the player appear in increments!
        playerMaterial.SetFloat(property, value);
        

    }


}
