using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "GlobalResetConfig", menuName = "Global Reset Config")]
[Reset(typeof(AttackState))]
public class GlobalResetConfig : ResetConfig
{

}