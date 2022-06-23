using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

using UnityEngine;

using Zenject;

public class ServerBusiness
{
    [Inject]
    private readonly GameHandleBusiness askGameHandleBusiness;

    [Inject]
    private readonly ClientBusiness clientBusiness;

    [Inject]
    private readonly Serveur server;

    [Inject]
    private readonly SessionHandleBusiness sessionHandleBusiness;

    [Inject]
    private readonly TowerHandleBusiness towerHandleBusiness;

    public void Start(int maxPlayers, int port)
    {
        this.server.MaxPlayers = maxPlayers;
        this.server.Port = port;

        for (int i = 1; i <= this.server.MaxPlayers; i++)
        {
            this.server.Clients.Add(i, new Client(i));
        }

        this.server.PacketHandlers = new Dictionary<int, Serveur.PacketHandler>()
        {
            { (int)ClientPackets.AskingConnection, this.sessionHandleBusiness.Connection },
            { (int)ClientPackets.AskingRegister, this.sessionHandleBusiness.Register },
            { (int)ClientPackets.AskForGame, this.askGameHandleBusiness.AskForGame },
            { (int)ClientPackets.CancelAskForGame, this.askGameHandleBusiness.CancelAskForGame },
            { (int)ClientPackets.InvokeTower, this.towerHandleBusiness.InvokeTower },
            { (int)ClientPackets.MergeTower, this.towerHandleBusiness.MergeTower },
            { (int)ClientPackets.LoadGameDone, this.askGameHandleBusiness.LoadGameDone },
            { (int)ClientPackets.AskForPrivateGame, this.askGameHandleBusiness.AskForPrivateGame },
            { (int)ClientPackets.AskJoinPrivateGame, this.askGameHandleBusiness.AskJoinPrivateGame },
        };

        IPAddress ip = IPAddress.Parse(this.server.Ip);
        this.server.TcpListener = new TcpListener(ip, this.server.Port);
        this.server.TcpListener.Start();
        this.server.TcpListener.BeginAcceptTcpClient(this.TCPConnectCallback, null);

        Debug.Log($"Server started successfully on {ip} with {this.server.MaxPlayers} max players on the port {this.server.Port}.");
    }

    private void TCPConnectCallback(IAsyncResult result)
    {
        TcpClient tcpClient = this.server.TcpListener.EndAcceptTcpClient(result);
        this.server.TcpListener.BeginAcceptTcpClient(this.TCPConnectCallback, null);

        Debug.Log($"Incoming connection from {tcpClient.Client.RemoteEndPoint}.");

        for (int i = 1; i <= this.server.MaxPlayers; i++)
        {
            if (this.server.Clients[i].Tcp.Socket == null)
            {
                this.clientBusiness.Connect(tcpClient, this.server.Clients[i]);
                return;
            }
        }

        Debug.Log("The server is full.");
    }
}