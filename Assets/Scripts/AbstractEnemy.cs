
using UnityEngine;

public abstract class AbstractEnemy : MonoBehaviour
{
    protected string m_Name;
    protected int m_health;
    protected int m_maxHealth;
    public abstract string enemyName { set; get; }
    public abstract int health { set; get; }
    public abstract int maxHealth { set; get;}
}
