using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Zenject;

public class TowerBusiness
{
    [Inject]
    private readonly GamerBusiness gamerBusiness;

    [Inject]
    private readonly Serveur serveur;

    [Inject]
    private readonly TowerSendBusiness towerSendBusiness;

    public void InvokeTower(int clientId, int checkClientId)
    {
        if (clientId == checkClientId)
        {
            Gamer currentGamer = this.serveur.Clients[clientId].Gamer;
            if (currentGamer != null &&
                currentGamer.TowersInGame.Count < Gamer.MAX_TOWERS &&
                currentGamer.Money >= currentGamer.TowerPrice &&
                currentGamer.TowerIndexsAvailableToInstantiate.Count > 0)
            {
                int randomPathMapIndex = Random.Range(0, currentGamer.TowerIndexsAvailableToInstantiate.Count);
                int pathMapIndex = currentGamer.TowerIndexsAvailableToInstantiate[randomPathMapIndex];

                List<Tower> towers = this.serveur.Clients[checkClientId].Player.Towers;
                int randomTower = Random.Range(0, towers.Count);

                Lobby lobby = this.serveur.Lobbies[this.serveur.Clients[checkClientId].Gamer.LobbyCode];

                if (this.GenerateTowerServerSide(clientId, currentGamer, pathMapIndex, towers, randomTower, lobby, 1))
                {
                    this.UpdateMoneyAndTowerPrice(clientId, currentGamer);
                    this.towerSendBusiness.InvokeTower(clientId, pathMapIndex, randomTower, 1);
                    currentGamer.TowerIndexsAvailableToInstantiate.RemoveAt(randomPathMapIndex);
                }
            }
        }
    }

    public void MergeTower(int clientId, int checkClientId, int draggedTowerIndex, int droppedTowerIndex)
    {
        if (clientId == checkClientId)
        {
            Gamer currentGamer = this.serveur.Clients[clientId].Gamer;

            // vérifier que index -1 pour les deux
            if (draggedTowerIndex != -1 &&
                droppedTowerIndex != -1 &&
                currentGamer.TowersInGame.ContainsKey(draggedTowerIndex) &&
                currentGamer.TowersInGame.ContainsKey(droppedTowerIndex) &&
                droppedTowerIndex != draggedTowerIndex)
            {
                //récuperer les tours de ces deux index
                TowerInGame draggedTower = this.serveur.Clients[clientId].Gamer.TowersInGame[draggedTowerIndex];
                TowerInGame droppedTower = this.serveur.Clients[clientId].Gamer.TowersInGame[droppedTowerIndex];

                //check if the towers have the same level and merge type
                if (draggedTower.Level == droppedTower.Level && draggedTower.MergeType.GetType() == droppedTower.MergeType.GetType())
                {
                    List<Tower> towers = this.serveur.Clients[clientId].Player.Towers;
                    int randomTower = Random.Range(0, towers.Count);
                    Lobby lobby = this.serveur.Lobbies[this.serveur.Clients[clientId].Gamer.LobbyCode];
                    GameObject game = lobby.Game;
                    LocalGameManager localGameManager = game.GetComponent<LocalGameManager>();
                    GamerGameManager currentPlayerGameManager = localGameManager.GamersGameManager.Where(x => x.Id == checkClientId).FirstOrDefault();

                    //Supprimer la première tour coté serveur
                    currentPlayerGameManager.TowerSlots[draggedTowerIndex].GetComponentInChildren<TowerInGame>().DestroyTower();
                    currentGamer.TowersInGame.Remove(draggedTowerIndex);
                    currentGamer.TowerIndexsAvailableToInstantiate.Add(draggedTowerIndex);

                    //Sauvegarde l'effet du MergeType de la tour cible pour l'instancier a la fin
                    IMerge savedMergeType = currentPlayerGameManager.TowerSlots[droppedTowerIndex].GetComponentInChildren<TowerInGame>().MergeType;

                    //Supprimer la deuxième tour coté serveur
                    currentPlayerGameManager.TowerSlots[droppedTowerIndex].GetComponentInChildren<TowerInGame>().DestroyTower();
                    currentGamer.TowersInGame.Remove(droppedTowerIndex);

                    // Merge des tours coté client
                    this.towerSendBusiness.MergeTower(checkClientId, draggedTowerIndex, droppedTowerIndex);

                    //Instancier la nouvelle tour + forte que les deux précédentes coté serveur
                    this.GenerateTowerServerSide(checkClientId, currentGamer, droppedTowerIndex, towers, randomTower, lobby, draggedTower.Level + 1);
                    this.towerSendBusiness.InvokeTower(checkClientId, droppedTowerIndex, randomTower, draggedTower.Level + 1);

                    //Instanciation de l'effet du merge
                    savedMergeType.AnimationAndChangeStats(currentGamer, localGameManager, currentPlayerGameManager);
                }
            }
        }
    }

    /// <summary>
    /// Shoot on every mob selected
    /// </summary>
    /// <param name="mobsSelected"></param>
    /// <param name="bulletPrefab"></param>
    /// <param name="stockBulletTransform"></param>
    /// <param name="gameObject"></param>
    /// <param name="towerInGame"></param>
    /// <param name="gamerGameManager"></param>
    public void Shoot(List<GameObject> mobsSelected, GameObject bulletPrefab, Transform stockBulletTransform, GameObject gameObject, TowerInGame towerInGame, GamerGameManager gamerGameManager)
    {
        if (gamerGameManager.MobsInstantiated.Count > 0 && gamerGameManager.MobsInstantiated[0] != null)
        {
            foreach (GameObject mobSelected in mobsSelected)
            {
                GameObject bullet = HelperManager.Instance.InstantiateGameObject(bulletPrefab, stockBulletTransform);
                bullet.transform.position = gameObject.transform.position;

                BulletManager bulletManager = bullet.GetComponent<BulletManager>();
                bulletManager.TowerInGame = towerInGame;
                bulletManager.TowerInGame = towerInGame;
                bulletManager.MobInGame = mobSelected.GetComponent<MobInGame>();
            }
        }
    }

    private bool GenerateTowerServerSide(int clientIdCheck, Gamer currentGamer, int towerSlotIndex, List<Tower> towers, int towerIndex, Lobby lobby, int level)
    {
        if (towers[towerIndex].TowerStats != null)
        {
            Dictionary<StatType, float> stats = towers[towerIndex].TowerStats.ToDictionary(x => x.Stat.StatType, x => x.Value);
            if (stats.ContainsKey(StatType.Damage) && stats.ContainsKey(StatType.AttackSpeed))
            {
                GameObject game = lobby.Game;
                LocalGameManager localGameManager = game.GetComponent<LocalGameManager>();
                GamerGameManager currentPlayerGameManager = localGameManager.GamersGameManager.Where(x => x.Id == clientIdCheck).FirstOrDefault();

                GameObject towerInScene = HelperManager.Instance.InstantiateGameObject(
                    currentPlayerGameManager.TowerPrefab,
                    currentPlayerGameManager.TowerSlots[towerSlotIndex].gameObject.transform
                    );
                TowerInGame towerInGame = towerInScene.GetComponent<TowerInGame>();

                // IAim is used to select the mob we want to shoot on
                IAim shootType = this.GetIAimFromShootType(towers[towerIndex].ShootType);
                IMerge mergeType = this.GetIMergeFromMergeType(towers[towerIndex].MergeType);
                towerInGame.Initialize(
                    GameManager.Instance.TowersColor[(int) towers[towerIndex].TowerType],
                    level,
                    currentPlayerGameManager,
                    this,
                    currentPlayerGameManager.StockBulletTransform,
                    shootType,
                    mergeType,
                    stats
                    );
                currentGamer.TowersInGame.Add(towerSlotIndex, towerInGame);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Return the good script depending on the ShootType
    /// </summary>
    /// <param name="shootType"></param>
    /// <returns></returns>
    private IAim GetIAimFromShootType(ShootType shootType)
    {
        // set shoot type to first because api is not using shoottype right now
        shootType = ShootType.First;
        switch (shootType)
        {
            case ShootType.BiggestHp:
                return new BiggestHpShootType();
            case ShootType.First:
                return new FirstShootType();
        }

        return null;
    }

    /// <summary>
    /// Return the good script depending on the MergetType
    /// </summary>
    /// <param name="mergeType"></param>
    /// <returns></returns>
    private IMerge GetIMergeFromMergeType(MergeType MergeType)
    {
        // set merge type to SlowMobs because api is not using mergetype right now
        MergeType = MergeType.SlowMobs;
        switch (MergeType)
        {
            case MergeType.SlowMobs:
                return new SlowMobsMergeType();
            case MergeType.AttackSpeedForAllTowers:
                return new AttackSpeedForAllTowersMergeType();
        }

        return null;
    }

    private void UpdateMoneyAndTowerPrice(int clientId, Gamer currentGamer)
    {
        currentGamer.Money -= currentGamer.TowerPrice;
        this.gamerBusiness.UpdateMoney(clientId, currentGamer.Money);
        currentGamer.TowerPrice += 10;
        this.towerSendBusiness.UpdateTowerPrice(clientId, currentGamer.TowerPrice);
    }
}