
using Assets.Scripts.GameState.Models;
using Assets.Scripts.BaseScene;
using UnityEngine;

public abstract class AbstractEntity : MonoBehaviorScene, IGameStateHandler
{
    public abstract Health Health { get; set; }
    public abstract void GameStateHandler(SceneData data);
}
