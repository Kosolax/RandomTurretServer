using System;
using System.Collections.Generic;

using UnityEngine;

public class GamerGameManager : MonoBehaviour
{
    public GameObject Map;

    public GameObject MobPrefab;

    public Transform StockBulletTransform;

    public Transform StockMobTransform;

    public GameObject TowerPrefab;

    public List<GameObject> TowerSlots;

    public event Action<GameObject, GamerGameManager> OnDeath;

    public event Action<GameObject, GamerGameManager> OnDeathFromMovement;

    public int AutoIncrementId { get; set; }

    public int Id { get; set; }

    public MobBusiness MobBusiness { get; set; }

    public List<GameObject> MobsInstantiated { get; set; }

    public void Death(GameObject mobInScene)
    {
        this.OnDeath?.Invoke(mobInScene, this);
    }

    public void DeathFromMovement(GameObject mobInScene)
    {
        this.OnDeathFromMovement?.Invoke(mobInScene, this);
    }

    public void SpawnMob(int mobId, Mob mob, LocalGameManager localGameManager)
    {
        this.MobBusiness.SpawnMob(mobId, this.Id, this, mob, localGameManager);
    }

    private void Start()
    {
        this.AutoIncrementId = 1;
        this.MobsInstantiated = new List<GameObject>();
    }
}