public class PlayerActionsModel
{
    public PlayerActionsModel() { }
    public bool GetJumpPressed { get; set; }
    public bool GetSlidePressed { get; set; }
    public float CharacterVelocityY { get; set; }
    public float CharacterVelocityX { get; set; }
    public float CharacterSpeed { get; set; }
    public float OriginalSpeed { get; set; }
    public bool LeftMouseButtonPressed { get; set; }
    public float TimeForMouseClickStart { get ; set; } = 0;
    public float TimeForMouseClickEnd { get; set; } = 0;
    public bool DaggerInput { get ; set; }
    public float KeyStrokeDifference { get; set; }
}