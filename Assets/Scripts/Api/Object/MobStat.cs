public class MobStat : BaseBusinessObject
{
    public int Id { get; set; }

    public int MobId { get; set; }

    public Stat Stat { get; set; }

    public int StatId { get; set; }

    public float Value { get; set; }
}