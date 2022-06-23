using System.Collections.Generic;

public class Wave : BaseBusinessObject
{
    public float DifficultyMultiplier { get; set; }

    public int Id { get; set; }

    public int WaveNumber { get; set; }

    public List<WaveMob> WavesMobs { get; set; }
}