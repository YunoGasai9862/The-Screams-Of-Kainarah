using Annotations.Enums;
using System;
using UnityEngine;

[Serializable]
[Asset(Asset.SCRIPTABLE_OBJECT, "AttackResetConfig", InstantiationOrder = 10)]
[CreateAssetMenu(fileName = "AttackResetConfig", menuName = "Attack Reset Config")]
[Reset(typeof(AttackState))]
public class AttackResetConfig : ResetConfig
{

}