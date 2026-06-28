
using Assets.Scripts.GameState.Models;
using Assets.Scripts.Scene;
using UnityEngine;

public abstract class AbstractEntity : Scene, IGameStateHandler
{
    public abstract Health Health { get; set; }
    public abstract void GameStateHandler(SceneData data);
}
