using UnityEngine;
using UnityEngine.UI;

public class SlowMob : IEffect
{
    public float DiffValue;

    public SlowMob(float duration, float value, MobInGame mobInGame)
    {
        this.Duration = duration;
        this.Value = value;
        this.MobInGame = mobInGame;
    }

    public float Duration { get; set; }

    private MobInGame MobInGame { get; set; }

    public float Value { get; set; }

    public void ApplyEffect()
    {
        this.DiffValue = this.MobInGame.MobMoveManager.Speed - (this.MobInGame.MobMoveManager.Speed * (1 - this.Value));
        this.MobInGame.MobMoveManager.Speed = this.MobInGame.MobMoveManager.Speed - DiffValue;
        this.MobInGame.gameObject.GetComponentInChildren<Image>().color = Color.blue;
    }

    public void Expire()
    {
        int isLastEffect = 0;
        this.MobInGame.MobMoveManager.Speed += this.DiffValue;
        foreach (IEffect effect in this.MobInGame.MobEffectManager.Effects)
        {
            if (effect.GetType() == this.GetType())
            {
                isLastEffect++;
            }
        }
        if (isLastEffect <= 1)
        {
            this.MobInGame.gameObject.GetComponentInChildren<Image>().color = this.MobInGame.MobEffectManager.BasicColor;
        }
    }
}