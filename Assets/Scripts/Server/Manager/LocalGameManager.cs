using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class LocalGameManager : MonoBehaviour
{
    public List<GamerGameManager> GamersGameManager;

    private Dictionary<int, Mob> mobDictionary;

    private DateTime oldDate;

    public GameBusiness GameBusiness { get; set; }

    public bool IsGameDone { get; set; }

    public int LobbyCode { get; set; }

    public MobBusiness MobBusiness { get; set; }

    public int MobIndex { get; set; }

    public List<Mob> Mobs { get; set; }

    public int WaveIndex { get; set; }

    public List<Wave> Waves { get; set; }

    public void Initialize(int lobbyCode, List<Mob> mobs, List<Wave> waves, GameBusiness gameBusiness, MobBusiness mobBusiness)
    {
        this.LobbyCode = lobbyCode;
        this.Mobs = mobs;
        this.Waves = waves;
        this.GameBusiness = gameBusiness;
        this.MobBusiness = mobBusiness;
    }

    private void FixedUpdate()
    {
        if (!this.IsGameDone)
        {
            //Spawn every 3 seconds
            if ((DateTime.Now - oldDate).TotalSeconds > 3)
            {
                oldDate = DateTime.Now;
                Wave currentWave = this.Waves[this.WaveIndex];
                //If a wave was created without any mob, the wave will just get skipped
                if (currentWave.WavesMobs != null)
                {
                    if (currentWave.WavesMobs.Count > this.MobIndex)
                    {
                        int mobId = currentWave.WavesMobs[this.MobIndex].MobId;

                        if (mobDictionary.ContainsKey(mobId))
                        {
                            Mob currentMob = mobDictionary[mobId];
                            this.MobBusiness.SpawnMobFromWave(currentMob, mobId, this);
                        }
                        else
                        {
                            this.MobBusiness.SpawnMobFromWave(null, mobId, this);
                        }
                    }
                    else
                    {
                        this.MobBusiness.SpawnMobFromWave(null, 0, this);
                    }
                }
                else
                {
                    this.MobBusiness.SpawnMobFromWave(null, 0, this);
                }
            }
        }
    }

    private void MobDeadFromMovement(GameObject mobInScene, GamerGameManager gamerGameManager)
    {
        MobInGame mobInGame = mobInScene.GetComponent<MobInGame>();
        int indexMob = gamerGameManager.MobsInstantiated.FindIndex(x => x.GetComponent<MobInGame>().Id == mobInGame.Id);
        this.MobBusiness.KillMob(true, this.IsGameDone, gamerGameManager.Id, indexMob, gamerGameManager.MobsInstantiated, mobInScene, this);
        Destroy(mobInScene);
    }

    private void MobDeath(GameObject mobInScene, GamerGameManager gamerGameManager)
    {
        MobInGame mobInGame = mobInScene.GetComponent<MobInGame>();
        int indexMob = gamerGameManager.MobsInstantiated.FindIndex(x => x.GetComponent<MobInGame>().Id == mobInGame.Id);
        this.MobBusiness.KillMob(false, this.IsGameDone, gamerGameManager.Id, indexMob, gamerGameManager.MobsInstantiated, mobInScene, this);
        Destroy(mobInScene);
    }

    private void Start()
    {
        this.mobDictionary = this.Mobs.ToDictionary(x => x.Id, x => x);
        this.WaveIndex = 0;
        this.MobIndex = 0;
        this.oldDate = DateTime.Now;
        this.IsGameDone = false;

        foreach (GamerGameManager gamerGameManager in this.GamersGameManager)
        {
            gamerGameManager.OnDeath += this.MobDeath;
            gamerGameManager.OnDeathFromMovement += this.MobDeadFromMovement;
        }
    }
}