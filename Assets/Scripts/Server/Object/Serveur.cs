using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

public class Serveur
{
    public Serveur()
    {
        this.Ip = "192.168.1.59";
        this.DataBufferSize = 4096;
        this.Clients = new Dictionary<int, Client>();
        this.PacketHandlers = new Dictionary<int, PacketHandler>();
        this.Lobbies = new Dictionary<int, Lobby>();
    }

    public delegate Task PacketHandler(int fromClient, Packet packet);

    public Dictionary<int, Client> Clients { get; set; }

    public int DataBufferSize { get; private set; }

    public string Ip { get; set; }

    public Dictionary<int, Lobby> Lobbies { get; set; }

    public int MaxPlayers { get; set; }

    public Dictionary<int, PacketHandler> PacketHandlers { get; set; }

    public int Port { get; set; }

    public TcpListener TcpListener { get; set; }
}
