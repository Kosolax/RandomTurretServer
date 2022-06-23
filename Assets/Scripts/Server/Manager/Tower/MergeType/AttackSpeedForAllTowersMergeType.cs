public class AttackSpeedForAllTowersMergeType : IMerge
{
    public void AnimationAndChangeStats(Gamer currentGamer, LocalGameManager localGameManager, GamerGameManager currentPlayerGameManager)
    {
        foreach (int towerIndex in currentGamer.TowersInGame.Keys)
        {
            TowerInGame towerInGame = currentGamer.TowersInGame[towerIndex];
            IEffect effect = new AttackSpeedTower(5.0f, (float) 50 / 100, towerInGame);
            towerInGame.TowerEffectManager.Effects.Add(effect);
            effect.ApplyEffect();
        }
    }
}