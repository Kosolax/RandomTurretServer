using System.Collections.Generic;
using System.Linq;

using Zenject;

public class GamerSendBusiness
{
    [Inject]
    private readonly Serveur serveur;

    [Inject]
    private readonly TCPBusiness tcpBusiness;

    public void UpdateLife(int clientId, float currentLife)
    {
        int lobbyCode = this.serveur.Clients[clientId].Gamer.LobbyCode;
        List<int> clientsId = this.serveur.Lobbies[lobbyCode].Clients
            .Select(x => x.Id)
            .ToList();

        foreach (int currentClientId in clientsId)
        {
            using (Packet packet = new Packet((int) ServerPackets.UpdateLife))
            {
                packet.Write(clientId);
                packet.Write(currentLife);
                this.tcpBusiness.SendTCPDataToOne(currentClientId, packet);
            }
        }
    }

    public void UpdateMoney(int toClient, int currentMoney)
    {
        using (Packet packet = new Packet((int) ServerPackets.UpdateMoney))
        {
            packet.Write(currentMoney);
            this.tcpBusiness.SendTCPDataToOne(toClient, packet);
        }
    }
}