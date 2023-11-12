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
    }

    private async Task spawnPlayer(GameObject Player, Vector3 locationToSpawn)
    {
        await Task.Delay(TimeSpan.FromSeconds(0));
        Instantiate(Player, locationToSpawn, Player.transform.rotation);
    }

    private async Task dissolve(Material playerMaterial)
    {
        await Task.Delay(TimeSpan.FromSeconds(0));

    }


}
