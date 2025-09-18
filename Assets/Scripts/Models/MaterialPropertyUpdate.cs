using UnityEngine;

public class MaterialPropertyUpdate<T>
{
    public Material Material { get; set; }

    public string PropertyName { get; set; }

    public T Value { get; set; }
}