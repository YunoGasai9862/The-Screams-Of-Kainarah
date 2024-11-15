using UnityEngine;

public abstract class AbstractNotifierEntity
{
    [SerializeField]
    private string m_tag;
    public string Tag { get => m_tag; set => m_tag = value; }
}