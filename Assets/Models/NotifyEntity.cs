
using System;
using UnityEngine;

[Serializable]
public class NotifyEntity
{
    [SerializeField]
    private string m_tag;

    [SerializeField]
    [ReadOnly]
    private bool m_isActive;

    public string Tag { get => m_tag; set => m_tag = value; }

    public bool IsActive { get => m_isActive; set => m_isActive = value; }

    public override string ToString()
    {
        return $"Tag: {Tag}, IsActive {IsActive}";
    }

}