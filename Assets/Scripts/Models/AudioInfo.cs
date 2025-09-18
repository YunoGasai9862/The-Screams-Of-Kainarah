using System;
using UnityEngine;

[Serializable]
public class AudioInfo
{

    [SerializeField]
    private string _audioName;

    [ReadOnly]
    public string audioPath;

    public string AudioName { get => _audioName; }
    public string AudioPath { get => audioPath; set => audioPath = value; }
}