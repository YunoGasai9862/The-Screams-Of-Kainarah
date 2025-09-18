using UnityEngine;

public class DialogueEntity
{
    [SerializeField]
    private GameObject _entity;
    [SerializeField]
    private string _entityTag;

    public GameObject Entity { get => _entity; }
    public string EntityTag { get => _entityTag; }
}