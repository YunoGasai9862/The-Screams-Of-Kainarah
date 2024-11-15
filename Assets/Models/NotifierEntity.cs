
using System;
using UnityEngine;

[Serializable]
public class NotifierEntity : AbstractNotifierEntity
{

    [SerializeField]
    [ReadOnly]
    private bool m_isActive;

    public bool IsActive { get => m_isActive; set => m_isActive = value; }

    public override string ToString()
    {
        return $"Tag: {Tag}, IsActive {IsActive}";
    }

}