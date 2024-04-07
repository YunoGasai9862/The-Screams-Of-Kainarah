
using UnityEngine;
public interface IAnimateProperty
{
    public abstract Color AnimColor(Color color, float duration);
    public abstract Transform AnimMovement(Transform transform, float duration);
}