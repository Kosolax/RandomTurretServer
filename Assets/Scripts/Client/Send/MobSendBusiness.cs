using System.Collections.Generic;
using System.Linq;

using Zenject;

public class MobSendBusiness
{
    [Inject]
    private readonly Serveur serveur;

    [Inject]
    private readonly TCPBusiness tcpBusiness;

    public void KillMob(int clientId, int indexMob)
    {
        int lobbyCode = this.serveur.Clients[clientId].Gamer.LobbyCode;
        List<int> clientsId = this.serveur.Lobbies[lobbyCode].Clients
            .Select(x => x.Id)
            .ToList();

        foreach (int currentClientId in clientsId)
        {
            using (Packet packet = new Packet((int) ServerPackets.KillMob))
            {
                packet.Write(clientId);
                packet.Write(indexMob);

                this.tcpBusiness.SendTCPDataToOne(currentClientId, packet);
            }
        }
    }

    public void SpawnMob(int mobId, int clientId, float difficultyMultiplier)
    {
        int lobbyCode = this.serveur.Clients[clientId].Gamer.LobbyCode;
        List<int> clientsId = this.serveur.Lobbies[lobbyCode].Clients
            .Select(x => x.Id)
            .ToList();

        foreach (int currentClientId in clientsId)
        {
            using (Packet packet = new Packet((int) ServerPackets.SpawnMob))
            {
                packet.Write(mobId);
                packet.Write(clientId);
                packet.Write(difficultyMultiplier);
                this.tcpBusiness.SendTCPDataToOne(currentClientId, packet);
            }
        }
    }

    public void UpdateLife(int clientId, float currentLife, int idMob)
    {
        int lobbyCode = this.serveur.Clients[clientId].Gamer.LobbyCode;
        List<int> clientsId = this.serveur.Lobbies[lobbyCode].Clients
            .Select(x => x.Id)
            .ToList();

        foreach (int currentClientId in clientsId)
        {
            using (Packet packet = new Packet((int) ServerPackets.UpdateMobLife))
            {
                packet.Write(clientId);
                packet.Write(currentLife);
                packet.Write(idMob);
                this.tcpBusiness.SendTCPDataToOne(currentClientId, packet);
            }
        }
    }

    public void UpdateWaveNumber(int waveNumber, int clientId)
    {
        int lobbyCode = this.serveur.Clients[clientId].Gamer.LobbyCode;
        List<int> clientsId = this.serveur.Lobbies[lobbyCode].Clients
            .Select(x => x.Id)
            .ToList();

        foreach (int currentClientId in clientsId)
        {
            using (Packet packet = new Packet((int) ServerPackets.UpdateWaveNumber))
            {
                packet.Write(waveNumber);
                this.tcpBusiness.SendTCPDataToOne(currentClientId, packet);
            }
        }
    }
}