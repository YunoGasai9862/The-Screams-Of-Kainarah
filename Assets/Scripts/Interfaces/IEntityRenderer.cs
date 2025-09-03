using UnityEngine;

public interface IEntityRenderer<T>
{
    public T Renderer { get; set; }
}