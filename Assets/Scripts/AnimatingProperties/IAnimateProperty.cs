
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public interface IAnimateProperty
{
    public abstract IEnumerator AnimColor(Color initialColor, Color endColor, float duration);
    public abstract IEnumerator AnimMovement(Vector3 initialPos, Vector3 finalPos, float duration);
    public abstract IEnumerator AnimMovement(Vector2 initialPos, Vector2 finalPos, float duration);

}