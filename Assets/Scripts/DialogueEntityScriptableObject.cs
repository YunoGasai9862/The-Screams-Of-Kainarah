using NUnit.Framework;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue Entities", menuName = "Dialogue Entities")]
[Serializable]
public class DialogueEntityScriptableObject: ScriptableObject
{


    public DialogueEntity[] entities;
}
