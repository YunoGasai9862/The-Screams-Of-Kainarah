
using System;
using UnityEngine;

[Serializable]
public class TextAudioPath
{
    [SerializeField]
    [TextArea(3, 10)]
    private string _sentence;

    [SerializeField]
    private string _audioName;

    [ReadOnly]
    public string audioPath;

    public string Sentence { get => _sentence; }
    public string AudioName { get => _audioName; }
    public string AudioPath { get => audioPath; set => audioPath = value; }
}