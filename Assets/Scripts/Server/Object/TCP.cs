using System.Net.Sockets;

public class TCP
{
    public TCP(int id)
    {
        this.Id = id;
    }

    public int Id { get; set; }

    public byte[] ReceiveBuffer { get; set; }

    public Packet ReceivedData { get; set; }

    public TcpClient Socket { get; set; }

    public NetworkStream Stream { get; set; }
}