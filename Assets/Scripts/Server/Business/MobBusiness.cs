using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Zenject;

public class MobBusiness
{
    [Inject]
    private readonly GameBusiness gameBusiness;

    [Inject]
    private readonly GamerBusiness gamerBusiness;

    [Inject]
    private readonly MobSendBusiness mobSendBusiness;

    [Inject]
    private readonly Serveur serveur;

    public bool KillMob(bool isFromMovement, bool isGameDone, int clientId, int indexMob, List<GameObject> mobsInstantiated, GameObject mob, LocalGameManager localGameManager)
    {
        if (!isGameDone)
        {
            Gamer currentGamer = this.serveur.Clients[clientId].Gamer;
            currentGamer.Money += 10;
            mobsInstantiated.Remove(mob);
            MobInGame mobInGame = mob.GetComponent<MobInGame>();

            if (isFromMovement)
            {
                currentGamer.Life -= mobInGame.Damage;
                this.gamerBusiness.UpdateMoney(clientId, currentGamer.Money);
                this.gamerBusiness.UpdateLife(clientId, currentGamer.Life);

                if (currentGamer.Life <= 0)
                {
                    this.gameBusiness.EndGame(clientId, localGameManager);
                    return true;
                }
            }
            else
            {
                this.mobSendBusiness.KillMob(clientId, indexMob);
                this.gamerBusiness.UpdateMoney(clientId, currentGamer.Money);
            }

            return false;
        }

        return true;
    }

    public void SpawnMob(int mobId, int clientId, GamerGameManager gamerGameManager, Mob mob, LocalGameManager localGameManager)
    {
        float difficultyMultiplier = localGameManager.Waves[localGameManager.WaveIndex].DifficultyMultiplier;
        this.mobSendBusiness.SpawnMob(mobId, clientId, difficultyMultiplier);
        GameObject mobInScene = HelperManager.Instance.InstantiateGameObject(gamerGameManager.MobPrefab, gamerGameManager.StockMobTransform);
        MobInGame mobInGame = mobInScene.GetComponent<MobInGame>();
        Dictionary<StatType, float> stats = mob.MobStats.ToDictionary(x => x.Stat.StatType, x => x.Value);
        mobInGame.Initialize(gamerGameManager.AutoIncrementId, gamerGameManager.Map, mobInScene, this, GameManager.Instance.MobsColor[(int) mob.MobType], clientId, stats, difficultyMultiplier);
        mobInGame.OnDeath += gamerGameManager.Death;
        mobInGame.ClientId = clientId;
        mobInGame.OnDeathFromMovement += gamerGameManager.DeathFromMovement;
        gamerGameManager.AutoIncrementId++;
        gamerGameManager.MobsInstantiated.Add(mobInScene);
    }

    public void SpawnMobFromWave(Mob currentMob, int mobId, LocalGameManager localGameManager)
    {
        // If there is still mobs to spawn then spawn
        if (currentMob != null)
        {
            foreach (GamerGameManager gamerGameManager in localGameManager.GamersGameManager)
            {
                gamerGameManager.SpawnMob(mobId, currentMob, localGameManager);
            }

            localGameManager.MobIndex++;
        }
        // Else, wait all mobs to die before going on the next wave
        else
        {
            // Count how many mobs are alive in all boards
            int remainingMobs = 0;
            foreach (GamerGameManager gamerGameManager in localGameManager.GamersGameManager)
            {
                remainingMobs += gamerGameManager.StockMobTransform.childCount;
            }

            if (remainingMobs == 0)
            {
                // Prepare everything for the next wave
                localGameManager.WaveIndex++;
                localGameManager.MobIndex = 0;
                this.mobSendBusiness.UpdateWaveNumber(localGameManager.WaveIndex + 1, localGameManager.GamersGameManager[0].Id);

                // if there is no wave available we need to end the game
                if (localGameManager.WaveIndex >= localGameManager.Waves.Count)
                {
                    this.gameBusiness.EndGameWhenNoWaveAvailable(localGameManager);
                }
            }
        }
    }

    public void UpdateMobLife(int clientId, float currentLife, int idMob)
    {
        this.mobSendBusiness.UpdateLife(clientId, currentLife, idMob);
    }
}