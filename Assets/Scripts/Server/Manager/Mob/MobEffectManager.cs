using System.Collections.Generic;

using UnityEngine;

public class MobEffectManager : MonoBehaviour
{
    public Color BasicColor;

    protected MobInGame MobInGame;

    public float BaseDamage { get; set; }

    public float BaseLife { get; set; }

    public float BaseResistance { get; set; }

    public float BaseSpeed { get; set; }

    public List<IEffect> Effects { get; set; }

    public List<IEffect> ExpiredEffects { get; set; }

    public void Initialize(MobInGame mobInGame)
    {
        this.MobInGame = mobInGame;
        this.BaseLife = mobInGame.Life;
        this.BaseSpeed = mobInGame.Stats[StatType.Speed];
        this.BaseDamage = mobInGame.Damage;
        this.BaseResistance = mobInGame.Stats[StatType.Resistance];
        this.BasicColor = mobInGame.Image.color;
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