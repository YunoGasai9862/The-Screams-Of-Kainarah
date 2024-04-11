
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public interface IAnimatePropertyColor<T>
{
    public abstract IEnumerator AnimColor(T initialColor, T endColor, float duration);
    public abstract T GetDifference(T initialValue, T finalValue);
}