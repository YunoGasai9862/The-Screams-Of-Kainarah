using System;
using UnityEngine;

[Serializable]
public class DependencyDto
{
    [SerializeField]
    private GameObject dependency;

    [SerializeField]
    private DependencyType dependencyType;

    public GameObject Dependency { get { return dependency; } }

    public DependencyType DependencyType { get { return dependencyType; } }
}