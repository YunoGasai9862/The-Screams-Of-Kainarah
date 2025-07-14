
//hmm, maybe use a class that couples IS_SLIDING and concluded = true - something along those lines or keep track of current action
public enum PlayerState
{
    IS_SLIDING,

    IS_ATTACKING,

    IS_GRABBING,

    IS_JUMPING,

    CURRENT_ACTION_CONCLUDED
}