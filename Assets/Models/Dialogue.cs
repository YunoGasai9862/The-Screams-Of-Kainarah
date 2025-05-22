
using System;
using UnityEngine;

[Serializable]
public class Dialogue
{
    [SerializeField]
    [TextArea(3, 10)]
    private string _sentence;

    public string Sentence { get => _sentence; }

    public AudioInfo AudioInfo { get; set; }
}