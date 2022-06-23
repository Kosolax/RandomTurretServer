using System;
using System.Collections.Generic;
using System.Net.Sockets;

using UnityEngine;

using Zenject;

public class TCPBusiness
{
    [Inject]
    private readonly Serveur server;

    public void Connect(TcpClient socket, Client client)
    {
        client.Tcp.Socket = socket;
        client.Tcp.Socket.ReceiveBufferSize = this.server.DataBufferSize;
        client.Tcp.Socket.SendBufferSize = this.server.DataBufferSize;

        client.Tcp.Stream = client.Tcp.Socket.GetStream();

        client.Tcp.ReceivedData = new Packet();

        client.Tcp.ReceiveBuffer = new byte[this.server.DataBufferSize];
    }

    public void Disconnect(Client client)
    {
        client.Tcp.Socket.Close();
        client.Tcp.Stream = null;
        client.Tcp.ReceivedData = null;
        client.Tcp.ReceiveBuffer = null;
        client.Tcp.Socket = null;
        client.Player = null;
    }

    public bool HandleData(byte[] data, Client client)
    {
        int packetLength = 0;

        client.Tcp.ReceivedData.SetBytes(data);

        if (client.Tcp.ReceivedData.UnreadLength() >= 4)
        {
            // If client's received data contains a packet
            packetLength = client.Tcp.ReceivedData.ReadInt();
            if (packetLength <= 0)
            {
                // If packet contains no data
                return true; // Reset receivedData instance to allow it to be reused
            }
        }

        while (packetLength > 0 && packetLength <= client.Tcp.ReceivedData.UnreadLength())
        {
            // While packet contains data AND packet data length doesn't exceed the length of the packet we're reading
            byte[] packetBytes = client.Tcp.ReceivedData.ReadBytes(packetLength);
            ThreadManager.ExecuteOnMainThread(async () =>
            {
                using (Packet packet = new Packet(packetBytes))
                {
                    int packetId = packet.ReadInt();
                    await this.server.PacketHandlers[packetId](client.Tcp.Id, packet); // Call appropriate method to handle the packet
                }
            });

            packetLength = 0; // Reset packet length
            if (client.Tcp.ReceivedData.UnreadLength() >= 4)
            {
                // If client's received data contains another packet
                packetLength = client.Tcp.ReceivedData.ReadInt();
                if (packetLength <= 0)
                {
                    // If packet contains no data
                    return true; // Reset receivedData instance to allow it to be reused
                }
            }
        }

        if (packetLength <= 1)
        {
            return true; // Reset receivedData instance to allow it to be reused
        }

        return false;
    }

    public void SendTCPDataToAll(Packet packet)
    {
        packet.WriteLength();
        for (int i = 1; i <= this.server.MaxPlayers; i++)
        {
            this.SendData(packet, this.server.Clients[i]);
        }
    }

    public void SendTCPDataToAllExceptSome(List<int> exceptClients, Packet packet)
    {
        packet.WriteLength();
        for (int i = 1; i <= this.server.MaxPlayers; i++)
        {
            if (!exceptClients.Contains(i))
            {
                this.SendData(packet, this.server.Clients[i]);
            }
        }
    }

    public void SendTCPDataToMany(List<int> clientIds, Packet packet)
    {
        packet.WriteLength();
        for (int i = 0; i < clientIds.Count; i++)
        {
            this.SendData(packet, this.server.Clients[clientIds[i]]);
        }
    }

    public void SendTCPDataToOne(int toClient, Packet packet)
    {
        packet.WriteLength();
        this.SendData(packet, this.server.Clients[toClient]);
    }

    private void SendData(Packet packet, Client client)
    {
        try
        {
            if (client.Tcp.Socket != null)
            {
                client.Tcp.Stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Error sending data to player {client.Tcp.Id} via TCP: {ex}");
        }
    }
}