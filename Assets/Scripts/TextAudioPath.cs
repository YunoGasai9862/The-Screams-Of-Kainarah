
using System;
using UnityEngine;

[Serializable]
public class TextAudioPath
{
    [SerializeField]
    [TextArea(3, 10)]
    private string _sentence;

    [ReadOnly]
    public string _audioPath;

    public string Sentence { get => _sentence; }
    public string AudioPath { get => _audioPath; set => _audioPath = value; }

}