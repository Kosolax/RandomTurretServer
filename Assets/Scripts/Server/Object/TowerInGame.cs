using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class TowerInGame : MonoBehaviour
{
    public AttackTowerManager AttackTowerManager;

    public GameObject BulletPrefab;

    public Image Image;

    public Text LevelText;

    public TowerEffectManager TowerEffectManager;

    private int level;

    public GamerGameManager GamerGameManager { get; set; }

    public int Level
    {
        get => this.level;
        set
        {
            this.level = value;
            this.LevelText.text = this.Level.ToString();
        }
    }

    public IMerge MergeType { get; set; }

    public IAim ShootType { get; set; }

    public Dictionary<StatType, float> Stats { get; set; }

    public Transform StockBulletTransform { get; set; }

    public TowerBusiness TowerBusiness { get; set; }

    public void DestroyTower()
    {
        this.AttackTowerManager.CanShoot -= this.Shoot;
        Destroy(this.gameObject);
    }

    public void Initialize(Color color, int level, GamerGameManager gamerGameManager, TowerBusiness towerBusiness, Transform stockBulletTransform, IAim shootType, IMerge mergeType, Dictionary<StatType, float> stats)
    {
        this.Image.color = color;
        this.Level = level;
        this.GamerGameManager = gamerGameManager;
        this.TowerBusiness = towerBusiness;
        this.StockBulletTransform = stockBulletTransform;
        this.ShootType = shootType;
        this.MergeType = mergeType;
        this.Stats = stats;
        this.AttackTowerManager.Initialize(this.Stats[StatType.AttackSpeed]);
        this.TowerEffectManager.Initialize(this);
    }

    /// <summary>
    /// Select mobs depending on the IAim and then call the business to shoot
    /// </summary>
    private void Shoot()
    {
        List<GameObject> mobsSelected = this.ShootType.SelectMobs(this.GamerGameManager.MobsInstantiated);
        this.TowerBusiness.Shoot(mobsSelected, this.BulletPrefab, this.StockBulletTransform, this.gameObject, this, this.GamerGameManager);
    }

    private void Start()
    {
        this.AttackTowerManager.CanShoot += this.Shoot;
    }
}