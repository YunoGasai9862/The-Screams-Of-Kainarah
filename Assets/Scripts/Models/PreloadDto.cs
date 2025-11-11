using System;
using UnityEngine;

[Serializable]
public class PreloadDto
{
    [SerializeField]
    private GameObject entity;

    [SerializeField]
    private PreloadEntityType reloadEntityType;

    public GameObject Entity { get { return entity; } }

    public PreloadEntityType PreloadEntityType { get { return reloadEntityType; } }
}