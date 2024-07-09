
using System;
using UnityEngine;

[Serializable]
public class DialogueTriggeringEntity
{
    [SerializeField]
    private GameObject _entity;
    [SerializeField]
    private string _entityTag;

    public GameObject Entity { get => _entity; }
    public string EntityTag { get => _entityTag; }
}