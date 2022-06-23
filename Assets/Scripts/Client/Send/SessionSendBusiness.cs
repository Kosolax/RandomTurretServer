using Newtonsoft.Json;

using Zenject;

public class SessionSendBusiness
{
    [Inject]
    private readonly TCPBusiness tcpBusiness;

    public void DisplayPlayerData(int toClient, Player player)
    {
        using (Packet packet = new Packet((int) ServerPackets.SendPlayerData))
        {
            packet.Write(JsonConvert.SerializeObject(player));
            this.tcpBusiness.SendTCPDataToOne(toClient, packet);
        }
    }

    public void SendConnectionError(int toClient, string key)
    {
        using (Packet packet = new Packet((int) ServerPackets.SendConnectionError))
        {
            packet.Write(key);
            this.tcpBusiness.SendTCPDataToOne(toClient, packet);
        }
    }

    public void SendRegisterError(int toClient, string key)
    {
        using (Packet packet = new Packet((int) ServerPackets.SendRegisterError))
        {
            packet.Write(key);
            this.tcpBusiness.SendTCPDataToOne(toClient, packet);
        }
    }

    public void Welcome(int toClient, string msg)
    {
        using (Packet packet = new Packet((int) ServerPackets.Welcome))
        {
            packet.Write(msg);
            packet.Write(toClient);

            this.tcpBusiness.SendTCPDataToOne(toClient, packet);
        }
    }
}