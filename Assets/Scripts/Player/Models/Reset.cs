using System.Collections.Generic;

public class Reset
{
    public bool ShouldReset { get; set; }
    public List<string> ResetParameters { get; set; } = new List<string>();

    public override string ToString()
    {
        return $"ResetParameters: {string.Join(",", ResetParameters?.ToArray())}, ShouldReset: {ShouldReset}";
    }
}
