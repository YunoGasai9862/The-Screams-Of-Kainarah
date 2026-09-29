using Annotations.Enums;
using System;
using UnityEngine;

[Serializable]
public class PreloadDto
{
    [SerializeField]
    private GameObject entity;

    [SerializeField] private Asset assetType;

    public GameObject Entity { get { return entity; } }

    public Asset AssetType { get { return assetType; } }
}