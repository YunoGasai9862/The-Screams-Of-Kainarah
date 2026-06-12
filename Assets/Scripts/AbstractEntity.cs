
using Assets.Scripts.Scene;
using Assets.Scripts.ScenePersistence.Models;
using UnityEngine;

public abstract class AbstractEntity : Scene, IGameStateHandler
{
    public abstract Health Health { get; set; }
    public abstract void GameStateHandler(SceneData data);
}
