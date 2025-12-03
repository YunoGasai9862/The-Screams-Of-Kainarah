using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "MovementResetConfig", menuName = "Movement Reset Config")]
[Reset(typeof(AttackState))]
public class MovementResetConfig : ResetConfig
{

}