using System.Threading.Tasks;
using UnityEngine;

public abstract class Listener : MonoBehaviour, IListenerEntity
{
    public abstract Task Listen();
}