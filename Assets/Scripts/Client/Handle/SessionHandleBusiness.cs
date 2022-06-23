using System.Threading.Tasks;

using Newtonsoft.Json;

using UnityEngine;

using Zenject;

public class SessionHandleBusiness
{
    [Inject]
    private readonly SessionBusiness sessionBusiness;

    public async Task Connection(int fromClient, Packet packet)
    {
        int clientIdCheck = packet.ReadInt();
        Player player = JsonConvert.DeserializeObject<Player>(packet.ReadString());
        await this.sessionBusiness.Connection(fromClient, clientIdCheck, player);
    }

    public async Task Register(int fromClient, Packet packet)
    {
        int clientIdCheck = packet.ReadInt();
        Player player = JsonConvert.DeserializeObject<Player>(packet.ReadString());
        Debug.Log(player.Pseudo);
        await this.sessionBusiness.Register(fromClient, clientIdCheck, player);
    }
}