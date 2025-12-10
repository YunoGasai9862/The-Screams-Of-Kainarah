
using UnityEngine;

public class PlayerActionsModel
{
    public PlayerActionsModel() { }
    public bool GetJumpPressed { get; set; }
    public bool GetSlidePressed { get; set; }
    public Vector2 CharacterVelocity { get; set; }
    public Vector2 CharacterSpeed { get; set; }
    public Vector2 OriginalSpeed { get; set; }
    public bool LeftMouseButtonPressed { get; set; }
    public float TimeForMouseClickStart { get ; set; } = 0;
    public float TimeForMouseClickEnd { get; set; } = 0;
    public bool DaggerInput { get ; set; }
    public float KeyStrokeDifference { get; set; }
    public bool VBoostKeyPressed { get; set; }
}