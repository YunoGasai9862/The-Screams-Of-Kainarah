using UnityEngine;

public abstract class AbstractEntityPool
{
    public string Name { get; set; }
    public string Tag { get; set; }
    public GameObject Entity { get; set; }
}