
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public interface IAnimatePropertyVector<T>
{
    public abstract IEnumerator AnimMovement(T initialPos, T finalPos, float duration);
    public abstract T GetDifference(T initialValue, T finalValue);

}