
//hmm, maybe use a class that couples IS_SLIDING and concluded = true - something along those lines or keep track of current action
public enum PlayerState
{
    IS_RUNNING,

    IS_WALKING,

    IS_IDLE,

    IS_SLIDING,

    IS_FALLING,

    IS_ATTACKING,

    IS_GRABBING,

    IS_JUMPING,

    CURRENT_ACTION_CONCLUDED
}