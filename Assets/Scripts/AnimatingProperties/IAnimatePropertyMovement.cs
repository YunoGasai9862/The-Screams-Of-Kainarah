
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public interface IAnimatePropertyMovement<T>
{
    public abstract IEnumerator AnimMovement(T initialPos, T finalPos, float duration);
}