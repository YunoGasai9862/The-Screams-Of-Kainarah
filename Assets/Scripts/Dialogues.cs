using Amazon.Polly;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Dialogues
{
    public string entityName;

    [TextArea(3, 10)]
    public string[] sentences;
}
