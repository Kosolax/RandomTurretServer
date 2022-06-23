using System.Collections.Generic;

public class Tower : BaseBusinessObject
{
    public int Id { get; set; }

    public MergeType MergeType { get; set; }

    public string Name { get; set; }

    public int PlayerId { get; set; }

    public ShootType ShootType { get; set; }

    public List<TowerStat> TowerStats { get; set; }

    public TowerType TowerType { get; set; }
}