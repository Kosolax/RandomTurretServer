public class TowerStat : BaseBusinessObject
{
    public int Id { get; set; }

    public Stat Stat { get; set; }

    public int StatId { get; set; }

    public int TowerId { get; set; }

    public float Value { get; set; }
}