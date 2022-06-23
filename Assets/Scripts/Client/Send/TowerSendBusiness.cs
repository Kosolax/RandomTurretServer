using System.Collections.Generic;
using System.Linq;

using Zenject;

public class TowerSendBusiness
{
    [Inject]
    private readonly Serveur serveur;

    [Inject]
    private readonly TCPBusiness tcpBusiness;

    public void InvokeTower(int clientId, int pathMapIndex, int towerIndex, int level)
    {
        int lobbyCode = this.serveur.Clients[clientId].Gamer.LobbyCode;
        List<int> clientsId = this.serveur.Lobbies[lobbyCode].Clients
            .Select(x => x.Id)
            .ToList();

        foreach (int currentClientId in clientsId)
        {
            using (Packet packet = new Packet((int) ServerPackets.InvokeTower))
            {
                packet.Write(clientId);
                packet.Write(pathMapIndex);
                packet.Write(towerIndex);
                packet.Write(level);
                this.tcpBusiness.SendTCPDataToOne(currentClientId, packet);
            }
        }
    }

    public void MergeTower(int clientId, int draggedTowerIndex, int droppedTowerIndex)
    {
        int lobbyCode = this.serveur.Clients[clientId].Gamer.LobbyCode;
        List<int> clientsId = this.serveur.Lobbies[lobbyCode].Clients
            .Select(x => x.Id)
            .ToList();

        foreach (int currentClientId in clientsId)
        {
            using (Packet packet = new Packet((int) ServerPackets.MergeTower))
            {
                packet.Write(clientId);
                packet.Write(draggedTowerIndex);
                packet.Write(droppedTowerIndex);

                this.tcpBusiness.SendTCPDataToOne(currentClientId, packet);
            }
        }
    }

    public void RemoveTower(int clientId, int draggedTowerIndex)
    {
        int lobbyCode = this.serveur.Clients[clientId].Gamer.LobbyCode;
        List<int> clientsId = this.serveur.Lobbies[lobbyCode].Clients
            .Select(x => x.Id)
            .ToList();

        foreach (int currentClientId in clientsId)
        {
            using (Packet packet = new Packet((int) ServerPackets.RemoveTower))
            {
                packet.Write(clientId);
                packet.Write(draggedTowerIndex);

                this.tcpBusiness.SendTCPDataToOne(currentClientId, packet);
            }
        }
    }

    public void UpdateTowerPrice(int toClient, int towerPrice)
    {
        using (Packet packet = new Packet((int) ServerPackets.UpdateTowerPrice))
        {
            packet.Write(towerPrice);
            this.tcpBusiness.SendTCPDataToOne(toClient, packet);
        }
    }
}