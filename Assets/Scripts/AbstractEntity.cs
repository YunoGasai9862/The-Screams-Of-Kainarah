
using UnityEngine;

public abstract class AbstractEntity : MonoBehaviour, IGameStateHandler
{
    protected string m_Name;
    protected float m_health;
    protected float m_maxHealth;
    public abstract string EntityName { set; get; }
    public abstract float Health { set; get; }
    public abstract float MaxHealth { set; get;}
    public abstract void GameStateHandler(SceneData data);
 
}
