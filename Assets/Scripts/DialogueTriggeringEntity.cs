
using System;
using UnityEngine;

[Serializable]
public class DialogueTriggeringEntity
{
    private GameObject _entity;
    private string _entityTag;

    public GameObject Entity { get => _entity; }
    public string EntityTag { get => _entityTag; }
}