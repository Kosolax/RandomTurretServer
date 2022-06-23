using System.Collections.Generic;

using UnityEngine;

public class Lobby
{
    public static List<int> CodeUsed = new List<int>();

    public Lobby(int code)
    {
        this.IsGameStarted = false;
        this.Clients = new List<Client>();
        this.Code = code;
    }

    public static int MAXPLAYER { get; set; } = 2;

    public List<int> ClientIds { get; set; }

    public List<Client> Clients { get; set; }

    public int Code { get; set; }

    public GameObject Game { get; set; }

    public bool IsGameStarted { get; set; }

    public bool IsPrivate { get; set; }
}