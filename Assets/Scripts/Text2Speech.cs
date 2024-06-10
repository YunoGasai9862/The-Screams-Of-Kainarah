using System;
using UnityEngine;

[Serializable]
public class Text2Speech
{
    public string Text { get; set; }
    public AudioSource AudioSource { get; set; }
    public bool TransferSuccessfull { get; set; }

}