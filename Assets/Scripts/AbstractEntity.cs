
using UnityEngine;

public abstract class AbstractEntity : MonoBehaviour, IGameStateHandler
{
    public abstract Health Health { get; set; }
    public abstract void GameStateHandler(SceneData data);
}
