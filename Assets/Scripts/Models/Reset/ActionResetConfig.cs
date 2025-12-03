using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ActionResetConfig", menuName = "Action Reset Config")]
[Reset(typeof(ActionState))]
public class ActionResetConfig : ResetConfig
{

}