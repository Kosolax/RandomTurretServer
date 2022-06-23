using System.Collections.Generic;

public class Mob : BaseBusinessObject
{
    public int Id { get; set; }

    public List<MobStat> MobStats { get; set; }

    public MobType MobType { get; set; }
}