public class SlowMobsMergeType : IMerge
{
    public void AnimationAndChangeStats(Gamer currentGamer, LocalGameManager localGameManager, GamerGameManager currentPlayerGameManager)
    {
        int count = currentPlayerGameManager.StockMobTransform.childCount;
        for (int i = 0; i < count; i++)
        {
            //Give to all the mob present in the current scene the effect slowness
            MobInGame mobInGame = currentPlayerGameManager.StockMobTransform.GetChild(i).GetComponent<MobInGame>();
            IEffect effect = new SlowMob(5.0f, (float) 50 / 100, mobInGame);
            mobInGame.MobEffectManager.Effects.Add(effect);
            effect.ApplyEffect();
        }
    }
}