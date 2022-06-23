public class AttackSpeedTower : IEffect
{
    public float DiffValue;

    public AttackSpeedTower(float duration, float value, TowerInGame TowerInGame)
    {
        this.Duration = duration;
        this.Value = value;
        this.TowerInGame = TowerInGame;
    }

    public float Duration { get; set; }

    private TowerInGame TowerInGame { get; set; }

    public float Value { get; set; }

    public void ApplyEffect()
    {
        this.DiffValue = this.TowerInGame.AttackTowerManager.AttackSpeed - (this.TowerInGame.AttackTowerManager.AttackSpeed * (1 + this.Value));
        this.TowerInGame.AttackTowerManager.AttackSpeed = this.TowerInGame.AttackTowerManager.AttackSpeed - DiffValue;
    }

    public void Expire()
    {
        int isLastEffect = 0;
        this.TowerInGame.AttackTowerManager.AttackSpeed += this.DiffValue;
        foreach (IEffect effect in this.TowerInGame.TowerEffectManager.Effects)
        {
            if (effect.GetType() == this.GetType())
            {
                isLastEffect++;
            }
        }
        if (isLastEffect <= 1)
        {
            //End Animation client / serveur
        }
    }
}