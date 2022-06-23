using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Zenject;

public class GameSendBusiness
{
    [Inject]
    private readonly Serveur serveur;

    [Inject]
    private readonly TCPBusiness tcpBusiness;

    public void CancelLoadingGame(int toClient)
    {
        using (Packet packet = new Packet((int) ServerPackets.CanceledLoadingGame))
        {
            this.tcpBusiness.SendTCPDataToOne(toClient, packet);
        }
    }

    public void CreateGame(List<Client> clients)
    {
        foreach (Client client in clients)
        {
            using (Packet packet = new Packet((int) ServerPackets.SendCreateGame))
            {
                this.tcpBusiness.SendTCPDataToOne(client.Id, packet);
            }
        }
    }

    public void DrawGame(int lobbyCode, int newWinnerElo, int newLoserElo)
    {
        List<int> clientsId = this.serveur.Lobbies[lobbyCode].Clients
            .Select(x => x.Id)
            .ToList();

        foreach (int currentClientId in clientsId)
        {
            if (currentClientId == clientsId.FirstOrDefault())
            {
                using (Packet packet = new Packet((int) ServerPackets.Draw))
                {
                    packet.Write(newWinnerElo);
                    this.tcpBusiness.SendTCPDataToOne(currentClientId, packet);
                }
            }
            else
            {
                using (Packet packet = new Packet((int) ServerPackets.Draw))
                {
                    packet.Write(newLoserElo);
                    this.tcpBusiness.SendTCPDataToOne(currentClientId, packet);
                }
            }
        }
    }

    public void EndGame(int clientIdThatLost, int newWinnerElo, int newLoserElo)
    {
        int lobbyCode = this.serveur.Clients[clientIdThatLost].Gamer.LobbyCode;
        List<int> clientsId = this.serveur.Lobbies[lobbyCode].Clients
            .Select(x => x.Id)
            .ToList();

        foreach (int currentClientId in clientsId)
        {
            if (currentClientId == clientIdThatLost)
            {
                using (Packet packet = new Packet((int) ServerPackets.Lose))
                {
                    packet.Write(newLoserElo);
                    this.tcpBusiness.SendTCPDataToOne(currentClientId, packet);
                }
            }
            else
            {
                using (Packet packet = new Packet((int) ServerPackets.Win))
                {
                    packet.Write(newWinnerElo);
                    this.tcpBusiness.SendTCPDataToOne(currentClientId, packet);
                }
            }
        }
    }

    public void LoadGame(List<Client> clients, int currentClientId, List<Mob> mobs)
    {
        Client currentClient = clients.Where(x => x.Id == currentClientId).FirstOrDefault();
        Client enemyClient = clients.Where(x => x.Id != currentClientId).FirstOrDefault();

        using (Packet packet = new Packet((int) ServerPackets.LoadGame))
        {
            packet.Write(enemyClient.Player.Pseudo);
            packet.Write(JsonConvert.SerializeObject(enemyClient.Player.Towers));
            packet.Write(JsonConvert.SerializeObject(currentClient.Player.Towers));
            packet.Write(enemyClient.Player.Elo);
            packet.Write(currentClient.Player.Elo);
            packet.Write(JsonConvert.SerializeObject(mobs));
            this.tcpBusiness.SendTCPDataToOne(currentClient.Id, packet);
        }

        using (Packet packet = new Packet((int) ServerPackets.LoadGame))
        {
            packet.Write(currentClient.Player.Pseudo);
            packet.Write(JsonConvert.SerializeObject(currentClient.Player.Towers));
            packet.Write(JsonConvert.SerializeObject(enemyClient.Player.Towers));
            packet.Write(currentClient.Player.Elo);
            packet.Write(enemyClient.Player.Elo);
            packet.Write(JsonConvert.SerializeObject(mobs));
            this.tcpBusiness.SendTCPDataToOne(enemyClient.Id, packet);
        }
    }

    public void SendLobbyCode(int currentClientId, int lobbyCode)
    {
        using (Packet packet = new Packet((int) ServerPackets.SendLobbyCode))
        {
            packet.Write(lobbyCode);
            this.tcpBusiness.SendTCPDataToOne(currentClientId, packet);
        }
    }

    public void SetJoinError(int clientId)
    {
        using (Packet packet = new Packet((int) ServerPackets.SetJoinError))
        {
            this.tcpBusiness.SendTCPDataToOne(clientId, packet);
        }
    }
}