using Annotations.Enums;
using System;
using UnityEngine;

[Serializable]
public class PreloadDto
{
    [SerializeField]
    private GameObject entity;

    [SerializeField]
    private PreloadEntityType preloadEntityType;

    [SerializeField] private Asset assetType;

    public GameObject Entity { get { return entity; } }

    public PreloadEntityType PreloadEntityType { get { return preloadEntityType; } }

    public Asset AssetType { get { return assetType; } }
}