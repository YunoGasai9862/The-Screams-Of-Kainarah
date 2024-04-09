
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public interface IAnimateProperty
{
    public abstract IEnumerator AnimColor(Color initialColor, Color endColor, float duration);
    public abstract IEnumerator AnimMovement(Vector3 initialTransform, Vector3 endTransform, float duration);
}