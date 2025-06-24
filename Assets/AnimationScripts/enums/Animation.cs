using System.ComponentModel;

public enum Animation
{
    [Description("walk")]
    START_WALK,

    [Description("walk")]
    STOP_WALK,

    [Description("attack")]
    START_ATTACK,

    [Description("attack")]
    STOP_ATTACK,

    [Description("take_hit")]
    TAKE_HIT
}