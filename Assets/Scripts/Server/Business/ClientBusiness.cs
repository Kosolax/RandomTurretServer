using System;
using System.Net.Sockets;

using UnityEngine;

using Zenject;

public class ClientBusiness
{
    [Inject]
    private readonly GameBusiness gameBusiness;

    [Inject]
    private readonly Serveur server;

    [Inject]
    private readonly SessionBusiness sessionBusiness;

    [Inject]
    private readonly TCPBusiness tcpBusiness;

    public void Connect(TcpClient socket, Client client)
    {
        this.tcpBusiness.Connect(socket, client);
        client.Tcp.Stream.BeginRead(client.Tcp.ReceiveBuffer, 0, this.server.DataBufferSize, ar => { this.ReceiveCallback(ar, client); }, null);
        this.sessionBusiness.Welcome(client.Tcp.Id, "Welcome to the server!");
    }

    public void Disconnect(Client client)
    {
        Debug.Log($"{client.Tcp.Socket.Client.RemoteEndPoint} has disconnected.");
        this.tcpBusiness.Disconnect(client);

        int lobbyCode = this.server.Clients[client.Id].Gamer.LobbyCode;
        this.gameBusiness.DisconnectedFromGame(client.Id);
    }

    private void ReceiveCallback(IAsyncResult result, Client client)
    {
        try
        {
            int byteLength = client.Tcp.Stream.EndRead(result);
            if (byteLength <= 0)
            {
                this.Disconnect(this.server.Clients[client.Id]);
                return;
            }

            byte[] data = new byte[byteLength];
            Array.Copy(client.Tcp.ReceiveBuffer, data, byteLength);

            client.Tcp.ReceivedData.Reset(this.tcpBusiness.HandleData(data, client)); // Reset receivedData if all data was handled
            client.Tcp.Stream.BeginRead(client.Tcp.ReceiveBuffer, 0, this.server.DataBufferSize, ar => { this.ReceiveCallback(ar, client); }, null);
        }
        catch (Exception ex)
        {
            Debug.Log($"Error receiving TCP data: {ex}");
            this.Disconnect(this.server.Clients[client.Id]);
        }
    }
}