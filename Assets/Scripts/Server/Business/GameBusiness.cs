using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;

using Zenject;

public class GameBusiness
{
    [Inject]
    private readonly GameSendBusiness gameSendBusiness;

    [Inject]
    private readonly MobClient mobClient;

    [Inject]
    private readonly PlayerClient playerClient;

    [Inject]
    private readonly Serveur serveur;

    [Inject]
    private readonly StatClient statClient;

    [Inject]
    private readonly WaveClient waveClient;

    private List<Mob> mobs;

    public async Task AskForGame(int clientId, int checkClientId)
    {
        if (clientId == checkClientId)
        {
            if (this.serveur.Lobbies.Count > 0)
            {
                foreach (Lobby lobby in this.serveur.Lobbies.Values)
                {
                    if (!lobby.IsPrivate && lobby.Clients.Count < Lobby.MAXPLAYER)
                    {
                        this.SetGamerToLobby(clientId, lobby);

                        if (lobby.Clients.Count == Lobby.MAXPLAYER)
                        {
                            await this.PrepareGame(clientId, lobby);
                            return;
                        }

                        return;
                    }
                }

                this.CreateLobby(clientId, false);
            }
            else
            {
                this.CreateLobby(clientId, false);
            }
        }
    }

    public Task AskForPrivateGame(int clientId, int checkClientId)
    {
        if (clientId == checkClientId)
        {
            this.CreateLobby(clientId, true);
        }

        return Task.CompletedTask;
    }

    public async Task AskJoinPrivateGame(int clientId, int checkClientId, int lobbyCode)
    {
        if (clientId == checkClientId)
        {
            if (this.serveur.Lobbies.ContainsKey(lobbyCode))
            {
                Lobby lobby = this.serveur.Lobbies[lobbyCode];
                if (lobby.Clients.Count < Lobby.MAXPLAYER)
                {
                    this.SetGamerToLobby(clientId, lobby);

                    if (lobby.Clients.Count == Lobby.MAXPLAYER)
                    {
                        await this.PrepareGame(clientId, lobby);
                        return;
                    }

                    return;
                }
            }
        }

        this.gameSendBusiness.SetJoinError(clientId);
    }

    private async Task PrepareGame(int clientId, Lobby lobby)
    {
        lobby.IsGameStarted = true;
        Dictionary<int, Stat> stats = (await this.statClient.List()).ToDictionary(x => x.Id);

        foreach (Client client in lobby.Clients)
        {
            foreach (Tower tower in client.Player.Towers)
            {
                foreach (TowerStat towerStat in tower.TowerStats)
                {
                    towerStat.Stat = stats[towerStat.StatId];
                }
            }
        }

        this.mobs = await this.mobClient.List();

        foreach (Mob mob in this.mobs)
        {
            foreach (MobStat mobStat in mob.MobStats)
            {
                mobStat.Stat = stats[mobStat.StatId];
            }
        }

        this.gameSendBusiness.LoadGame(lobby.Clients, clientId, this.mobs);
    }

    public void CancelAskForGame(int clientId, int checkClientId)
    {
        if (clientId == checkClientId)
        {
            int lobbyCode = this.serveur.Clients[clientId].Gamer.LobbyCode;
            if (this.serveur.Lobbies.ContainsKey(lobbyCode))
            {
                Lobby lobby = this.serveur.Lobbies[lobbyCode];
                lobby.Clients.Remove(this.serveur.Clients[clientId]);
                this.serveur.Clients[clientId].Gamer = new Gamer();
                this.gameSendBusiness.CancelLoadingGame(clientId);

                if (lobby.Clients.Count == 0)
                {
                    Lobby.CodeUsed.Remove(lobbyCode);
                    this.serveur.Lobbies.Remove(lobbyCode);
                }

                Debug.Log(" left the lobby");
            }
        }
    }

    public void DisconnectedFromGame(int clientId)
    {
        Debug.Log(" left the game");
        this.serveur.Clients[clientId].Gamer = new Gamer();
    }

    public void DrawGame(int lobbyCode, LocalGameManager localGameManager)
    {
        localGameManager.IsGameDone = true;
        if (this.serveur.Lobbies.ContainsKey(lobbyCode))
        {
            List<Client> clients = this.serveur.Lobbies[lobbyCode].Clients;
            Client firstClient = clients.FirstOrDefault();
            Client lastClient = clients.LastOrDefault();
            int newWinnerElo = this.CalculateElo(firstClient, null, firstClient.Player.Elo, lastClient.Player.Elo);
            int newLoserElo = this.CalculateElo(lastClient, null, lastClient.Player.Elo, firstClient.Player.Elo);
            this.gameSendBusiness.DrawGame(lobbyCode, newWinnerElo, newLoserElo);
            Lobby.CodeUsed.Remove(lobbyCode);
            this.serveur.Lobbies.Remove(lobbyCode);
        }

        HelperManager.Instance.DestroyGameObject(localGameManager.gameObject);
    }

    public void EndGame(int clientIdThatLost, LocalGameManager localGameManager)
    {
        localGameManager.IsGameDone = true;
        if (this.serveur.Clients.ContainsKey(clientIdThatLost))
        {
            int lobbyCode = this.serveur.Clients[clientIdThatLost].Gamer.LobbyCode;

            Client clientLoser = this.serveur.Clients[clientIdThatLost];
            Client clientWinner = this.serveur.Lobbies[this.serveur.Clients[clientIdThatLost].Gamer.LobbyCode].Clients.Where(x => x.Id != clientIdThatLost).FirstOrDefault();
            int newLoserElo = this.CalculateElo(clientLoser, false, clientLoser.Player.Elo, clientWinner.Player.Elo);
            int newWinnerElo = this.CalculateElo(clientWinner, true, clientWinner.Player.Elo, clientLoser.Player.Elo);
            this.gameSendBusiness.EndGame(clientIdThatLost, newWinnerElo, newLoserElo);
            Lobby.CodeUsed.Remove(lobbyCode);
            this.serveur.Lobbies.Remove(lobbyCode);
        }

        HelperManager.Instance.DestroyGameObject(localGameManager.gameObject);
    }

    public void EndGameWhenNoWaveAvailable(LocalGameManager localGameManager)
    {
        List<Client> currentClients = this.serveur.Lobbies[this.serveur.Clients[localGameManager.GamersGameManager[0].Id].Gamer.LobbyCode].Clients;
        Dictionary<int, float> playersHp = new Dictionary<int, float>();

        // Get hp of all client
        foreach (Client client in currentClients)
        {
            playersHp.Add(client.Id, client.Gamer.Life);
        }

        // If they all have the same hp --> Draw Game
        // Else --> The lower one loose
        if (playersHp.Values.Distinct().Count() == 1)
        {
            this.DrawGame(currentClients[0].Gamer.LobbyCode, localGameManager);
        }
        else
        {
            this.EndGame(playersHp.FirstOrDefault(x => x.Value == playersHp.Values.Min()).Key, localGameManager);
        }
    }

    public async Task LoadGameDone(int clientId, int checkClientId)
    {
        if (clientId == checkClientId)
        {
            this.serveur.Clients[clientId].Gamer.IsReady = true;
            Lobby lobby = this.serveur.Lobbies[this.serveur.Clients[clientId].Gamer.LobbyCode];

            foreach (Client client in lobby.Clients)
            {
                if (!client.Gamer.IsReady)
                {
                    return;
                }
            }

            List<Wave> waves = await this.waveClient.List();
            this.CreateGame(lobby, this.mobs, waves);
            this.gameSendBusiness.CreateGame(lobby.Clients);
        }
    }

    public GameObject StartGame(GameManager gameManager, int player1Id, int player2Id, int lobbyCode, List<Mob> mobs)
    {
        Debug.Log("Start a game");
        GameObject game = HelperManager.Instance.InstantiateGameObject(gameManager.GamePrefab, gameManager.GridLayoutTransform);
        LocalGameManager localGameManager = game.GetComponent<LocalGameManager>();
        localGameManager.LobbyCode = lobbyCode;
        localGameManager.Mobs = mobs;
        localGameManager.GameBusiness = gameManager.GameBusiness;
        localGameManager.MobBusiness = gameManager.MobBusiness;

        List<GamerGameManager> playersGameManager = localGameManager.GamersGameManager;
        gameManager.SetPlayersId(player1Id, 0, playersGameManager);
        gameManager.SetPlayersId(player2Id, 1, playersGameManager);

        return game;
    }

    private int CalculateElo(Client client, bool? isWin, int hisElo, int opponentElo)
    {
        float pourcentageOfWin = 1 / (1 + Mathf.Pow(10, -(hisElo - opponentElo) / 400));

        int newElo = (int) (isWin.HasValue
            ? isWin.Value ? Math.Round(hisElo + (20 * (1 - pourcentageOfWin))) : Math.Round(hisElo + (20 * (0 - pourcentageOfWin)))
            : Math.Round(hisElo + (20 * (0.5 - pourcentageOfWin))));

        client.Player.Elo = newElo;

        // We don't await because we need to
        // Also it make problem if we do
        this.playerClient.UpdateElo(client.Player);

        return newElo;
    }

    private void CreateGame(Lobby lobby, List<Mob> mobs, List<Wave> waves)
    {
        Debug.Log("Start a game");
        GameObject game = HelperManager.Instance.InstantiateGameObject(GameManager.Instance.GamePrefab, GameManager.Instance.GridLayoutTransform);
        LocalGameManager localGameManager = game.GetComponent<LocalGameManager>();
        localGameManager.Initialize(lobby.Code, mobs, waves, GameManager.Instance.GameBusiness, GameManager.Instance.MobBusiness);

        List<GamerGameManager> playersGameManager = localGameManager.GamersGameManager;
        GameManager.Instance.SetPlayersId(lobby.Clients[0].Id, 0, playersGameManager);
        GameManager.Instance.SetPlayersId(lobby.Clients[1].Id, 1, playersGameManager);
        lobby.Game = game;
    }

    private void CreateLobby(int fromClient, bool isPrivate)
    {

        int code;
        do
        {
            code = UnityEngine.Random.Range(1000, 100000);
        } while (Lobby.CodeUsed.Contains(code));

        Lobby.CodeUsed.Add(code);
        Lobby lobby = new Lobby(code)
        {
            IsPrivate = isPrivate
        };

        this.serveur.Lobbies.Add(lobby.Code, lobby);
        this.SetGamerToLobby(fromClient, lobby);

        if (isPrivate)
        {
            this.gameSendBusiness.SendLobbyCode(fromClient, code);
        }
    }

    private void SetGamerToLobby(int fromClient, Lobby lobby)
    {
        lobby.Clients.Add(this.serveur.Clients[fromClient]);
        this.serveur.Clients[fromClient].Gamer = new Gamer(fromClient)
        {
            LobbyCode = lobby.Code,
            IsReady = false,
        };
    }
}