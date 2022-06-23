using System;

using UnityEngine;
using UnityEngine.UI;

public class MobHealthManager : MonoBehaviour
{
    public Slider slider;

    public event Action OnDeath;

    public event Action<float> OnHealthChange; 

    public float Resistance { get; set; }

    public void Initialize(float maxLife, float Resistance)
    {
        this.Resistance = Resistance;
        this.SetMaxLife(maxLife);
    }

    public void ReceiveDamage(float damage, MobInGame mobInGame)
    {
        damage -= this.Resistance;
        if(damage > 0)
        {
            this.UpdateLife(mobInGame.Life - damage, mobInGame);
        }

        if (mobInGame.Life <= 0)
        {
            this.OnDeath?.Invoke();
        }
    }

    public void UpdateLife(float lifeSet, MobInGame mobInGame)
    {
        mobInGame.Life = lifeSet;
        this.slider.value = lifeSet;
        this.OnHealthChange?.Invoke(lifeSet);
    }

    private void SetMaxLife(float maxLife)
    {
        this.slider.maxValue = maxLife;
        this.slider.value = maxLife;
    }
}