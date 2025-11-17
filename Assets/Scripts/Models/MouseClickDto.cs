using System;

public class MouseClickDto
{
    public float ClickStartTime { get; set; }
    public float ClickEndTime { get; set; }

    public float TimeDifference { get => Math.Abs(ClickEndTime - ClickStartTime); }
}