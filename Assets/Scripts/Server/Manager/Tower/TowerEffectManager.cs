using System.Collections.Generic;

using UnityEngine;

public class TowerEffectManager : MonoBehaviour
{
    protected TowerInGame TowerInGame;

    public float BaseAttackSpeed { get; set; }

    public float BaseDamage { get; set; }

    public float BaseLevel { get; set; }

    public List<IEffect> Effects { get; set; }

    public List<IEffect> ExpiredEffects { get; set; }

    public void Initialize(TowerInGame towerInGame)
    {
        this.TowerInGame = towerInGame;
        this.BaseLevel = towerInGame.Level;
        this.BaseAttackSpeed = towerInGame.AttackTowerManager.AttackSpeed;
        this.BaseDamage = towerInGame.Stats[StatType.Damage];
        this.Effects = new List<IEffect>();
        this.ExpiredEffects = new List<IEffect>();
    }

    private void FixedUpdate()
    {
        if (this.Effects.Count != 0)
        {
            if (this.ExpiredEffects.Count != 0)
            {
                this.ExpiredEffects.Clear();
            }
            foreach (IEffect effect in this.Effects)
            {
                effect.Duration -= Time.deltaTime;
                if (effect.Duration <= 0.0f)
                {
                    this.ExpiredEffects.Add(effect);
                }
            }
            foreach (IEffect effect in this.ExpiredEffects)
            {
                effect.Expire();
                this.Effects.Remove(effect);
            }
        }
    }
}