using Assets.Annotations;

public enum PlayerAnimationField
{
    [Display("OVER_ALL_STATE")]
    OverallState,

    [Display("LEAP_STATE")]
    LeapState,
    
    [Display("SPEED")]
    Speed,

    [Display("ATTACK_ON_JUMP")]
    AttackOnJump,

    [Display("LEDGE_GRAB")]
    LedgeGrab,

    [Display("SLIDING")]
    Sliding,

    [Display("RUNNING")]
    Running,

    [Display("THROW_DAGGER")]
    ThrowDagger,

    [Display("ATTACK")]
    Attack,

    [Display("ATTACK_2")]
    Attack2,

    [Display("ATTACK_3")]
    Attack3,

    [Display("ATTACK_4")]
    Attack4,

    [Display("FALL")]
    Fall,

    [Display("DEATH")]
    Death,

    [Display("JUMP")]
    Jump,

    [Display("IDLE_ANIM")]
    IdleAnim
}