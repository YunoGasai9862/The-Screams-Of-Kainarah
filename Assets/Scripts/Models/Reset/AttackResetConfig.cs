using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "AttackResetConfig", menuName = "Attack Reset Config")]
[Reset(typeof(AttackState))]
public class AttackResetConfig : ResetConfig
{

}