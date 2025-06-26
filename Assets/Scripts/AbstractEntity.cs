
using UnityEngine;

public abstract class AbstractEntity : MonoBehaviour, IGameStateHandler
{
    public abstract PlayerVariables Health { get; set; }
    public abstract void GameStateHandler(SceneData data);
}
