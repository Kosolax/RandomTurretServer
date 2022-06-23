using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class MobInGame : MonoBehaviour
{
    public Image Image;

    public MobEffectManager MobEffectManager;

    public MobHealthManager MobHealthManager;

    public MobMoveManager MobMoveManager;

    public event Action<GameObject> OnDeath;

    public event Action<GameObject> OnDeathFromMovement;

    public int ClientId { get; set; }

    public float Damage { get; set; }

    public int Id { get; set; }

    public float Life { get; set; }

    public GameObject Map { get; set; }

    public MobBusiness MobBusiness { get; set; }

    public GameObject MobInScene { get; set; }

    public Dictionary<StatType, float> Stats { get; set; }

    public void Initialize(int id, GameObject map, GameObject mobInScene, MobBusiness MobBusiness, Color color, int clientId, Dictionary<StatType, float> stats, float difficultyMultiplier)
    {
        this.Id = id;
        this.Map = map;
        this.MobInScene = mobInScene;
        this.MobBusiness = MobBusiness;
        this.Stats = stats;
        this.Damage = this.Stats[StatType.Damage];
        this.Life = this.Stats[StatType.MaxHealth] * difficultyMultiplier;
        this.Image.color = color;
        this.ClientId = clientId;
        this.MobMoveManager.Initialize(this.Map, this.MobInScene, this.Stats[StatType.Speed]);
        this.MobHealthManager.Initialize(this.Life, this.Stats[StatType.Resistance]);
        this.MobEffectManager.Initialize(this);
    }

    private void ClearEvents()
    {
        this.MobMoveManager.OnDeath -= this.WantToDestroyFromMovement;
        this.MobHealthManager.OnDeath -= this.WantToDestroy;
        this.MobHealthManager.OnHealthChange -= this.HealthChange;
    }

    private void HealthChange(float life)
    {
        this.MobBusiness.UpdateMobLife(this.ClientId, life, this.Id);
    }

    private void Start()
    {
        this.MobMoveManager.OnDeath += this.WantToDestroyFromMovement;
        this.MobHealthManager.OnDeath += this.WantToDestroy;
        this.MobHealthManager.OnHealthChange += this.HealthChange;
    }

    private void WantToDestroy()
    {
        this.ClearEvents();
        this.OnDeath?.Invoke(this.gameObject);
    }

    private void WantToDestroyFromMovement()
    {
        this.ClearEvents();
        this.OnDeathFromMovement?.Invoke(this.gameObject);
    }
}